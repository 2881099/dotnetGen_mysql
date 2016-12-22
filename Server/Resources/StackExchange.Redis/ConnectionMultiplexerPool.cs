using System;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace StackExchange.Redis {
	/// <summary>
	/// ConnectionMultiplexer链接池
	/// </summary>
	public partial class ConnectionMultiplexerPool {

		public int MaxPoolSize = 32;
		public List<ConnectionMultiplexer2> AllConnections = new List<ConnectionMultiplexer2>();
		public Queue<ConnectionMultiplexer2> FreeConnections = new Queue<ConnectionMultiplexer2>();
		public Queue<ManualResetEvent> GetConnectionQueue = new Queue<ManualResetEvent>();
		private static object _lock = new object();
		private static object _lock_GetConnectionQueue = new object();
		private string _connectionString;
		public string ConnectionString {
			get { return _connectionString; }
			set {
				_connectionString = value;
				Match m = Regex.Match(_connectionString, @"Max\s*pool\s*size=(\d+)", RegexOptions.IgnoreCase);
				if (m.Success) int.TryParse(m.Groups[1].Value, out MaxPoolSize);
				else MaxPoolSize = 32;
				if (MaxPoolSize <= 0) MaxPoolSize = 32;
			}
		}

		public ConnectionMultiplexerPool(string connectionString) {
			ConnectionString = connectionString;
		}

		public ConnectionMultiplexer2 GetConnection() {
			ConnectionMultiplexer2 conn = null;
			if (FreeConnections.Count > 0)
				lock (_lock)
					if (FreeConnections.Count > 0)
						conn = FreeConnections.Dequeue();
			if (conn == null && AllConnections.Count < MaxPoolSize) {
				lock (_lock)
					if (AllConnections.Count < MaxPoolSize) {
						conn = new ConnectionMultiplexer2();
						AllConnections.Add(conn);
					}
				if (conn != null) {
					conn.Pool = this;
					conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
				}
			}
			if (conn == null) {
				ManualResetEvent wait = new ManualResetEvent(false);
				lock (_lock_GetConnectionQueue)
					GetConnectionQueue.Enqueue(wait);
				if (wait.WaitOne(TimeSpan.FromSeconds(10)))
					return GetConnection();
				throw new Exception("GetConnection 连接池获取超时10秒");
			}
			conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
			conn.LastActive = DateTime.Now;
			Interlocked.Increment(ref conn.UseSum);
			try {
				if (conn.Database == null || !conn.Database.IsConnected("test")) {
					conn.Connection = ConnectionMultiplexer.Connect(ConnectionString);
					conn.Database = conn.Connection.GetDatabase();
				}
			} catch(Exception ex) {
				throw new Exception("GetConnection/GetDatabase 出错", ex);
			}
			return conn;
		}

		public void ReleaseConnection(ConnectionMultiplexer2 conn) {
			lock (_lock)
				FreeConnections.Enqueue(conn);

			if (GetConnectionQueue.Count > 0) {
				ManualResetEvent wait = null;
				lock (_lock_GetConnectionQueue)
					if (GetConnectionQueue.Count > 0)
						wait = GetConnectionQueue.Dequeue();
				if (wait != null) wait.Set();
			}
		}
	}

	public class ConnectionMultiplexer2 : IDisposable {
		public ConnectionMultiplexer Connection;
		public IDatabase Database;
		public DateTime LastActive;
		public long UseSum;
		internal int ThreadId;
		internal ConnectionMultiplexerPool Pool;

		public void Dispose() {
			Pool.ReleaseConnection(this);
		}
	}
}