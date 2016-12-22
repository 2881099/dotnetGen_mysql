using System;
using System.Data;
using System.Windows.Forms;

namespace MakeCode {
	public static class Lib {

		public static void Msgbox(string text) {
			Msgbox(text, MessageBoxIcon.Information);
		}
		public static void Msgbox(string text, MessageBoxIcon icon) {
			MessageBox.Show(text, Application.ProductName + " " + Application.ProductVersion, MessageBoxButtons.OK, icon);
		}
	}
}
