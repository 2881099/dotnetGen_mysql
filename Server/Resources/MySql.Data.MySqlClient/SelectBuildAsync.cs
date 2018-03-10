using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace MySql.Data.MySqlClient {
	partial class SelectBuild<TReturnInfo> {
		async public Task<List<TReturnInfo>> ToListAsync(Func<string, Task<string>> cache_get, Func<string, string, int, Task<bool>> cache_set, TimeSpan expire, string cacheKey = null) {
			bool isCache = expire > TimeSpan.Zero && cache_get != null && cache_set != null;
			List<TReturnInfo> ret = new List<TReturnInfo>();
			string sql = this.ToString();
			string[] objNames = new string[_dals.Count - 1];
			for (int b = 1; b < _dals.Count; b++) {
				string name = _dals[b].GetType().Name;
				objNames[b - 1] = string.Concat("Obj_", name[0].ToString().ToLower(), name.Substring(1));
			}
			if (isCache) {
				if (string.IsNullOrEmpty(cacheKey)) cacheKey = sql.Substring(sql.IndexOf(" \r\nFROM ") + 8);
				MethodInfo[] parses = new MethodInfo[_dals.Count];
				for (int b = 0; b < _dals.Count; b++) {
					string modelTypeName = string.Concat(_dals[b].GetType().FullName.Replace(".DAL.", ".Model."), "Info");
					parses[b] = this.GetType().GetTypeInfo().Assembly.GetType(modelTypeName).GetMethod("Parse", new Type[] { typeof(string) });
				}
				string cacheValue = await cache_get(cacheKey);
				if (!string.IsNullOrEmpty(cacheValue)) {
					try {
						string[] vs = JsonConvert.DeserializeObject<string[]>(cacheValue);
						for (int a = 0, skip = objNames.Length + 1; a < vs.Length; a += skip) {
							TReturnInfo info = (TReturnInfo) parses[0].Invoke(null, new object[] { vs[a] });
							Type type = info.GetType();
							for (int b = 1; b < parses.Length; b++) {
								object item = parses[b].Invoke(null, new object[] { vs[a + b] });
								PropertyInfo prop = type.GetProperty(objNames[b - 1]);
								if (prop != null) prop.SetValue(info, item, null);
							}
							ret.Add(info);
						}
						return ret;
					} catch {
						// 转换列表的时候出错
					}
					ret.Clear();
				}
			}
			List<object> cacheList = null;
			if (isCache) cacheList = new List<object>();
			await _exec.ExecuteReaderAsync(async dr => {
				int dataIndex = -1;
				var read = await _dals[0].GetItemAsync(dr, dataIndex);
				TReturnInfo info = (TReturnInfo) read.result;
				dataIndex = read.dataIndex;
				Type type = info.GetType();
				ret.Add(info);
				if (isCache) cacheList.Add(type.GetMethod("Stringify").Invoke(info, null));
				for (int b = 0; b < objNames.Length; b++) {
					var read2 = await _dals[b + 1].GetItemAsync(dr, dataIndex);
					object obj = read2.result;
					dataIndex = read2.dataIndex;
					PropertyInfo prop = type.GetProperty(objNames[b]);
					if (prop == null) throw new Exception(string.Concat(type.FullName, " 没有定义属性 ", objNames[b]));
					prop.SetValue(info, obj, null);
					if (isCache) cacheList.Add(obj.GetType().GetMethod("Stringify").Invoke(obj, null));
				}
			}, CommandType.Text, sql);
			if (isCache) {
				string json = JsonConvert.SerializeObject(cacheList);
				await cache_set(cacheKey, json, (int) expire.TotalSeconds);
			}
			return ret;
		}
		async public Task<List<TReturnInfo>> ToListAsync() {
			return await this.ToListAsync(null, null, TimeSpan.Zero);
		}
		async public Task<TReturnInfo> ToOneAsync() {
			List<TReturnInfo> ret = await this.Limit(1).ToListAsync();
			return ret.Count > 0 ? ret[0] : default(TReturnInfo);
		}
		/// <summary>
		/// 查询指定字段，返回元组或单值
		/// </summary>
		/// <typeparam name="T">元组或单值，如：.Aggregate&lt;(int id, string name)&gt;("id,title")，或 .Aggregate&lt;int&gt;("id")</typeparam>
		/// <param name="field">返回的字段，用逗号分隔，如：id,name</param>
		/// <returns></returns>
		async public Task<List<T>> AggregateAsync<T>(string fields) {
			string limit = _skip > 0 || _limit > 0 ? string.Format(" \r\nlimit {0},{1}", Math.Max(0, _skip), _limit > 0 ? _limit : -1) : string.Empty;
			string where = string.IsNullOrEmpty(_where) ? string.Empty : string.Concat(" \r\nWHERE ", _where.Substring(5));
			string having = string.IsNullOrEmpty(_groupby) ||
							string.IsNullOrEmpty(_having) ? string.Empty : string.Concat(" \r\nHAVING ", _having.Substring(5));
			string sql = string.Concat("SELECT ", fields, _table, _join, where, _groupby, having, _sort, limit);

			List<T> ret = new List<T>();
			Type type = typeof(T);

			await _exec.ExecuteReaderAsync(async dr => {
				int dataIndex = -1;
				var read = await this.AggregateReadTupleAsync(type, dr, dataIndex);
				ret.Add((T) read.result);
				dataIndex = read.dataIndex;
			}, CommandType.Text, sql);
			return ret;
		}
		async public Task<T> AggregateScalarAsync<T>(string field, params object[] parms) {
			var items = await this.AggregateAsync<T>(field);
			return items.Count > 0 ? items[0] : default(T);
		}
		async private Task<(object result, int dataIndex)> AggregateReadTupleAsync(Type type, MySqlDataReader dr, int dataIndex) {
			bool isTuple = type.Namespace == "System" && type.Name.StartsWith("ValueTuple`");
			if (isTuple) {
				FieldInfo[] fs = type.GetFields();
				Type[] types = new Type[fs.Length];
				object[] parms = new object[fs.Length];
				for (int a = 0; a < fs.Length; a++) {
					types[a] = fs[a].FieldType;
					var read = await this.AggregateReadTupleAsync(types[a], dr, dataIndex);
					parms[a] = read.result;
					dataIndex = read.dataIndex;
				}
				ConstructorInfo constructor = type.GetConstructor(types);
				return (constructor.Invoke(parms), dataIndex);
			}
			return (await dr.IsDBNullAsync(++dataIndex) ? null : await dr.GetFieldValueAsync<object>(dataIndex), dataIndex);
		}
		async public Task<long> CountAsync() {
			return await this.AggregateScalarAsync<long>("count(1)");
		}
	}
}