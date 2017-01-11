using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace MakeCode {
	static class Program {
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(params string[] args) {
			if (args != null && args.Length > 0) {
				ManualResetEvent wait = new ManualResetEvent(false);
				new Thread(() => {
					Thread.CurrentThread.Join(TimeSpan.FromSeconds(1));
					ConsoleApp app = new ConsoleApp(args, wait);
				}).Start();
				wait.WaitOne();
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FrmMain());
		}
	}
}