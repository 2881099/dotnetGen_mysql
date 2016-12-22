using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MakeCode {
	public partial class FrmView : Form {
		public FrmView() {
			InitializeComponent();
		}

		private void btnOk_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}
