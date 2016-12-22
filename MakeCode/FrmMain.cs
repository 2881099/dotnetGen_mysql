using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Model;

namespace MakeCode {
	public partial class FrmMain : Form {
		public FrmMain() {
			InitializeComponent();
		}

		private ClientInfo _client;
		private ClientSocket _socket;
		public List<TableInfo> _tables = new List<TableInfo>();

		public string ConnectionString {
			get {
				string connStr = "Data Source={0};User ID={1};Password={2};Initial Catalog={3};Charset=utf8";
				return string.Format(connStr, this._client.Server, this._client.Username, this._client.Password, this._client.Database);
			}
		}

		private void BindGridView() {
			DataGridViewLinkColumn dgvColName = new DataGridViewLinkColumn();
			dgvColName.Name = "dgvColName";
			dgvColName.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
			dgvColName.DataPropertyName = "FullName";
			dgvColName.HeaderText = "Name";
			dgvColName.DisplayIndex = 1;
			dgvColName.Width = 206;

			DataGridViewCheckBoxColumn dgvColIsOutput = new DataGridViewCheckBoxColumn();
			dgvColIsOutput.Name = "dgvColIsOutput";
			dgvColIsOutput.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
			dgvColIsOutput.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dgvColIsOutput.DataPropertyName = "IsOutput";
			dgvColIsOutput.HeaderText = "Ins Sel";
			dgvColIsOutput.DisplayIndex = 2;
			dgvColIsOutput.Width = 60;

			this.dgvGridview.AutoGenerateColumns = false;
			this.dgvGridview.DataSource = null;

			this.dgvGridview.Columns.Clear();
			this.dgvGridview.Columns.AddRange(new DataGridViewColumn[]{
				dgvColName,
				dgvColIsOutput
			});

			dgvGridview.DataSource = _tables;
			txtProject_TextChanged(this, EventArgs.Empty);
		}

		private void FrmMain_Load(object sender, EventArgs e) {
			Uri uri = new Uri("tcp://" + Settings.Default.server + "/");
			this._socket = new ClientSocket();
			this._socket.Error += Socket_OnError;
			this._socket.Receive += Socket_OnReceive;
			this._socket.Connect(uri.Host, uri.Port);
			this.Closed += delegate(object sender2, EventArgs e2) {
				this._socket.Dispose();
			};
		}

		private void Socket_OnError(object sender, ClientSocketErrorEventArgs e) {
			//Lib.Msgbox(e.Exception.Message + "\r\n\r\n" + e.Exception.StackTrace, MessageBoxIcon.Error);
			Lib.Msgbox(e.Exception.Message, MessageBoxIcon.Error);
		}

		private void Socket_OnReceive(object sender, ClientSocketReceiveEventArgs e) {
			SocketMessager messager = null;
			switch (e.Messager.Action) {
				case "ExecuteDataSet":
					string sql = e.Messager.Arg.ToString();
					DataSet ds = null;
					try {
						ds = ExecuteDataSet(sql);
					} catch(Exception ex) {
						this.Socket_OnError(this, new ClientSocketErrorEventArgs(ex, 0));
					}
					messager = new SocketMessager(e.Messager.Action, ds);
					messager.Id = e.Messager.Id;
					this._socket.Write(messager);
					break;
				case "ExecuteNonQuery":
					string sql2 = e.Messager.Arg.ToString();
					int val = 0;
					try {
						val = ExecuteNonQuery(sql2);
					} catch (Exception ex) {
						this.Socket_OnError(this, new ClientSocketErrorEventArgs(ex, 0));
					}
					messager = new SocketMessager(e.Messager.Action, val);
					messager.Id = e.Messager.Id;
					this._socket.Write(messager);
					break;
				default:
					Lib.Msgbox("您当前使用的版本未能实现功能！");
					break;
			}
		}

		public int ExecuteNonQuery(string cmdText) {
			int val = 0;
			using (MySqlConnection conn = new MySqlConnection(this.ConnectionString)) {
				MySqlCommand cmd = new MySqlCommand(cmdText, conn);
				try {
					cmd.Connection.Open();
					val = cmd.ExecuteNonQuery();
				} catch {
					cmd.Parameters.Clear();
					cmd.Connection.Close();
					throw;
				}
			}
			return val;
		}
		public DataSet ExecuteDataSet(string cmdText) {
			DataSet ds = new DataSet();
			using (MySqlConnection conn = new MySqlConnection(this.ConnectionString)) {
				MySqlCommand cmd = new MySqlCommand(cmdText, conn);
				MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
				try {
					cmd.Connection.Open();
					sda.Fill(ds);
				} catch {
					cmd.Parameters.Clear();
					cmd.Connection.Close();
					throw;
				}
				cmd.Connection.Close();
				cmd.Parameters.Clear();
			}
			return ds;
		}

		private void btnConnect_Click(object sender, EventArgs e) {
			this.btnConnect.Enabled = false;
			if (this.btnConnect.Text == "Connect") {
				this._client = new ClientInfo(this.txtServer.Text, this.txtUsername.Text, this.txtPassword.Text);
				List<DatabaseInfo> dbs = null;
				SocketMessager messager = new SocketMessager("GetDatabases", this._client);
				this._socket.Write(messager, delegate(object sender2, ClientSocketReceiveEventArgs e2) {
					dbs = e2.Messager.Arg as List<DatabaseInfo>;
				});
				if (dbs == null) {
					this.btnConnect.Enabled = true;
					return;
				}
				this.cmbDatabase.DisplayMember = "Name";
				this.cmbDatabase.DataSource = dbs;

				if (this.cmbDatabase.Items.Count > 0) {
					this.cmbDatabase.SelectedIndex = 0;
					this.cmbDatabase.Enabled = true;
				}
				this.txtServer.Enabled = this.txtUsername.Enabled = this.txtPassword.Enabled = false;
			} else {
				this.txtSolution.Clear();
				this.cmbDatabase.DataSource = null;
				this.cmbDatabase.Enabled = false;
				this.btnBuild.Enabled = false;

				this.txtServer.Enabled = this.txtUsername.Enabled = this.txtPassword.Enabled = true;

				this.dgvGridview.DataSource = null;
			}

			this.btnConnect.Text = this.btnConnect.Text == "Connect" ? "DisConnect" : "Connect";
			this.btnConnect.Enabled = true;
		}

		private void cmbDatabase_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.btnConnect.Text == "DisConnect" && this.btnConnect.Enabled == false) return;
			this._client.Database = this.cmbDatabase.Text;
			List<TableInfo> tables = null;
			SocketMessager messager = new SocketMessager("GetTablesByDatabase", this._client.Database);
			this._socket.Write(messager, delegate(object sender2, ClientSocketReceiveEventArgs e2) {
				tables = e2.Messager.Arg as List<TableInfo>;
			});
			this._tables = tables;
			this.BindGridView();
		}

		private void btnBuild_Click(object sender, EventArgs e) {
			if (this._tables.Find(delegate(TableInfo table) {
				return table.IsOutput;
			}) == null) {
				DataGridViewCellMouseEventArgs e2 = new DataGridViewCellMouseEventArgs(1, -1, 1, 1, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
				this.dgvGridview_ColumnHeaderMouseClick(this, e2);
			}
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() != DialogResult.OK) return;

			string selectedPath = fbd.SelectedPath;
			List<BuildInfo> bs = null;
			SocketMessager messager = new SocketMessager("Build", new object[] {
				this.txtSolution.Text,
				this.chkSolution.Checked,
				string.Join("", this._tables.ConvertAll<string>(delegate(TableInfo table){
					return string.Concat(table.IsOutput ? 1 : 0);
				}).ToArray()),
				this.chkWebAdmin.Checked,
				this.chkDownloadRes.Checked
			});
			this._socket.Write(messager, delegate(object sender2, ClientSocketReceiveEventArgs e2) {
				bs = e2.Messager.Arg as List<BuildInfo>;
				if (e2.Messager.Arg is Exception) throw e2.Messager.Arg as Exception;
			}, TimeSpan.FromSeconds(60 * 5));
			if (bs == null) return;

			foreach (BuildInfo b in bs) {
				string path = Path.Combine(selectedPath, b.Path);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				string fileName = Path.GetFileName(b.Path);
				string ext = Path.GetExtension(b.Path);
				Encoding encode = Encoding.UTF8;

				if (fileName.EndsWith(".rar") || fileName.EndsWith(".zip") || fileName.EndsWith(".dll")) {
					using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
						fs.Write(b.Data, 0, b.Data.Length);
						fs.Close();
					}
					continue;
				}

				byte[] data = Deflate.Decompress(b.Data);
				string content = Encoding.UTF8.GetString(data);

				if (string.Compare(fileName, "web.config") == 0) {
					string place = System.Web.HttpUtility.HtmlEncode(this.ConnectionString);
					content = content.Replace("{connectionString}", place);
				}
				if (fileName.EndsWith(".json")) {
					string place = this.ConnectionString.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("'", "\\'");
					content = content.Replace("{connectionString}", this.ConnectionString);
				}
				//if (string.Compare(fileName, "procedure.sql") == 0) {
				//    this.ExecuteNonQuery(content);
				//}
				if (string.Compare(ext, ".refresh") == 0) {
					encode = Encoding.Unicode;
				}
				using (StreamWriter sw = new StreamWriter(path, false, encode)) {
					sw.Write(content);
					sw.Close();
				}
			}
			GC.Collect();

			Lib.Msgbox("The code files be maked in \"" + selectedPath + "\", please check.");
			//System.Diagnostics.Process.Start("iexplore.exe", "http://www.penzz.com/");
		}

		private void txtProject_TextChanged(object sender, EventArgs e) {
			this.btnBuild.Enabled = this._tables != null && this._tables.Count > 0 && this.txtSolution.Text != string.Empty;
		}

		private void dgvGridview_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			if (e.Button == MouseButtons.Left && e.ColumnIndex == 1 && this._tables != null) {
				foreach (TableInfo table in _tables) table.IsOutput = !table.IsOutput && 
					(table.PrimaryKeys.Count > 0 && table.FullName != "dbo.sysdiagrams" || table.Type == "P");
				this.BindGridView();
			}
		}

		private void dgvGridview_CellContentClick(object sender, DataGridViewCellEventArgs e) {
			DataGridView dgv = sender as DataGridView;
			if (dgv != null) {
				bool isFrmMain = dgv.FindForm() is FrmMain;
				if (e.RowIndex >= 0) {
					DataGridViewColumn column = dgv.Columns[e.ColumnIndex];
					DataGridViewRow row = dgv.Rows[e.RowIndex];
					if (isFrmMain && column.Name == "dgvColName" || column.Name == "dgvColView") {
						string pdgvColName = string.Concat(row.Cells["dgvColName"].Value);
						string dgvColValue = string.Concat(column.Name == "dgvColView" ? row.Cells["dgvColView"].Value : null);
						string viewTable = isFrmMain ? pdgvColName : row.Cells["dgvColView"].Tag.ToString();
						string name = isFrmMain ? pdgvColName : dgv.Tag.ToString();

						if (dgvColValue == "FK-x") {
							Lib.Msgbox("引用了外部项目，请直接生成后查看代码！");
							return;
						}

						TableInfo table = _tables.Find(delegate(TableInfo table1) {
							return viewTable == table1.FullName;
						});
						if (table == null) return;

						FrmView frmView = new FrmView();
						frmView.Text = isFrmMain ? (name + " - view") :
							(name + "." + pdgvColName + " - " + table.FullName + " - relation view");

						frmView.dgvGridview.Tag = viewTable;
						foreach (ColumnInfo c1 in table.Columns) {
							string viewText = null;
							object image = c1.IsPrimaryKey ? this.imageList1.Images["PrimaryKey.ico"] : null;

							table.ForeignKeys.FindAll(delegate(ForeignKeyInfo fk) {
								ColumnInfo c2 = fk.Columns.Find(delegate(ColumnInfo c3) {
									return c3.Name == c1.Name;
								});
								if (c2 != null) {
									if (fk.ReferencedTable != null) {
										viewTable = fk.ReferencedTable.FullName;
										viewText = "View";
									} else {
										viewTable = fk.ReferencedTableName;
										viewText = "FK-x";
									}
									if (image == null) image = imageList1.Images["Key.ico"];
								}
								return c2 != null;
							});

							frmView.dgvGridview.Rows.Add(new object[] { image, c1.Name, c1.SqlType, c1.IsNullable, viewText });
							if (viewText != null) frmView.dgvGridview.Rows[frmView.dgvGridview.Rows.Count - 1].Cells["dgvColView"].Tag = viewTable;
						}

						frmView.dgvGridview.CellContentClick += dgvGridview_CellContentClick;
						frmView.ShowDialog();
						frmView.Dispose();
					}
				}
			}
		}

		private void dgvGridview_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (e.RowIndex >= 0) {
				DataGridViewColumn column = ((DataGridView)sender).Columns[e.ColumnIndex];
				DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
				if (column.Name == "dgvColIsOutput") {
					txtProject_TextChanged(sender, e);
				}
			}
		}

		private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
			Properties.Settings.Default.Save();
		}

		private void dgvGridview_DataError(object sender, DataGridViewDataErrorEventArgs e) {
			e.Cancel = true;
		}

		private void dgvGridview_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			if (e.ColumnIndex == 0 && this._tables != null && e.RowIndex < this._tables.Count) {
				switch (this._tables[e.RowIndex].Type) {
					case "P":
						e.CellStyle.BackColor = ColorTranslator.FromHtml("#CDEDFC");
						break;
				}
			}
		}
	}
}
