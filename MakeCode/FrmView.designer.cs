namespace MakeCode {
	partial class FrmView {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.btnOk = new System.Windows.Forms.Button();
			this.dgvGridview = new System.Windows.Forms.DataGridView();
			this.dgvColIcon = new System.Windows.Forms.DataGridViewImageColumn();
			this.dgvColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvColDBType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvColAllowDBNull = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.dgvColView = new System.Windows.Forms.DataGridViewLinkColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvGridview)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOk.Location = new System.Drawing.Point(262, 372);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(62, 21);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// dgvGridview
			// 
			this.dgvGridview.AllowUserToAddRows = false;
			this.dgvGridview.AllowUserToResizeRows = false;
			this.dgvGridview.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.dgvGridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvGridview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvColIcon,
            this.dgvColName,
            this.dgvColDBType,
            this.dgvColAllowDBNull,
            this.dgvColView});
			this.dgvGridview.Location = new System.Drawing.Point(12, 12);
			this.dgvGridview.Name = "dgvGridview";
			this.dgvGridview.ReadOnly = true;
			this.dgvGridview.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgvGridview.RowTemplate.Height = 23;
			this.dgvGridview.Size = new System.Drawing.Size(562, 352);
			this.dgvGridview.TabIndex = 0;
			// 
			// dgvColIcon
			// 
			this.dgvColIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.NullValue = null;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
			this.dgvColIcon.DefaultCellStyle = dataGridViewCellStyle1;
			this.dgvColIcon.HeaderText = "  ";
			this.dgvColIcon.Name = "dgvColIcon";
			this.dgvColIcon.ReadOnly = true;
			this.dgvColIcon.Width = 21;
			// 
			// dgvColName
			// 
			this.dgvColName.HeaderText = "Name";
			this.dgvColName.Name = "dgvColName";
			this.dgvColName.ReadOnly = true;
			this.dgvColName.Width = 210;
			// 
			// dgvColDBType
			// 
			this.dgvColDBType.HeaderText = "SqlType";
			this.dgvColDBType.Name = "dgvColDBType";
			this.dgvColDBType.ReadOnly = true;
			this.dgvColDBType.Width = 130;
			// 
			// dgvColAllowDBNull
			// 
			this.dgvColAllowDBNull.HeaderText = "AllowDBNull";
			this.dgvColAllowDBNull.Name = "dgvColAllowDBNull";
			this.dgvColAllowDBNull.ReadOnly = true;
			this.dgvColAllowDBNull.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvColAllowDBNull.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.dgvColAllowDBNull.Width = 80;
			// 
			// dgvColView
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
			this.dgvColView.DefaultCellStyle = dataGridViewCellStyle2;
			this.dgvColView.HeaderText = "Relation";
			this.dgvColView.Name = "dgvColView";
			this.dgvColView.ReadOnly = true;
			this.dgvColView.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvColView.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.dgvColView.Text = "View";
			this.dgvColView.Width = 60;
			// 
			// FrmView
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOk;
			this.ClientSize = new System.Drawing.Size(586, 405);
			this.Controls.Add(this.dgvGridview);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FrmView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "靓仔的 c# 代码生成器 (http://www.kellynic.com/)";
			((System.ComponentModel.ISupportInitialize)(this.dgvGridview)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		public System.Windows.Forms.DataGridView dgvGridview;
		private System.Windows.Forms.DataGridViewImageColumn dgvColIcon;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvColName;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvColDBType;
		private System.Windows.Forms.DataGridViewCheckBoxColumn dgvColAllowDBNull;
		private System.Windows.Forms.DataGridViewLinkColumn dgvColView;
	}
}