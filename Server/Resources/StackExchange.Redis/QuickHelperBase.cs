using System;
using System.Collections.Generic;
using System.Linq;

namespace StackExchange.Redis {
	public partial class QuickHelperBase {
		protected static string Name { get; set; }
		public static ConnectionMultiplexerPool Instance { get; protected set; }
		public static bool Set(string key, string value, int expireSeconds = -1) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				if (expireSeconds > 0)
					return conn.Database.StringSet(key, value, TimeSpan.FromSeconds(expireSeconds));
				else
					return conn.Database.StringSet(key, value);
			}
		}
		public static string Get(string key) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.StringGet(key);
			}
		}
		public static long Remove(params string[] key) {
			if (key == null || key.Length == 0) return 0;
			RedisKey[] rkeys = new RedisKey[key.Length];
			for (int a = 0; a < key.Length; a++) rkeys[a] = string.Concat(Name, key[a]);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.KeyDelete(rkeys);
			}
		}
		public static bool Exists(string key) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.KeyExists(key);
			}
		}
		public static long Increment(string key, long value = 1) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.StringIncrement(key, value);
			}
		}
		public static bool Expire(string key, TimeSpan expire) {
			if (expire <= TimeSpan.Zero) return false;
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.KeyExpire(key, expire);
			}
		}
		public static long Publish(string channel, string data) {
			using (var conn = Instance.GetConnection()) {
				return conn.Database.Publish(channel, data);
			}
		}
		#region Hash 操作
		public static void HashSet(string key, params object[] keyValues) {
			HashSet(key, TimeSpan.Zero, keyValues);
		}
		public static void HashSetExpire(string key, TimeSpan expire, params object[] keyValues) {
			key = string.Concat(Name, key);
			HashEntry[] hvs = new HashEntry[keyValues.Length / 2];
			for (var a = 0; a < hvs.Length; a++) hvs[a] = new HashEntry(string.Concat(keyValues[a * 2]), string.Concat(keyValues[a * 2 + 1]));
			using (var conn = Instance.GetConnection()) {
				conn.Database.HashSet(key, hvs);
				if (expire > TimeSpan.Zero) conn.Database.KeyExpire(key, expire);
			}
		}
		public static string HashGet(string key, string field) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.HashGet(key, field);
			}
		}
		public static long HashIncrement(string key, string field, long value = 1) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.HashIncrement(key, field, value);
			}
		}
		public static long HashDelete(string key, params string[] fields) {
			if (fields == null || fields.Length == 0) return 0;
			key = string.Concat(Name, key);
			long ret = 0;
			using (var conn = Instance.GetConnection()) {
				foreach (var field in fields) ret += conn.Database.HashDelete(key, field) ? 1 : 0;
			}
			return ret;
		}
		public static bool HashExists(string key, string field) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.HashExists(key, field);
			}
		}
		public static long HashLength(string key) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.HashLength(key);
			}
		}
		public static Dictionary<string, string> HashGetAll(string key) {
			key = string.Concat(Name, key);
			HashEntry[] vs;
			using (var conn = Instance.GetConnection()) {
				vs = conn.Database.HashGetAll(key);
			}
			Dictionary<string, string> ret = new Dictionary<string, string>();
			foreach (var v in vs) ret.Add(v.Name, v.Value);
			return ret;
		}
		public static string[] HashKeys(string key) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.HashKeys(key).Select(a => a.IsNull ? null : a.ToString()).ToArray();
			}
		}
		public static string[] HashVals(string key) {
			key = string.Concat(Name, key);
			using (var conn = Instance.GetConnection()) {
				return conn.Database.HashValues(key).Select(a => a.IsNull ? null : a.ToString()).ToArray();
			}
		}
		#endregion
	}
}