using System;
using System.Collections.Generic;
using System.Text;

namespace Model {

	[Serializable]
	public class ClientInfo {
		private string _server;
		private int _port;
		private string _username;
		private string _password;
		private string _database;

		public ClientInfo(string server, int port, string username, string password) {
			_server = server;
			_port = port;
			_username = username;
			_password = password;
		}

		public string Server {
			get { return _server; }
		}
		public int Port {
			get { return _port; }
		}
		public string Username {
			get { return _username; }
		}
		public string Password {
			get { return _password; }
		}
		public string Database {
			get { return _database; }
			set { _database = value; }
		}
	}
}
