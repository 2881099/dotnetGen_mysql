using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace ServerWinService {
	[RunInstaller(true)]
	public partial class Installer1 : Installer {
		public Installer1() {
			System.ServiceProcess.ServiceInstaller si = new System.ServiceProcess.ServiceInstaller();
			System.ServiceProcess.ServiceProcessInstaller spi = new System.ServiceProcess.ServiceProcessInstaller();

			spi.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			si.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			si.ServiceName = "Microsoft MVC 10.0 (Core + MySql)";
			si.Description = "This service must be run automatically.";

			Installers.Add(spi);
			Installers.Add(si);
		}
	}
}