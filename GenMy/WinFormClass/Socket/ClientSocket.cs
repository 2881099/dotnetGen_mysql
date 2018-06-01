using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientSocket : BaseSocket, IDisposable {

	private bool _isDisposed;
	private IPEndPoint _remotePoint;
	private TcpClient _tcpClient;
	private Thread _thread;
	private bool _running;
	private int _receives;
	private int _errors;
	private object _errors_lock = new object();
	private object _write_lock = new object();
	private Dictionary<int, SyncReceive> _receiveHandlers = new Dictionary<int, SyncReceive>();
	private object _receiveHandlers_lock = new object();
	private DateTime _lastActive;
	public event ClientSocketClosedEventHandler Closed;
	public event ClientSocketReceiveEventHandler Receive;
	public event ClientSocketErrorEventHandler Error;

	public void Connect(string hostname, int port) {
		if (this._isDisposed == false && this._running == false) {
			this._running = true;
			try {
				IPAddress[] ips = Dns.GetHostAddresses(hostname);
				if (ips.Length == 0) throw new Exception("无法解析“" + hostname + "”");
				this._remotePoint = new IPEndPoint(ips[0], port);
				this._tcpClient = new TcpClient();
				this._tcpClient.Connect(this._remotePoint);
			} catch (Exception ex) {
				this._running = false;
				this.OnError(ex);
				this.OnClosed();
				return;
			}
			this._receives = 0;
			this._errors = 0;
			this._lastActive = DateTime.Now;
			this._thread = new Thread(delegate() {
				while (this._running) {
					try {
						NetworkStream ns = this._tcpClient.GetStream();
						ns.ReadTimeout = 1000 * 20;
						if (ns.DataAvailable) {
							SocketMessager messager = base.Read(ns);
							if (string.Compare(messager.Action, SocketMessager.SYS_TEST_LINK.Action) == 0) {
							} else if (this._receives == 0 &&
								string.Compare(messager.Action, SocketMessager.SYS_HELLO_WELCOME.Action) == 0) {
								this._receives++;
								this.Write(messager);
							} else if (string.Compare(messager.Action, SocketMessager.SYS_ACCESS_DENIED.Action) == 0) {
								throw new Exception(SocketMessager.SYS_ACCESS_DENIED.Action);
							} else {
								ClientSocketReceiveEventArgs e = new ClientSocketReceiveEventArgs(this._receives++, messager);
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
								} else if (this.Receive != null) {
									new Thread(delegate() {
										this.OnReceive(e);
									}).Start();
								}
							}
							this._lastActive = DateTime.Now;
						} else {
							TimeSpan ts = DateTime.Now - _lastActive;
							if (ts.TotalSeconds > 3) {
								this.Write(SocketMessager.SYS_TEST_LINK);
							}
						}
						if (!ns.DataAvailable) Thread.CurrentThread.Join(1);
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
	}

	public void Close() {
		this._running = false;
		if (this._tcpClient != null) {
			this._tcpClient.Close();
		}
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
	public void Write(SocketMessager messager, ClientSocketReceiveEventHandler receiveHandler) {
		this.Write(messager, receiveHandler, TimeSpan.FromSeconds(20));
	}
	public void Write(SocketMessager messager, ClientSocketReceiveEventHandler receiveHandler, TimeSpan timeout) {
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

	protected virtual void OnClosed(EventArgs e) {
		if (this.Closed != null) {
			new Thread(delegate() {
				try {
					this.Closed(this, e);
				} catch (Exception ex) {
					this.OnError(ex);
				}
			}).Start();
		}
	}
	protected void OnClosed() {
		this.OnClosed(new EventArgs());
	}

	protected virtual void OnReceive(ClientSocketReceiveEventArgs e) {
		if (this.Receive != null) {
			try {
				this.Receive(this, e);
			} catch (Exception ex) {
				this.OnError(ex);
			}
		}
	}

	protected virtual void OnError(ClientSocketErrorEventArgs e) {
		if (this.Error != null) {
			this.Error(this, e);
		}
	}
	protected void OnError(Exception ex) {
		int errors = 0;
		lock (this._errors_lock) {
			errors = ++this._errors;
		}
		ClientSocketErrorEventArgs e = new ClientSocketErrorEventArgs(ex, errors);
		this.OnError(e);
	}

	public bool Running {
		get { return this._running; }
	}

	class SyncReceive : IDisposable {
		private ClientSocketReceiveEventHandler _receiveHandler;
		private ManualResetEvent _wait;

		public SyncReceive(ClientSocketReceiveEventHandler receiveHandler) {
			this._receiveHandler = receiveHandler;
			this._wait = new ManualResetEvent(false);
		}

		public ClientSocketReceiveEventHandler ReceiveHandler {
			get { return _receiveHandler; }
		}
		public ManualResetEvent Wait {
			get { return _wait; }
		}

		#region IDisposable 成员

		public void Dispose() {
			this._wait.Set();
			this._wait.Close();
		}

		#endregion
	}

	#region IDisposable 成员

	public void Dispose() {
		this._isDisposed = true;
		this.Close();
	}

	#endregion
}

public delegate void ClientSocketClosedEventHandler(object sender, EventArgs e);
public delegate void ClientSocketErrorEventHandler(object sender, ClientSocketErrorEventArgs e);
public delegate void ClientSocketReceiveEventHandler(object sender, ClientSocketReceiveEventArgs e);

public class ClientSocketErrorEventArgs : EventArgs {

	private int _errors;
	private Exception _exception;

	public ClientSocketErrorEventArgs(Exception exception, int errors) {
		this._exception = exception;
		this._errors = errors;
	}

	public int Errors {
		get { return _errors; }
	}
	public Exception Exception {
		get { return _exception; }
	}
}

public class ClientSocketReceiveEventArgs : EventArgs {

	private int _receives;
	private SocketMessager _messager;

	public ClientSocketReceiveEventArgs(int receives, SocketMessager messager) {
		this._receives = receives;
		this._messager = messager;
	}

	public int Receives {
		get { return _receives; }
	}
	public SocketMessager Messager {
		get { return _messager; }
	}
}