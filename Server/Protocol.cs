using System;
using System.Collections.Generic;
using System.Text;
using Model;

namespace Server {

	public class Protocol : IDisposable {
		internal ServerSocket _socket;
		private Dictionary<int, CodeBuild> _builds;
		private object _builds_lock = new object();

		private Protocol(int port) {
			_builds = new Dictionary<int, CodeBuild>();
			_socket = new ServerSocket(port);
			_socket.Closed += this.OnClosed;
			_socket.Accepted += this.OnAccepted;
			_socket.Error += this.OnError;
			_socket.Receive += this.OnReceive;
			_socket.Start();
		}

		protected virtual void OnClosed(object sender, ServerSocketClosedEventArgs e) {
			_builds.Remove(e.AcceptSocketId);
		}
		protected virtual void OnReceive(object sender, ServerSocketReceiveEventArgs e) {
			switch (e.Messager.Action) {
				case "GetDatabases":
					ClientInfo ci = e.Messager.Arg as ClientInfo;
					if (ci == null) {
						e.AcceptSocket.AccessDenied();
					} else {
						CodeBuild build = new CodeBuild(ci, e.AcceptSocket);
						lock (_builds_lock) {
							_builds.Remove(e.AcceptSocket.Id);
							_builds.Add(e.AcceptSocket.Id, build);
						}
						List<DatabaseInfo> dbs = build.GetDatabases();
						SocketMessager messager = new SocketMessager(e.Messager.Action, dbs);
						messager.Id = e.Messager.Id;
						e.AcceptSocket.Write(messager);
					}
					break;
				case "GetTablesByDatabase":
					string database = string.Concat(e.Messager.Arg);
					if (string.IsNullOrEmpty(database)) {
						e.AcceptSocket.AccessDenied();
					} else {
						CodeBuild build = null;
						if (!_builds.TryGetValue(e.AcceptSocket.Id, out build)) {
							e.AcceptSocket.AccessDenied();
						} else {
							List<TableInfo> tables = build.GetTablesByDatabase(database);
							SocketMessager messager = new SocketMessager(e.Messager.Action, tables);
							messager.Id = e.Messager.Id;
							e.AcceptSocket.Write(messager);
						}
					}
					break;
				case "Build":
					object[] parms = e.Messager.Arg as object[];
					if (parms.Length < 4) {
						e.AcceptSocket.AccessDenied();
					} else {
						string solutionName = string.Concat(parms[0]);
						bool isSolution, isMakeAdmin, isDownloadRes;
						string op10 = string.Concat(parms[2]);
						if (string.IsNullOrEmpty(solutionName) ||
							!bool.TryParse(string.Concat(parms[1]), out isSolution) ||
							string.IsNullOrEmpty(op10)) {
							e.AcceptSocket.AccessDenied();
						} else {
							isMakeAdmin = false;
							isDownloadRes = false;
							if (parms.Length >= 4) bool.TryParse(string.Concat(parms[3]), out isMakeAdmin);
							if (parms.Length >= 5) bool.TryParse(string.Concat(parms[4]), out isDownloadRes);
							CodeBuild build = null;
							if (!_builds.TryGetValue(e.AcceptSocket.Id, out build)) {
								e.AcceptSocket.AccessDenied();
							} else {
								List<bool> outputs = new List<bool>();
								char[] cs = op10.ToCharArray();
								foreach (char c in cs) {
									outputs.Add(c == '1');
								}
								build.SetOutput(outputs.ToArray());
								object parm = null;
								try {
									parm = build.Build(solutionName, isSolution, isMakeAdmin, isDownloadRes);
								} catch (Exception ex) {
									parm = ex;
								}
								SocketMessager messager = new SocketMessager(e.Messager.Action, parm);
								messager.Id = e.Messager.Id;
								e.AcceptSocket.Write(messager);
							}
						}
					}
					break;
				default:
					e.AcceptSocket.AccessDenied();
					break;
			}
		}
		protected virtual void OnAccepted(object sender, ServerSocketAcceptedEventArgs e) {
		}
		protected virtual void OnError(object sender, ServerSocketErrorEventArgs e) {
			Logger.remotor.Debug("Errors: " + e.Errors, e.Exception);
		}

		public static Protocol Create(int port) {
			return new Protocol(port);
		}

		#region IDisposable ≥…‘±

		public void Dispose() {
			if (_socket != null) {
				_socket.Dispose();
			}
		}

		#endregion
	}
}
