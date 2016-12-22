using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerSocket : IDisposable {

	private TcpListener _tcpListener;
	private Thread _tcpListenerThread;
	private Dictionary<int, AcceptSocket> _clients = new Dictionary<int, AcceptSocket>();
	private object _clients_lock = new object();
	private int _id = 1;
	private int _port;
	private bool _running;
	private ManualResetEvent _stopWait;
	public event ServerSocketAcceptedEventHandler Accepted;
	public event ServerSocketClosedEventHandler Closed;
	public event ServerSocketReceiveEventHandler Receive;
	public event ServerSocketErrorEventHandler Error;

	public ServerSocket(int port) {
		this._port = port;
	}

	public void Start() {
		if (this._running == false) {
			this._running = true;
			try {
				this._tcpListener = new TcpListener(IPAddress.Any, this._port);
				this._tcpListener.Start();
			} catch (Exception ex) {
				this._running = false;
				this.OnError(ex);
				return;
			}
			this._tcpListenerThread = new Thread(delegate() {
				while (this._running) {
					try {
						TcpClient tcpClient = this._tcpListener.AcceptTcpClient();
						new Thread(delegate() {
							try {
								AcceptSocket acceptSocket = new AcceptSocket(this, tcpClient, this._id);
								this.OnAccepted(acceptSocket);
							} catch (Exception ex) {
								this.OnError(ex);
							}
						}).Start();
					} catch (Exception ex) {
						this.OnError(ex);
					}
				}

				int[] keys = new int[this._clients.Count];
				try {
					this._clients.Keys.CopyTo(keys, 0);
				} catch {
					lock (this._clients_lock) {
						keys = new int[this._clients.Count];
						this._clients.Keys.CopyTo(keys, 0);
					}
				}
				foreach (int key in keys) {
					AcceptSocket client = null;
					if (this._clients.TryGetValue(key, out client)) {
						client.Close();
					}
				}
				this._clients.Clear();
				this._stopWait.Set();
			});
			this._tcpListenerThread.Start();
		}
	}

	public void Stop() {
		if (this._tcpListener != null) {
			this._tcpListener.Stop();
		}

		if (this._running == true) {
			this._stopWait = new ManualResetEvent(false);
			this._stopWait.Reset();
			this._running = false;
			this._stopWait.WaitOne();
		}
	}

	internal void AccessDenied(AcceptSocket client) {
		client.Write(SocketMessager.SYS_ACCESS_DENIED, delegate(object sender2, ServerSocketReceiveEventArgs e2) {
		}, TimeSpan.FromSeconds(3));
		client.Close();
	}

	public void Write(SocketMessager messager) {
		int[] keys = new int[this._clients.Count];
		try {
			this._clients.Keys.CopyTo(keys, 0);
		} catch {
			lock (this._clients_lock) {
				keys = new int[this._clients.Count];
				this._clients.Keys.CopyTo(keys, 0);
			}
		}
		foreach (int key in keys) {
			AcceptSocket client = null;
			if (this._clients.TryGetValue(key, out client)) {
				client.Write(messager);
			}
		}
	}

	public AcceptSocket GetAcceptSocket(int id) {
		AcceptSocket socket = null;
		this._clients.TryGetValue(id, out socket);
		return socket;
	}

	internal void CloseClient(AcceptSocket client) {
		this._clients.Remove(client.Id);
	}

	protected virtual void OnAccepted(ServerSocketAcceptedEventArgs e) {
		e.AcceptSocket.Write(SocketMessager.SYS_HELLO_WELCOME, delegate(object sender2, ServerSocketReceiveEventArgs e2) {
			if (e2.Messager.Id == SocketMessager.SYS_HELLO_WELCOME.Id &&
				string.Compare(e2.Messager.Action, SocketMessager.SYS_HELLO_WELCOME.Action) == 0) {
				e.AcceptSocket._accepted = true;
			}
		}, TimeSpan.FromSeconds(5));
		if (e.AcceptSocket._accepted) {
			if (this.Accepted != null) {
				try {
					this.Accepted(this, e);
				} catch (Exception ex) {
					this.OnError(ex);
				}
			}
		} else {
			e.AcceptSocket.AccessDenied();
		}
	}
	private void OnAccepted(AcceptSocket client) {
		lock (_clients_lock) {
			_clients.Add(this._id++, client);
		}
		ServerSocketAcceptedEventArgs e = new ServerSocketAcceptedEventArgs(this._clients.Count, client);
		this.OnAccepted(e);
	}

	protected virtual void OnClosed(ServerSocketClosedEventArgs e) {
		if (this.Closed != null) {
			this.Closed(this, e);
		}
	}
	internal void OnClosed(AcceptSocket client) {
		ServerSocketClosedEventArgs e = new ServerSocketClosedEventArgs(this._clients.Count, client.Id);
		this.OnClosed(e);
	}

	protected virtual void OnReceive(ServerSocketReceiveEventArgs e) {
		if (this.Receive != null) {
			this.Receive(this, e);
		}
	}
	internal void OnReceive2(ServerSocketReceiveEventArgs e) {
		this.OnReceive(e);
	}

	protected virtual void OnError(ServerSocketErrorEventArgs e) {
		if (this.Error != null) {
			this.Error(this, e);
		}
	}
	protected void OnError(Exception ex) {
		ServerSocketErrorEventArgs e = new ServerSocketErrorEventArgs(-1, ex, null);
		this.Error(this, e);
	}
	internal void OnError2(ServerSocketErrorEventArgs e) {
		this.OnError(e);
	}

	#region IDisposable 成员

	public void Dispose() {
		this.Stop();
	}

	#endregion
}

public class AcceptSocket : BaseSocket, IDisposable {

	private ServerSocket _server;
	private TcpClient _tcpClient;
	private Thread _thread;
	private bool _running;
	private int _id;
	private int _receives;
	private int _errors;
	private object _errors_lock = new object();
	private object _write_lock = new object();
	private Dictionary<int, SyncReceive> _receiveHandlers = new Dictionary<int, SyncReceive>();
	private object _receiveHandlers_lock = new object();
	private DateTime _lastActive;
	internal bool _accepted;

	public AcceptSocket(ServerSocket server, TcpClient tcpClient, int id) {
		this._running = true;
		this._id = id;
		this._server = server;
		this._tcpClient = tcpClient;
		this._lastActive = DateTime.Now;
		this._thread = new Thread(delegate() {
			while (this._running) {
				try {
					NetworkStream ns = this._tcpClient.GetStream();
					ns.ReadTimeout = 1000 * 20;
					if (ns.DataAvailable) {
						SocketMessager messager = base.Read(ns);
						if (string.Compare(messager.Action, SocketMessager.SYS_TEST_LINK.Action) != 0) {
							ServerSocketReceiveEventArgs e = new ServerSocketReceiveEventArgs(this._receives++, messager, this);
							SyncReceive receive = null;

							if (this._receiveHandlers.TryGetValue(messager.Id, out receive)) {
								new Thread(delegate() {
									try {
										receive.ReceiveHandler(this, e);
									} catch (Exception ex) {
										this.OnError(ex);
									} finally {
										receive.Wait.Set();
									}
								}).Start();
							} else {
								new Thread(delegate() {
									this.OnReceive(e);
								}).Start();
							}
						}
						this._lastActive = DateTime.Now;
					} else if (_accepted) {
						TimeSpan ts = DateTime.Now - _lastActive;
						if (ts.TotalSeconds > 5) {
							this.Write(SocketMessager.SYS_TEST_LINK);
						}
					}
					if (!ns.DataAvailable) Thread.CurrentThread.Join(100);
				} catch (Exception ex) {
					this._running = false;
					this.OnError(ex);
				}
			}
			this.Close();
			this.OnClosed();
		});
		this._thread.Start();
	}

	public void Close() {
		this._running = false;
		this._tcpClient.Close();
		this._server.CloseClient(this);
		int[] keys = new int[this._receiveHandlers.Count];
		try {
			this._receiveHandlers.Keys.CopyTo(keys, 0);
		} catch {
			lock (this._receiveHandlers_lock) {
				keys = new int[this._receiveHandlers.Count];
				this._receiveHandlers.Keys.CopyTo(keys, 0);
			}
		}
		foreach (int key in keys) {
			SyncReceive receiveHandler = null;
			if (this._receiveHandlers.TryGetValue(key, out receiveHandler)) {
				receiveHandler.Wait.Set();
			}
		}
		lock (this._receiveHandlers_lock) {
			this._receiveHandlers.Clear();
		}
	}

	public void Write(SocketMessager messager) {
		this.Write(messager, null, TimeSpan.Zero);
	}
	public void Write(SocketMessager messager, ServerSocketReceiveEventHandler receiveHandler) {
		this.Write(messager, receiveHandler, TimeSpan.FromSeconds(20));
	}
	public void Write(SocketMessager messager, ServerSocketReceiveEventHandler receiveHandler, TimeSpan timeout) {
		if (!messager._isChangeId) {
			messager.Id = -messager.Id;
		}
		SyncReceive syncReceive = null;
		try {
			if (receiveHandler != null) {
				syncReceive = new SyncReceive(receiveHandler);
				lock (this._receiveHandlers_lock) {
					if (!this._receiveHandlers.ContainsKey(messager.Id)) {
						this._receiveHandlers.Add(messager.Id, syncReceive);
					} else {
						this._receiveHandlers[messager.Id] = syncReceive;
					}
				}
			}
			lock (_write_lock) {
				NetworkStream ns = this._tcpClient.GetStream();
				base.Write(ns, messager);
			}
			this._lastActive = DateTime.Now;
			if (syncReceive != null) {
				syncReceive.Wait.Reset();
				syncReceive.Wait.WaitOne(timeout, false);
				syncReceive.Wait.Set();
				lock (this._receiveHandlers_lock) {
					this._receiveHandlers.Remove(messager.Id);
				}
			}
		} catch (Exception ex) {
			this._running = false;
			this.OnError(ex);
			if (syncReceive != null) {
				syncReceive.Wait.Set();
				lock (this._receiveHandlers_lock) {
					this._receiveHandlers.Remove(messager.Id);
				}
			}
		}
	}

	/// <summary>
	/// 拒绝访问，并关闭连接
	/// </summary>
	public void AccessDenied() {
		this._server.AccessDenied(this);
	}

	protected virtual void OnClosed() {
		try {
			this._server.OnClosed(this);
		} catch (Exception ex) {
			this.OnError(ex);
		}
	}

	protected virtual void OnReceive(ServerSocketReceiveEventArgs e) {
		try {
			this._server.OnReceive2(e);
		} catch (Exception ex) {
			this.OnError(ex);
		}
	}

	protected virtual void OnError(Exception ex) {
		int errors = 0;
		lock (this._errors_lock) {
			errors = ++this._errors;
		}
		ServerSocketErrorEventArgs e = new ServerSocketErrorEventArgs(errors, ex, this);
		this._server.OnError2(e);
	}

	public int Id {
		get { return _id; }
	}

	class SyncReceive : IDisposable {
		private ServerSocketReceiveEventHandler _receiveHandler;
		private ManualResetEvent _wait;

		public SyncReceive(ServerSocketReceiveEventHandler onReceive) {
			this._receiveHandler = onReceive;
			this._wait = new ManualResetEvent(false);
		}

		public ManualResetEvent Wait {
			get { return _wait; }
		}
		public ServerSocketReceiveEventHandler ReceiveHandler {
			get { return _receiveHandler; }
		}

		#region IDisposable 成员

		public void Dispose() {
			this._wait.Set();
			this._wait.Close();
		}

		#endregion
	}

	#region IDisposable 成员

	void IDisposable.Dispose() {
		this.Close();
	}

	#endregion
}

public delegate void ServerSocketClosedEventHandler(object sender, ServerSocketClosedEventArgs e);
public delegate void ServerSocketAcceptedEventHandler(object sender, ServerSocketAcceptedEventArgs e);
public delegate void ServerSocketErrorEventHandler(object sender, ServerSocketErrorEventArgs e);
public delegate void ServerSocketReceiveEventHandler(object sender, ServerSocketReceiveEventArgs e);

public class ServerSocketClosedEventArgs : EventArgs {

	private int _accepts;
	private int _acceptSocketId;

	public ServerSocketClosedEventArgs(int accepts, int acceptSocketId) {
		this._accepts = accepts;
		this._acceptSocketId = acceptSocketId;
	}

	public int Accepts {
		get { return _accepts; }
	}
	public int AcceptSocketId {
		get { return _acceptSocketId; }
	}
}

public class ServerSocketAcceptedEventArgs : EventArgs {

	private int _accepts;
	private AcceptSocket _acceptSocket;

	public ServerSocketAcceptedEventArgs(int accepts, AcceptSocket acceptSocket) {
		this._accepts = accepts;
		this._acceptSocket = acceptSocket;
	}

	public int Accepts {
		get { return _accepts; }
	}
	public AcceptSocket AcceptSocket {
		get { return _acceptSocket; }
	}
}

public class ServerSocketErrorEventArgs : EventArgs {

	private int _errors;
	private Exception _exception;
	private AcceptSocket _acceptSocket;

	public ServerSocketErrorEventArgs(int errors, Exception exception, AcceptSocket acceptSocket) {
		this._errors = errors;
		this._exception = exception;
		this._acceptSocket = acceptSocket;
	}

	public int Errors {
		get { return _errors; }
	}
	public Exception Exception {
		get { return _exception; }
	}
	public AcceptSocket AcceptSocket {
		get { return _acceptSocket; }
	}
}

public class ServerSocketReceiveEventArgs : EventArgs {

	private int _receives;
	private SocketMessager _messager;
	private AcceptSocket _acceptSocket;

	public ServerSocketReceiveEventArgs(int receives, SocketMessager messager, AcceptSocket acceptSocket) {
		this._receives = receives;
		this._messager = messager;
		this._acceptSocket = acceptSocket;
	}

	public int Receives {
		get { return _receives; }
	}
	public SocketMessager Messager {
		get { return _messager; }
	}
	public AcceptSocket AcceptSocket {
		get { return _acceptSocket; }
	}
}