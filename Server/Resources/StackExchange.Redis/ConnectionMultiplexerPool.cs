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

		public List<RedisConnectionMultiplexer2> AllConnections = new List<RedisConnectionMultiplexer2>();
		public Queue<RedisConnectionMultiplexer2> FreeConnections = new Queue<RedisConnectionMultiplexer2>();
		public Queue<ManualResetEvent> GetConnectionQueue = new Queue<ManualResetEvent>();
		private static object _lock = new object();
		private static object _lock_GetConnectionQueue = new object();
		private string _connectionString;
		private int _poolsize;

		public ConnectionMultiplexerPool(string connectionString, int poolsize = 50) {
			_connectionString = connectionString;
			_poolsize = poolsize;
			conn = new RedisConnectionMultiplexer2 {
				Connection = ConnectionMultiplexer.Connect(_connectionString)
			};
			conn.Database = conn.Connection.GetDatabase();
		}

		RedisConnectionMultiplexer2 conn;
		public RedisConnectionMultiplexer2 GetConnection() {
			//RedisConnectionMultiplexer2 conn = null;
			//if (FreeConnections.Count > 0)
			//	lock (_lock)
			//		if (FreeConnections.Count > 0)
			//			conn = FreeConnections.Dequeue();
			//if (conn == null && AllConnections.Count < _poolsize) {
			//	lock (_lock)
			//		if (AllConnections.Count < _poolsize) {
			//			conn = new RedisConnectionMultiplexer2();
			//			AllConnections.Add(conn);
			//		}
			//	if (conn != null) {
			//		conn.Pool = this;
			//		conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
			//	}
			//}
			//if (conn == null) {
			//	ManualResetEvent wait = new ManualResetEvent(false);
			//	lock (_lock_GetConnectionQueue)
			//		GetConnectionQueue.Enqueue(wait);
			//	if (wait.WaitOne(TimeSpan.FromSeconds(10)))
			//		return GetConnection();
			//	throw new Exception("StackExchange.Redis.ConnectionMultiplexerPool.GetConnection 连接池获取超时（10秒）");
			//}
			//conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
			//conn.LastActive = DateTime.Now;
			//Interlocked.Increment(ref conn.UseSum);

			return conn;
		}

		public void ReleaseConnectionMultiplexer (RedisConnectionMultiplexer2 conn) {
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

	public class RedisConnectionMultiplexer2 : IDisposable {
		public ConnectionMultiplexer Connection;
		public IDatabase Database;
		public DateTime LastActive;
		public long UseSum;
		//internal int ThreadId;
		//internal ConnectionMultiplexerPool Pool;

		public void Dispose() {
			//if (Pool != null) Pool.ReleaseConnectionMultiplexer(this);
		}
	}
}