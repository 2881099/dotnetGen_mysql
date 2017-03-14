using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Model;

namespace MakeCode {
	partial class FrmMain {
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.labServer = new System.Windows.Forms.Label();
			this.labProject = new System.Windows.Forms.Label();
			this.labUsername = new System.Windows.Forms.Label();
			this.labPassword = new System.Windows.Forms.Label();
			this.cmbDatabase = new System.Windows.Forms.ComboBox();
			this.btnBuild = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.dgvGridview = new System.Windows.Forms.DataGridView();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.chkDownloadRes = new System.Windows.Forms.CheckBox();
			this.chkWebAdmin = new System.Windows.Forms.CheckBox();
			this.chkSolution = new System.Windows.Forms.CheckBox();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.txtSolution = new System.Windows.Forms.TextBox();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.labDatabase = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.labPort = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvGridview)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "PrimaryKey.ico");
			this.imageList1.Images.SetKeyName(1, "Key.ico");
			// 
			// labServer
			// 
			this.labServer.AutoSize = true;
			this.labServer.Location = new System.Drawing.Point(10, 323);
			this.labServer.Name = "labServer";
			this.labServer.Size = new System.Drawing.Size(59, 12);
			this.labServer.TabIndex = 16;
			this.labServer.Text = "MySql主机";
			// 
			// labProject
			// 
			this.labProject.AutoSize = true;
			this.labProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labProject.Location = new System.Drawing.Point(302, 323);
			this.labProject.Name = "labProject";
			this.labProject.Size = new System.Drawing.Size(53, 12);
			this.labProject.TabIndex = 27;
			this.labProject.Text = "项目名称";
			// 
			// labUsername
			// 
			this.labUsername.AutoSize = true;
			this.labUsername.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labUsername.Location = new System.Drawing.Point(10, 350);
			this.labUsername.Name = "labUsername";
			this.labUsername.Size = new System.Drawing.Size(41, 12);
			this.labUsername.TabIndex = 18;
			this.labUsername.Text = "用户名";
			// 
			// labPassword
			// 
			this.labPassword.AutoSize = true;
			this.labPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labPassword.Location = new System.Drawing.Point(10, 377);
			this.labPassword.Name = "labPassword";
			this.labPassword.Size = new System.Drawing.Size(41, 12);
			this.labPassword.TabIndex = 20;
			this.labPassword.Text = "密  码";
			// 
			// cmbDatabase
			// 
			this.cmbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDatabase.Enabled = false;
			this.cmbDatabase.FormattingEnabled = true;
			this.cmbDatabase.Location = new System.Drawing.Point(364, 374);
			this.cmbDatabase.Margin = new System.Windows.Forms.Padding(4);
			this.cmbDatabase.Name = "cmbDatabase";
			this.cmbDatabase.Size = new System.Drawing.Size(117, 20);
			this.cmbDatabase.TabIndex = 24;
			this.toolTip1.SetToolTip(this.cmbDatabase, "请选择一个数据库");
			this.cmbDatabase.SelectedIndexChanged += new System.EventHandler(this.cmbDatabase_SelectedIndexChanged);
			// 
			// btnBuild
			// 
			this.btnBuild.Enabled = false;
			this.btnBuild.Location = new System.Drawing.Point(487, 373);
			this.btnBuild.Name = "btnBuild";
			this.btnBuild.Size = new System.Drawing.Size(89, 21);
			this.btnBuild.TabIndex = 25;
			this.btnBuild.Text = "生成";
			this.toolTip1.SetToolTip(this.btnBuild, "生成");
			this.btnBuild.UseVisualStyleBackColor = true;
			this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(208, 374);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(90, 21);
			this.btnConnect.TabIndex = 22;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// dgvGridview
			// 
			this.dgvGridview.AllowUserToAddRows = false;
			this.dgvGridview.AllowUserToResizeRows = false;
			this.dgvGridview.BackgroundColor = System.Drawing.SystemColors.HighlightText;
			this.dgvGridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvGridview.Location = new System.Drawing.Point(12, 12);
			this.dgvGridview.Name = "dgvGridview";
			this.dgvGridview.RowHeadersVisible = false;
			this.dgvGridview.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgvGridview.RowTemplate.Height = 23;
			this.dgvGridview.Size = new System.Drawing.Size(286, 302);
			this.dgvGridview.TabIndex = 26;
			this.dgvGridview.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGridview_CellContentClick);
			this.dgvGridview.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvGridview_CellFormatting);
			this.dgvGridview.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGridview_CellValueChanged);
			this.dgvGridview.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvGridview_ColumnHeaderMouseClick);
			this.dgvGridview.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvGridview_DataError);
			// 
			// chkDownloadRes
			// 
			this.chkDownloadRes.AutoSize = true;
			this.chkDownloadRes.Checked = global::MakeCode.Properties.Settings.Default.chkDownloadRes_checked;
			this.chkDownloadRes.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MakeCode.Properties.Settings.Default, "chkDownloadRes_checked", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkDownloadRes.Location = new System.Drawing.Point(397, 347);
			this.chkDownloadRes.Name = "chkDownloadRes";
			this.chkDownloadRes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkDownloadRes.Size = new System.Drawing.Size(84, 16);
			this.chkDownloadRes.TabIndex = 37;
			this.chkDownloadRes.Text = "下载资源包";
			this.toolTip1.SetToolTip(this.chkDownloadRes, "是否下载资源包，因网速原因，可能会影响生成速度");
			this.chkDownloadRes.UseVisualStyleBackColor = true;
			// 
			// chkWebAdmin
			// 
			this.chkWebAdmin.AutoSize = true;
			this.chkWebAdmin.Checked = global::MakeCode.Properties.Settings.Default.chkWebAdmin_checked;
			this.chkWebAdmin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MakeCode.Properties.Settings.Default, "chkWebAdmin_checked", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkWebAdmin.Location = new System.Drawing.Point(486, 322);
			this.chkWebAdmin.Name = "chkWebAdmin";
			this.chkWebAdmin.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkWebAdmin.Size = new System.Drawing.Size(96, 16);
			this.chkWebAdmin.TabIndex = 35;
			this.chkWebAdmin.Text = "生成后台管理";
			this.toolTip1.SetToolTip(this.chkWebAdmin, "是否生成 WEB 管理");
			this.chkWebAdmin.UseVisualStyleBackColor = true;
			// 
			// chkSolution
			// 
			this.chkSolution.AutoSize = true;
			this.chkSolution.Checked = global::MakeCode.Properties.Settings.Default.chkSolution_checked;
			this.chkSolution.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MakeCode.Properties.Settings.Default, "chkSolution_checked", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkSolution.Location = new System.Drawing.Point(486, 347);
			this.chkSolution.Name = "chkSolution";
			this.chkSolution.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkSolution.Size = new System.Drawing.Size(96, 16);
			this.chkSolution.TabIndex = 30;
			this.chkSolution.Text = "生成解决方案";
			this.toolTip1.SetToolTip(this.chkSolution, "是否生成解决方案(.sln)和项目文件(.csproj)");
			this.chkSolution.UseVisualStyleBackColor = true;
			// 
			// txtServer
			// 
			this.txtServer.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MakeCode.Properties.Settings.Default, "txtServer_text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtServer.Location = new System.Drawing.Point(81, 320);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(116, 21);
			this.txtServer.TabIndex = 17;
			this.txtServer.Text = global::MakeCode.Properties.Settings.Default.txtServer_text;
			this.toolTip1.SetToolTip(this.txtServer, "数据库地址\r\n如：101.10.131.100");
			// 
			// txtSolution
			// 
			this.txtSolution.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MakeCode.Properties.Settings.Default, "txtSolution_text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtSolution.Location = new System.Drawing.Point(364, 320);
			this.txtSolution.Name = "txtSolution";
			this.txtSolution.Size = new System.Drawing.Size(117, 21);
			this.txtSolution.TabIndex = 28;
			this.txtSolution.Text = global::MakeCode.Properties.Settings.Default.txtSolution_text;
			this.toolTip1.SetToolTip(this.txtSolution, "要生成的解决方案名（不能为空）\r\n如：Nic");
			this.txtSolution.TextChanged += new System.EventHandler(this.txtProject_TextChanged);
			// 
			// txtUsername
			// 
			this.txtUsername.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MakeCode.Properties.Settings.Default, "txtUsername_text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtUsername.Location = new System.Drawing.Point(81, 347);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(116, 21);
			this.txtUsername.TabIndex = 19;
			this.txtUsername.Text = global::MakeCode.Properties.Settings.Default.txtUsername_text;
			this.toolTip1.SetToolTip(this.txtUsername, "数据库用户\r\n如：sa");
			// 
			// txtPassword
			// 
			this.txtPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MakeCode.Properties.Settings.Default, "txtPassword_text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtPassword.Location = new System.Drawing.Point(81, 374);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(116, 21);
			this.txtPassword.TabIndex = 21;
			this.txtPassword.Text = global::MakeCode.Properties.Settings.Default.txtPassword_text;
			this.toolTip1.SetToolTip(this.txtPassword, "密码\r\n如：123456");
			// 
			// labDatabase
			// 
			this.labDatabase.AutoSize = true;
			this.labDatabase.Location = new System.Drawing.Point(302, 378);
			this.labDatabase.Name = "labDatabase";
			this.labDatabase.Size = new System.Drawing.Size(53, 12);
			this.labDatabase.TabIndex = 23;
			this.labDatabase.Text = "Database";
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.webBrowser1);
			this.panel1.Location = new System.Drawing.Point(304, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(270, 302);
			this.panel1.TabIndex = 36;
			// 
			// webBrowser1
			// 
			this.webBrowser1.AllowWebBrowserDrop = false;
			this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
			this.webBrowser1.Location = new System.Drawing.Point(0, 0);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(270, 302);
			this.webBrowser1.TabIndex = 33;
			this.webBrowser1.Url = new System.Uri("http://www.penzz.com/nicpetshop.html", System.UriKind.Absolute);
			this.webBrowser1.WebBrowserShortcutsEnabled = false;
			// 
			// txtPort
			// 
			this.txtPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MakeCode.Properties.Settings.Default, "txtPort_text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtPort.Location = new System.Drawing.Point(243, 320);
			this.txtPort.MaxLength = 5;
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(55, 21);
			this.txtPort.TabIndex = 41;
			this.txtPort.Text = "3306";
			this.toolTip1.SetToolTip(this.txtPort, "\r\n如：127.0.0.1:5432");
			// 
			// labPort
			// 
			this.labPort.AutoSize = true;
			this.labPort.Location = new System.Drawing.Point(208, 326);
			this.labPort.Name = "labPort";
			this.labPort.Size = new System.Drawing.Size(29, 12);
			this.labPort.TabIndex = 40;
			this.labPort.Text = "端口";
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.NavajoWhite;
			this.ClientSize = new System.Drawing.Size(586, 405);
			this.Controls.Add(this.txtPort);
			this.Controls.Add(this.labPort);
			this.Controls.Add(this.chkDownloadRes);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.chkWebAdmin);
			this.Controls.Add(this.chkSolution);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.txtSolution);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.labServer);
			this.Controls.Add(this.labProject);
			this.Controls.Add(this.labDatabase);
			this.Controls.Add(this.labUsername);
			this.Controls.Add(this.labPassword);
			this.Controls.Add(this.cmbDatabase);
			this.Controls.Add(this.btnBuild);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.dgvGridview);
			this.ForeColor = System.Drawing.Color.Black;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FrmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "代码生成器(.NET Core + MySql)";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
			this.Load += new System.EventHandler(this.FrmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvGridview)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList imageList1;
		private CheckBox chkSolution;
		private TextBox txtServer;
		private TextBox txtSolution;
		private TextBox txtUsername;
		private TextBox txtPassword;
		private Label labServer;
		private Label labProject;
		private Label labUsername;
		private Label labPassword;
		private ComboBox cmbDatabase;
		private Button btnBuild;
		private Button btnConnect;
		private DataGridView dgvGridview;
        private ToolTip toolTip1;
        private Label labDatabase;
		private Panel panel1;
        private WebBrowser webBrowser1;
		private CheckBox chkDownloadRes;
		private CheckBox chkWebAdmin;
		private TextBox txtPort;
		private Label labPort;
	}
}