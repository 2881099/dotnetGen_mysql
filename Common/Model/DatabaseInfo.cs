using System;
using System.Collections.Generic;
using System.Text;

namespace Model {

	[Serializable]
	public class DatabaseInfo {

		private string _name;

		public DatabaseInfo() { }
		public DatabaseInfo(string name) {
			_name = name;
		}

		public string Name {
			get { return _name; }
		}
	}
}
