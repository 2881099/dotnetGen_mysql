using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.Remoting;
using System.ServiceProcess;
using System.Text;

namespace ServerWinService {
	public partial class Service1 : ServiceBase {
		public Service1() {
			InitializeComponent();
		}

		Server.Protocol _prol;
		protected override void OnStart(string[] args) {
			Deflate.cs_head = Settings.Default.cs_head;
			_prol = Server.Protocol.Create(Settings.Default.socket_port);
		}

		protected override void OnStop() {
			_prol.Dispose();
		}
	}
}
