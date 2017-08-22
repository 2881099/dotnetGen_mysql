using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Data;
using Microsoft.Extensions.Logging;

namespace MySql.Data.MySqlClient {
	public partial class Executer : IDisposable {

		public ILogger Log { get; set; }
		public ConnectionPool Pool { get; }
		public Executer() { }
		public Executer(ILogger log, string connectionString) {
			this.Log = log;
			this.Pool = new ConnectionPool(connectionString);
		}

		void LoggerException(MySqlCommand cmd, Exception e) {
			if (e == null) return;
			string log = $"数据库出错（执行SQL）〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓\r\n{cmd.CommandText}\r\n";
			foreach (MySqlParameter parm in cmd.Parameters) {
				log += Lib.PadRight(parm.ParameterName, 20) + " = " + Lib.PadRight(parm.Value ?? "NULL", 20) + "\r\n";
			}
			log += e.Message;
			Log.LogError(log);

			RollbackTransaction();
			cmd.Parameters.Clear();
			throw e;
		}

		public static string Addslashes(string filter, params object[] parms) {
			if (filter == null || parms == null) return string.Empty;
			if (parms.Length == 0) return filter;
			object[] nparms = new object[parms.Length];
			for (int a = 0; a < parms.Length; a++) {
				if (parms[a] == null) nparms[a] = "NULL";
				else {
					if (parms[a] is bool || parms[a] is bool?)
						nparms[a] = (bool)parms[a] ? 1 : 0;
					else if (parms[a] is string)
						nparms[a] = string.Concat("'", parms[a].ToString().Replace("'", "''"), "'");
					else if (parms[a] is Enum)
						nparms[a] = ((Enum)parms[a]).ToInt64();
					else if (decimal.TryParse(string.Concat(parms[a]), out decimal trydec))
						nparms[a] = parms[a];
					else if (parms[a] is DateTime) {
						DateTime dt = (DateTime)parms[a];
						nparms[a] = string.Concat("'", dt.ToString("yyyy-MM-dd HH:mm:ss"), "'");
					} else if (parms[a] is DateTime?) {
						DateTime? dt = parms[a] as DateTime?;
						nparms[a] = string.Concat("'", dt.Value.ToString("yyyy-MM-dd HH:mm:ss"), "'");
					} else {
						nparms[a] = string.Concat("'", parms[a].ToString().Replace("'", "''"), "'");
						//if (parms[a] is string) nparms[a] = string.Concat('N', nparms[a]);
					}
				}
			}
			try { string ret = string.Format(filter, nparms); return ret; } catch { return filter; }
		}
		public void ExecuteReader(Action<IDataReader> readerHander, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms) {
			MySqlCommand cmd = new MySqlCommand();
			var pc = PrepareCommand(cmd, cmdType, cmdText, cmdParms);
			Exception ex = Lib.Trys(delegate () {
				if (cmd.Connection.State == ConnectionState.Closed || cmd.Connection.Ping() == false) cmd.Connection.Open();
				try {
					using (MySqlDataReader dr = cmd.ExecuteReader()) {
						while (dr.Read())
							if (readerHander != null) readerHander(dr);
					}
				} catch {
					throw;
				}
			}, 1);

			if (pc.Tran == null) this.Pool.ReleaseConnection(pc.Conn);
			LoggerException(cmd, ex);
		}
		public object[][] ExeucteArray(CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms) {
			List<object[]> ret = new List<object[]>();
			ExecuteReader(dr => {
				object[] item = new object[dr.FieldCount];
				dr.GetValues(item);
				ret.Add(item);
			}, cmdType, cmdText, cmdParms);
			return ret.ToArray();
		}
		public int ExecuteNonQuery(CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms) {
			MySqlCommand cmd = new MySqlCommand();
			var pc = PrepareCommand(cmd, cmdType, cmdText, cmdParms);
			int val = 0;
			Exception ex = Lib.Trys(delegate () {
				if (cmd.Connection.State == ConnectionState.Closed || cmd.Connection.Ping() == false) cmd.Connection.Open();
				try {
					val = cmd.ExecuteNonQuery();
				} catch {
					throw;
				}
			}, 1);

			if (pc.Tran == null) this.Pool.ReleaseConnection(pc.Conn);
			LoggerException(cmd, ex);
			cmd.Parameters.Clear();
			return val;
		}
		public object ExecuteScalar(CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms) {
			MySqlCommand cmd = new MySqlCommand();
			var pc = PrepareCommand(cmd, cmdType, cmdText, cmdParms);
			object val = null;
			Exception ex = Lib.Trys(delegate () {
				if (cmd.Connection.State == ConnectionState.Closed || cmd.Connection.Ping() == false) cmd.Connection.Open();
				try {
					val = cmd.ExecuteScalar();
				} catch {
					throw;
				}
			}, 1);

			if (pc.Tran == null) this.Pool.ReleaseConnection(pc.Conn);
			LoggerException(cmd, ex);
			cmd.Parameters.Clear();
			return val;
		}

		private PrepareCommandReturnInfo PrepareCommand(MySqlCommand cmd, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms) {
			cmd.CommandType = cmdType;
			cmd.CommandText = cmdText;

			if (cmdParms != null) {
				foreach (MySqlParameter parm in cmdParms) {
					if (parm == null) continue;
					if (parm.Value == null) parm.Value = DBNull.Value;
					cmd.Parameters.Add(parm);
				}
			}

			SqlConnection2 conn = null;
			MySqlTransaction tran = CurrentThreadTransaction;
			if (tran == null) {
				conn = this.Pool.GetConnection();
				cmd.Connection = conn.SqlConnection;
			} else {
				cmd.Connection = tran.Connection;
				cmd.Transaction = tran;
			}

			AutoCommitTransaction();
			return new PrepareCommandReturnInfo { Conn = conn, Tran = tran };
		}

		class PrepareCommandReturnInfo {
			public SqlConnection2 Conn;
			public MySqlTransaction Tran;
		}

		#region 事务处理

		class SqlTransaction2 {
			internal SqlConnection2 Conn;
			internal MySqlTransaction Transaction;
			internal DateTime RunTime;
			internal TimeSpan Timeout;

			public SqlTransaction2(SqlConnection2 conn, MySqlTransaction tran, TimeSpan timeout) {
				Conn = conn;
				Transaction = tran;
				RunTime = DateTime.Now;
				Timeout = timeout;
			}
		}

		private Dictionary<int, SqlTransaction2> _trans = new Dictionary<int, SqlTransaction2>();
		private List<SqlTransaction2> _trans_tmp = new List<SqlTransaction2>();
		private object _trans_lock = new object();

		private MySqlTransaction CurrentThreadTransaction {
			get {
				int tid = Thread.CurrentThread.ManagedThreadId;
				if (_trans.ContainsKey(tid) && _trans[tid].Transaction.Connection != null)
					return _trans[tid].Transaction;
				return null;
			}
		}

		/// <summary>
		/// 启动事务
		/// </summary>
		public void BeginTransaction() {
			BeginTransaction(TimeSpan.FromSeconds(10));
		}
		public void BeginTransaction(TimeSpan timeout) {
			int tid = Thread.CurrentThread.ManagedThreadId;
			var conn = this.Pool.GetConnection();
			SqlTransaction2 tran = null;

			Exception ex = Lib.Trys(delegate () {
				if (conn.SqlConnection.Ping() == false) conn.SqlConnection.Open();
				tran = new SqlTransaction2(conn, conn.SqlConnection.BeginTransaction(), timeout);
			}, 1);

			if (ex != null) {
				Log.LogError(new EventId(9999, "数据库出错（开启事务）"), ex, "数据库出错（开启事务）");
				throw ex;
			}

			if (_trans.ContainsKey(tid))
				CommitTransaction();

			lock (_trans_lock) {
				_trans.Add(tid, tran);
				_trans_tmp.Add(tran);
			}
		}

		/// <summary>
		/// 自动提交事务
		/// </summary>
		private void AutoCommitTransaction() {
			if (_trans_tmp.Count > 0) {
				List<SqlTransaction2> trans = null;
				lock (_trans_lock)
					trans = _trans_tmp.FindAll(st2 => DateTime.Now.Subtract(st2.RunTime) > st2.Timeout);
				foreach (SqlTransaction2 tran in trans)
					CommitTransaction(true, tran);
			}
		}
		private void CommitTransaction(bool isCommit, SqlTransaction2 tran) {
			if (tran == null || tran.Transaction == null || tran.Transaction.Connection == null) return;

			lock (_trans_lock) {
				_trans.Remove(tran.Conn.ThreadId);
				_trans_tmp.Remove(tran);
			}

			try {
				if (isCommit)
					tran.Transaction.Commit();
				else
					tran.Transaction.Rollback();
				this.Pool.ReleaseConnection(tran.Conn);
			} catch { }
		}
		private void CommitTransaction(bool isCommit) {
			int tid = Thread.CurrentThread.ManagedThreadId;

			if (_trans.ContainsKey(tid))
				CommitTransaction(isCommit, _trans[tid]);
		}
		/// <summary>
		/// 提交事务
		/// </summary>
		public void CommitTransaction() {
			CommitTransaction(true);
		}
		/// <summary>
		/// 回滚事务
		/// </summary>
		public void RollbackTransaction() {
			CommitTransaction(false);
		}

		public void Dispose() {
			SqlTransaction2[] trans = null;
			lock (_trans_lock)
				trans = _trans_tmp.ToArray();
			foreach (SqlTransaction2 tran in trans)
				CommitTransaction(false, tran);
		}
		#endregion
	}

	public static partial class ExtensionMethods {
		public static object GetEnum<T>(this IDataReader dr, int index) {
			string value = dr.GetString(index);
			Type t = typeof(T);
			foreach (var f in t.GetFields())
				if (f.GetCustomAttribute<DescriptionAttribute>()?.Description == value || f.Name == value) return Enum.Parse(t, f.Name);
			return null;
		}
	}
}

public static partial class MySql_Data_MySqlClient_ExtensionMethods {
	public static string ToDescriptionOrString(this Enum item) {
		string name = item.ToString();
		DescriptionAttribute desc = item.GetType().GetField(name)?.GetCustomAttribute<DescriptionAttribute>();
		return desc?.Description ?? name;
	}
	public static long ToInt64(this Enum item) {
		return Convert.ToInt64(item);
	}
	public static IEnumerable<T> ToSet<T>(this long value) {
		List<T> ret = new List<T>();
		if (value == 0) return ret;
		Type t = typeof(T);
		foreach (FieldInfo f in t.GetFields()) {
			if (f.FieldType != t) continue;
			object o = Enum.Parse(t, f.Name);
			long v = (long)o;
			if ((value & v) == v) ret.Add((T)o);
		}
		return ret;
	}
}