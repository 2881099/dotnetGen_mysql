using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data;
using Newtonsoft.Json;

namespace MySql.Data.MySqlClient {
	public partial interface IDAL {
		string Table { get; }
		string Field { get; }
		string Sort { get; }
		object GetItem(IDataReader dr, ref int index);
	}
	public class SelectBuild<TReturnInfo, TLinket> : SelectBuild<TReturnInfo> where TLinket : SelectBuild<TReturnInfo> {
		protected SelectBuild<TReturnInfo> Where1Or(string filterFormat, Array values) {
			if (values == null) values = new object[] { null };
			if (values.Length == 0) return this;
			if (values.Length == 1) return base.Where(filterFormat, values.GetValue(0));
			string filter = string.Empty;
			for (int a = 0; a < values.Length; a++) filter = string.Concat(filter, " OR ", string.Format(filterFormat, "{" + a + "}"));
			object[] parms = new object[values.Length];
			values.CopyTo(parms, 0);
			return base.Where(filter.Substring(4), parms);
		}
		public new TLinket Count(out int count) {
			return base.Count(out count) as TLinket;
		}
		public new TLinket Where(string filter, params object[] parms) {
			return base.Where(true, filter, parms) as TLinket;
		}
		public new TLinket Where(bool isadd, string filter, params object[] parms) {
			return base.Where(isadd, filter, parms) as TLinket;
		}
		public new TLinket GroupBy(string groupby) {
			return base.GroupBy(groupby) as TLinket;
		}
		public new TLinket Having(string filter, params object[] parms) {
			return base.Having(true, filter, parms) as TLinket;
		}
		public new TLinket Having(bool isadd, string filter, params object[] parms) {
			return base.Having(isadd, filter, parms) as TLinket;
		}
		public new TLinket Sort(string sort) {
			return base.Sort(sort) as TLinket;
		}
		public new TLinket OrderBy(string sort) {
			return base.Sort(sort) as TLinket;
		}
		public new TLinket From<TBLL>() {
			return base.From<TBLL>() as TLinket;
		}
		public new TLinket From<TBLL>(string alias) {
			return base.From<TBLL>(alias) as TLinket;
		}
		public new TLinket InnerJoin<TBLL>(string alias, string on) {
			return base.InnerJoin<TBLL>(alias, on) as TLinket;
		}
		public new TLinket LeftJoin<TBLL>(string alias, string on) {
			return base.LeftJoin<TBLL>(alias, on) as TLinket;
		}
		public new TLinket RightJoin<TBLL>(string alias, string on) {
			return base.RightJoin<TBLL>(alias, on) as TLinket;
		}
		public new TLinket Skip(int skip) {
			return base.Skip(skip) as TLinket;
		}
		public new TLinket Limit(int limit) {
			return base.Limit(limit) as TLinket;
		}
		public SelectBuild(IDAL dal, Executer exec) : base(dal, exec) { }
	}
	public class SelectBuild<TReturnInfo> {
		protected int _limit, _skip;
		protected string _sort, _field, _table, _join, _where, _groupby, _having;
		protected List<IDAL> _dals = new List<IDAL>();
		protected Executer _exec;
		public List<TReturnInfo> ToList(Func<string, string> cache_get, Func<string, string, int, string> cache_set, TimeSpan expire, string cacheKey = null) {
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
				string cacheValue = cache_get(cacheKey);
				if (!string.IsNullOrEmpty(cacheValue)) {
					try {
						string[] vs = JsonConvert.DeserializeObject<string[]>(cacheValue);
						for (int a = 0, skip = objNames.Length + 1; a < vs.Length; a += skip) {
							TReturnInfo info = (TReturnInfo)parses[0].Invoke(null, new object[] { vs[a] });
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
			_exec.ExecuteReader(dr => {
				int index = -1;
				TReturnInfo info = (TReturnInfo)_dals[0].GetItem(dr, ref index);
				Type type = info.GetType();
				ret.Add(info);
				if (isCache) cacheList.Add(info.GetType().GetMethod("Stringify").Invoke(info, null));
				for (int b = 0; b < objNames.Length; b++) {
					object obj = _dals[b + 1].GetItem(dr, ref index);
					PropertyInfo prop = info.GetType().GetProperty(objNames[b]);
					if (prop == null) throw new Exception(string.Concat(type.FullName, " 没有定义属性 ", objNames[b]));
					prop.SetValue(info, obj, null);
					if (isCache) cacheList.Add(obj.GetType().GetMethod("Stringify").Invoke(obj, null));
				}
			}, CommandType.Text, this.ToString());
			if (isCache) {
				string json = JsonConvert.SerializeObject(cacheList);
				cache_set(cacheKey, json, (int)expire.TotalSeconds);
			}
			return ret;
		}
		public List<TReturnInfo> ToList() {
			return this.ToList(null, null, TimeSpan.Zero);
		}
		public TReturnInfo ToOne() {
			List<TReturnInfo> ret = this.Limit(1).ToList();
			return ret.Count > 0 ? ret[0] : default(TReturnInfo);
		}
		public override string ToString() {
			if (string.IsNullOrEmpty(_sort) && _skip > 0) this.Sort(_dals[0].Sort);
			string limit = _skip > 0 || _limit > 0 ? string.Format(" \r\nlimit {0},{1}", Math.Max(0, _skip), _limit > 0 ? _limit : -1) : string.Empty;
			string where = string.IsNullOrEmpty(_where) ? string.Empty : string.Concat(" \r\nWHERE ", _where.Substring(5));
			string sql = string.Concat("SELECT ", _field, _table, _join, where, _sort, limit);
			return sql;
		}
		public object[][] Aggregate(string fields) {
			string limit = _skip > 0 || _limit > 0 ? string.Format(" \r\nlimit {0},{1}", Math.Max(0, _skip), _limit > 0 ? _limit : -1) : string.Empty;
			string where = string.IsNullOrEmpty(_where) ? string.Empty : string.Concat(" \r\nWHERE ", _where.Substring(5));
			string having = string.IsNullOrEmpty(_groupby) ||
							string.IsNullOrEmpty(_having) ? string.Empty : string.Concat(" \r\nHAVING ", _having.Substring(5));
			string sql = string.Concat("SELECT ", fields, _table, _join, where, _groupby, having, _sort, limit);
			return _exec.ExeucteArray(CommandType.Text, sql);
		}
		public T Aggregate<T>(string fields) {
			return Lib.ConvertTo<T>(this.Aggregate(fields)[0][0]);
		}
		public int Count() {
			return this.Aggregate<int>("count(1)");
		}
		public SelectBuild<TReturnInfo> Count(out int count) {
			count = this.Count();
			return this;
		}
		public static SelectBuild<TReturnInfo> From(IDAL dal, Executer exec) {
			return new SelectBuild<TReturnInfo>(dal, exec);
		}
		int _fields_count = 0;
		protected SelectBuild(IDAL dal, Executer exec) {
			_dals.Add(dal);
			_field = dal.Field;
			_table = string.Concat(" \r\nFROM ", dal.Table, " a");
			_exec = exec;
		}
		public SelectBuild<TReturnInfo> From<TBLL>() {
			return this.From<TBLL>(string.Empty);
		}
		public SelectBuild<TReturnInfo> From<TBLL>(string alias) {
			IDAL dal = this.ConvertTBLL<TBLL>();
			_table = string.Concat(_table, ", ", dal.Table, " ", alias);
			return this;
		}
		protected IDAL ConvertTBLL<TBLL>() {
			string dalTypeName = typeof(TBLL).FullName.Replace(".BLL.", ".DAL.");
			IDAL dal = this.GetType().GetTypeInfo().Assembly.CreateInstance(dalTypeName) as IDAL;
			if (dal == null) throw new Exception(string.Concat("找不到类型 ", dalTypeName));
			return dal;
		}
		protected SelectBuild<TReturnInfo> Join<TBLL>(string alias, string on, string joinType) {
			IDAL dal = this.ConvertTBLL<TBLL>();
			_dals.Add(dal);
			string fields2 = dal.Field.Replace("a.", string.Concat(alias, "."));
			string[] names = fields2.Split(new string[] { ", " }, StringSplitOptions.None);
			for (int a = 0; a < names.Length; a++) {
				string ast = string.Concat(" as", ++_fields_count);
				names[a] = string.Concat(names[a], ast);
			}
			_field = string.Concat(_field, ", \r\n", string.Join(", ", names));
			_join = string.Concat(_join, " \r\n", joinType, " ", dal.Table, " ", alias, " ON ", on);
			return this;
		}
		public SelectBuild<TReturnInfo> Where(string filter, params object[] parms) {
			return this.Where(true, filter, parms);
		}
		public SelectBuild<TReturnInfo> Where(bool isadd, string filter, params object[] parms) {
			if (isadd) {
				//将参数 = null 转换成 IS NULL
				if (parms != null && parms.Length > 0) {
					for (int a = 0; a < parms.Length; a++)
						if (parms[a] == null)
							filter = Regex.Replace(filter, @"\s+=\s+\{" + a + @"\}", " IS {" + a + "}");
				}
				_where = string.Concat(_where, " AND (", Executer.Addslashes(filter, parms), ")");
			}
			return this;
		}
		public SelectBuild<TReturnInfo> GroupBy(string groupby) {
			_groupby = groupby;
			if (string.IsNullOrEmpty(_groupby)) return this;
			_groupby = string.Concat(" \r\nGROUP BY ", _groupby);
			return this;
		}
		public SelectBuild<TReturnInfo> Having(string filter, params object[] parms) {
			return this.Having(true, filter, parms);
		}
		public SelectBuild<TReturnInfo> Having(bool isadd, string filter, params object[] parms) {
			if (string.IsNullOrEmpty(_groupby)) return this;
			if (isadd) _having = string.Concat(_having, " AND (", Executer.Addslashes(filter, parms), ")");
			return this;
		}
		public SelectBuild<TReturnInfo> Sort(string sort) {
			if (!string.IsNullOrEmpty(sort)) _sort = string.Concat(" \r\nORDER BY ", sort);
			return this;
		}
		public SelectBuild<TReturnInfo> OrderBy(string sort) {
			return this.Sort(sort);
		}
		public SelectBuild<TReturnInfo> InnerJoin<TBLL>(string alias, string on) {
			return this.Join<TBLL>(alias, on, "INNER JOIN");
		}
		public SelectBuild<TReturnInfo> LeftJoin<TBLL>(string alias, string on) {
			return this.Join<TBLL>(alias, on, "LEFT JOIN");
		}
		public SelectBuild<TReturnInfo> RightJoin<TBLL>(string alias, string on) {
			return this.Join<TBLL>(alias, on, "RIGHT JOIN");
		}
		public SelectBuild<TReturnInfo> Skip(int skip) {
			_skip = skip;
			return this;
		}
		public SelectBuild<TReturnInfo> Limit(int limit) {
			_limit = limit;
			return this;
		}
	}
}