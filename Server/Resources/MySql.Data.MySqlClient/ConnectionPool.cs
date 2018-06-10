using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MySql.Data.MySqlClient {
	/// <summary>
	/// 数据库链接池
	/// </summary>
	public partial class ConnectionPool {

		public int MaxPoolSize = 32;
		public List<SqlConnection2> AllConnections = new List<SqlConnection2>();
		public Queue<SqlConnection2> FreeConnections = new Queue<SqlConnection2>();
		public Queue<ManualResetEventSlim> GetConnectionQueue = new Queue<ManualResetEventSlim>();
		public Queue<TaskCompletionSource<SqlConnection2>> GetConnectionAsyncQueue = new Queue<TaskCompletionSource<SqlConnection2>>();
		private static object _lock = new object();
		private static object _lock_GetConnectionQueue = new object();
		private string _connectionString;
		public string ConnectionString {
			get { return _connectionString; }
			set {
				_connectionString = value;
				if (string.IsNullOrEmpty(_connectionString)) return;
				Match m = Regex.Match(_connectionString, @"Max\s*pool\s*size\s*=\s*(\d+)", RegexOptions.IgnoreCase);
				if (m.Success) int.TryParse(m.Groups[1].Value, out MaxPoolSize);
				else MaxPoolSize = 32;
				if (MaxPoolSize <= 0) MaxPoolSize = 32;
				var initConns = new SqlConnection2[MaxPoolSize];
				for (var a = 0; a < MaxPoolSize; a++) initConns[a] = GetFreeConnection();
				foreach (var conn in initConns) ReleaseConnection(conn);
			}
		}

		public ConnectionPool(string connectionString) {
			ConnectionString = connectionString;
		}

		public SqlConnection2 GetFreeConnection() {
			SqlConnection2 conn = null;
			if (FreeConnections.Count > 0)
				lock (_lock)
					if (FreeConnections.Count > 0)
						conn = FreeConnections.Dequeue();
			if (conn == null && AllConnections.Count < MaxPoolSize) {
				lock (_lock)
					if (AllConnections.Count < MaxPoolSize) {
						conn = new SqlConnection2();
						AllConnections.Add(conn);
					}
				if (conn != null) {
					conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
					conn.SqlConnection = new MySqlConnection(ConnectionString);
				}
			}
			return conn;
		}
		public SqlConnection2 GetConnection() {
			var conn = GetFreeConnection();
			if (conn == null) {
				ManualResetEventSlim wait = new ManualResetEventSlim(false);
				lock (_lock_GetConnectionQueue)
					GetConnectionQueue.Enqueue(wait);
				if (wait.Wait(TimeSpan.FromSeconds(10)))
					return GetConnection();
				throw new Exception("MySql.Data.MySqlClient.ConnectionPool.GetConnection 连接池获取超时（10秒）");
			}
			conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
			conn.LastActive = DateTime.Now;
			Interlocked.Increment(ref conn.UseSum);
			return conn;
		}

		async public Task<SqlConnection2> GetConnectionAsync() {
			var conn = GetFreeConnection();
			if (conn == null) {
				TaskCompletionSource<SqlConnection2> tcs = new TaskCompletionSource<SqlConnection2>();
				lock (_lock_GetConnectionQueue)
					GetConnectionAsyncQueue.Enqueue(tcs);
				return await tcs.Task;
			}
			conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
			conn.LastActive = DateTime.Now;
			Interlocked.Increment(ref conn.UseSum);
			return conn;
		}

		public void ReleaseConnection(SqlConnection2 conn) {
			//try { conn.SqlConnection.Close(); } catch { }
			lock (_lock)
				FreeConnections.Enqueue(conn);

			bool isAsync = false;
			if (GetConnectionAsyncQueue.Count > 0) {
				TaskCompletionSource<SqlConnection2> tcs = null;
				lock (_lock_GetConnectionQueue)
					if (GetConnectionAsyncQueue.Count > 0)
						tcs = GetConnectionAsyncQueue.Dequeue();
				if (isAsync = (tcs != null)) tcs.TrySetResult(GetConnectionAsync().Result);
			}
			if (isAsync == false && GetConnectionQueue.Count > 0) {
				ManualResetEventSlim wait = null;
				lock (_lock_GetConnectionQueue)
					if (GetConnectionQueue.Count > 0)
						wait = GetConnectionQueue.Dequeue();
				if (wait != null) wait.Set();
			}
		}
	}

	public class SqlConnection2 {
		public MySqlConnection SqlConnection;
		public DateTime LastActive;
		public long UseSum;
		internal int ThreadId;
	}
}