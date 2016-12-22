using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace StackExchange.Redis {
	public partial class QuickHelperBase {
		public static ConnectionMultiplexerPool Instance { get; protected set; }
		public static void Set(string key, string value, int expireSeconds = -1) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				if (expireSeconds > 0)
					conn.Database.StringSet(key, value, TimeSpan.FromSeconds(expireSeconds));
				else
					conn.Database.StringSet(key, value);
			}
		}
		public static string Get(string key) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.StringGet(key);
			}
		}
		public static void Remove(params string[] key) {
			if (key == null || key.Length == 0) return;
			using (var conn = Instance.GetConnection()) {
				RedisKey[] rkeys = new RedisKey[key.Length];
				for (int a = 0; a < key.Length; a++) rkeys[a] = string.Concat(conn.Connection.ClientName, key[a]);
				conn.Database.KeyDelete(rkeys);
			}
		}
		public static bool Exists(string key) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.KeyExists(key);
			}
		}
		public static double Increment(string key, double value = 1) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.StringIncrement(key, value);
			}
		}
		public static void KeyExpire(string key, TimeSpan expire) {
			if (expire <= TimeSpan.Zero) return;
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				conn.Database.KeyExpire(key, expire);
			}
		}
		#region Hash 操作
		public static void HashSet(string key, HashEntry[] fields, TimeSpan expire) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				conn.Database.HashSet(key, fields);
				if (expire > TimeSpan.Zero) conn.Database.KeyExpire(key, expire);
			}
		}
		public static RedisValue HashGet(string key, string field) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashGet(key, field);
			}
		}
		public static double HashIncrement(string key, string field, double value = 1) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashIncrement(key, field, value);
			}
		}
		public static bool HashDelete(string key, string field) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashDelete(key, field);
			}
		}
		public static bool HashExists(string key, string field) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashExists(key, field);
			}
		}
		public static long HashLength(string key) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashLength(key);
			}
		}
		public static HashEntry[] HashGetAll(string key) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashGetAll(key);
			}
		}
		public static RedisValue[] HashKeys(string key) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashKeys(key);
			}
		}
		public static RedisValue[] HashGet(string key) {
			using (var conn = Instance.GetConnection()) {
				key = string.Concat(conn.Connection.ClientName, key);
				return conn.Database.HashValues(key);
			}
		}
		#endregion
	}
}