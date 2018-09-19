using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Model;

namespace Server {

	internal partial class CodeBuild : IDisposable {
		private ClientInfo _client;
		private AcceptSocket _socket;
		private List<TableInfo> _tables;
		private Dictionary<string, Dictionary<string, string>> _column_coments = new Dictionary<string, Dictionary<string, string>>();

		public CodeBuild(ClientInfo client, AcceptSocket socket) {
			_client = client;
			_socket = socket;
		}

		private object[][] GetDataSet(string commandText) {
			SocketMessager messager = new SocketMessager("ExecuteDataSet", commandText);
			_socket.Write(messager, delegate(object sender, ServerSocketReceiveEventArgs e) {
				messager = e.Messager;
			});
			object[][] ret = messager.Arg as object[][]; //兼容.netcore传过来的数据
			if (ret == null) {
				DataSet ds = messager.Arg as DataSet; //兼容.net传过来的数据
				if (ds != null) {
					List<object[]> tmp = new List<object[]>();
					foreach (DataRow row in ds.Tables[0].Rows)
						tmp.Add(row.ItemArray);
					ret = tmp.ToArray();
				}
			}
			return ret;
		}
		private int ExecuteNonQuery(string commandText) {
			SocketMessager messager = new SocketMessager("ExecuteNonQuery", commandText);
			_socket.Write(messager, delegate(object sender, ServerSocketReceiveEventArgs e) {
				messager = e.Messager;
			});
			int val;
			int.TryParse(string.Concat(messager.Arg), out val);
			return val;
		}

		public List<DatabaseInfo> GetDatabases() {
			Logger.remotor.Info("GetDatabases: " + _client.Server + "," + _client.Username + "," + _client.Password);

			List<DatabaseInfo> loc1 = null;

			object[][] ds = this.GetDataSet(@"select schema_name from information_schema.schemata where schema_name not in ('information_schema', 'mysql', 'performance_schema')");
			if (ds == null) return loc1;

			loc1 = new List<DatabaseInfo>();
			foreach (object[] row in ds) {
				loc1.Add(new DatabaseInfo(string.Concat(row[0])));
			}
			return loc1;
		}
		public List<TableInfo> GetTablesByDatabase(string database) {
			_client.Database = database;
			Logger.remotor.Info("GetTablesByDatabase: " + _client.Server + "," + _client.Username + "," + _client.Password + "," + _client.Database);
			string[] dbs = database.Split(',');

			List<TableInfo> loc1 = _tables = null;
			Dictionary<string, TableInfo> loc2 = new Dictionary<string, TableInfo>();
			Dictionary<string, Dictionary<string, ColumnInfo>> loc3 = new Dictionary<string, Dictionary<string, ColumnInfo>>();

			object[][] ds = this.GetDataSet(string.Format(@"
select 
concat(a.table_schema, '.', a.table_name) 'id',
a.table_schema 'owner',
a.table_name 'table',
'T'
from information_schema.tables a
where a.table_schema in ('{0}')
", database.Replace("'", "''").Replace(",", "','")));
			if (ds == null) return loc1;

			List<string> loc6 = new List<string>();
			List<string> loc66 = new List<string>();
			foreach (object[] row in ds) {
				string table_id = string.Concat(row[0]);
				string owner = string.Concat(row[1]);
				string table = string.Concat(row[2]);
				string type = string.Concat(row[3]);
				if (dbs.Length == 1) {
					table_id = table_id.Substring(table_id.IndexOf('.') + 1);
					owner = "";
				}
				loc2.Add(table_id, new TableInfo(table_id, owner, table, type));
				loc3.Add(table_id, new Dictionary<string, ColumnInfo>());
				switch (type) {
					case "T":
						loc6.Add(table.Replace("'", "''"));
						break;
					case "P":
						loc66.Add(table.Replace("'", "''"));
						break;
				}
			}
			if (loc6.Count == 0) return loc1;
			string loc8 = "'" + string.Join("','", loc6.ToArray()) + "'";
			string loc88 = "'" + string.Join("','", loc66.ToArray()) + "'";

			ds = this.GetDataSet(string.Format(@"
SELECT
concat(a.table_schema, '.', a.table_name),
a.column_name,
a.data_type,
ifnull(a.character_maximum_length, 0) 'len',
a.column_type,
case when a.is_nullable then 1 else 0 end 'is_nullable',
case when locate('auto_increment', a.extra) > 0 then 1 else 0 end 'is_identity',
a.column_comment 'comment'
from information_schema.columns a
where a.table_schema in ('{1}') and a.table_name in ({0})
", loc8, database.Replace("'", "''").Replace(",", "','")));
			if (ds == null) return loc1;

			foreach (object[] row in ds) {
				string table_id = string.Concat(row[0]);
				string column = string.Concat(row[1]);
				string type = string.Concat(row[2]);
				//long max_length = long.Parse(string.Concat(row[3]));
				string sqlType = string.Concat(row[4]);
				Match m_len = Regex.Match(sqlType, @"\w+\((\d+)");
				long max_length = m_len.Success ? int.Parse(m_len.Groups[1].Value) : -1;
				bool is_nullable = string.Concat(row[5]) == "1";
				bool is_identity = string.Concat(row[6]) == "1";
				string comment = string.Concat(row[7]);
				if (string.IsNullOrEmpty(comment)) comment = column;
				if (max_length == 0) max_length = -1;
				if (dbs.Length == 1) {
					table_id = table_id.Substring(table_id.IndexOf('.') + 1);
				}
				loc3[table_id].Add(column, new ColumnInfo(
					column, CodeBuild.GetDBType(type, sqlType.EndsWith(" unsigned")), max_length, sqlType,
					DataSort.ASC, is_nullable, is_identity, false, false));
				if (!_column_coments.ContainsKey(table_id)) _column_coments.Add(table_id, new Dictionary<string, string>());
				if (!_column_coments[table_id].ContainsKey(column)) _column_coments[table_id].Add(column, comment);
				else _column_coments[table_id][column] = comment;
			}

			ds = this.GetDataSet(string.Format(@"
select 
concat(a.constraint_schema, '.', a.table_name) 'table_id',
a.column_name,
concat(a.constraint_schema, '/', a.table_name, '/', a.constraint_name) 'index_id',
1 'IsUnique',
case when constraint_name = 'PRIMARY' then 1 else 0 end 'IsPrimaryKey',
0 'IsClustered',
0 'IsDesc'
from information_schema.key_column_usage a
where a.constraint_schema in ('{1}') and a.table_name in ({0}) and isnull(position_in_unique_constraint)
", loc8, database.Replace("'", "''").Replace(",", "','")));
			if (ds == null) return loc1;

			Dictionary<string, Dictionary<string, List<ColumnInfo>>> indexColumns = new Dictionary<string, Dictionary<string, List<ColumnInfo>>>();
			Dictionary<string, Dictionary<string, List<ColumnInfo>>> uniqueColumns = new Dictionary<string, Dictionary<string, List<ColumnInfo>>>();
			foreach (object[] row in ds) {
				string table_id = string.Concat(row[0]);
				string column = string.Concat(row[1]);
				string index_id = string.Concat(row[2]);
				bool is_unique = string.Concat(row[3]) == "1";
				bool is_primary_key = string.Concat(row[4]) == "1";
				bool is_clustered = string.Concat(row[5]) == "1";
				int is_desc = int.Parse(string.Concat(row[6]));
				if (dbs.Length == 1) {
					table_id = table_id.Substring(table_id.IndexOf('.') + 1);
				}
				if (loc3.ContainsKey(table_id) == false || loc3[table_id].ContainsKey(column) == false) continue;
				ColumnInfo loc9 = loc3[table_id][column];
				if (loc9.IsClustered == false && is_clustered) loc9.IsClustered = is_clustered;
				if (loc9.IsPrimaryKey == false && is_primary_key) loc9.IsPrimaryKey = is_primary_key;
				if (loc9.Orderby == DataSort.NONE) loc9.Orderby = (DataSort)is_desc;
				
				Dictionary<string, List<ColumnInfo>> loc10 = null;
				List<ColumnInfo> loc11 = null;
				if (!indexColumns.TryGetValue(table_id, out loc10)) {
					indexColumns.Add(table_id, loc10 = new Dictionary<string, List<ColumnInfo>>());
				}
				if (!loc10.TryGetValue(index_id, out loc11)) {
					loc10.Add(index_id, loc11 = new List<ColumnInfo>());
				}
				loc11.Add(loc9);
				if (is_unique) {
					if (!uniqueColumns.TryGetValue(table_id, out loc10)) {
						uniqueColumns.Add(table_id, loc10 = new Dictionary<string, List<ColumnInfo>>());
					}
					if (!loc10.TryGetValue(index_id, out loc11)) {
						loc10.Add(index_id, loc11 = new List<ColumnInfo>());
					}
					loc11.Add(loc9);
				}
			}
			foreach (string table_id in indexColumns.Keys) {
				foreach (List<ColumnInfo> columns in indexColumns[table_id].Values) {
					loc2[table_id].Indexes.Add(columns);
				}
			}
			foreach (string table_id in uniqueColumns.Keys) {
				foreach (List<ColumnInfo> columns in uniqueColumns[table_id].Values) {
					columns.Sort(delegate(ColumnInfo c1, ColumnInfo c2) {
						return c1.Name.CompareTo(c2.Name);
					});
					loc2[table_id].Uniques.Add(columns);
				}
			}
			ds = this.GetDataSet(string.Format(@"
select 
concat(a.constraint_schema, '.', a.table_name) 'table_id',
a.column_name,
concat(a.constraint_schema, '/', a.constraint_name) 'FKId',
concat(a.referenced_table_schema, '.', a.referenced_table_name) 'ref_table_id',
1 'IsForeignKey',
a.referenced_column_name 'ref_column',
null 'ref_sln',
null 'ref_table'
from information_schema.key_column_usage a
where a.constraint_schema in ('{1}') and a.table_name in ({0}) and not isnull(position_in_unique_constraint)
", loc8, database.Replace("'", "''").Replace(",", "','")));
			if (ds == null) return loc1;

			Dictionary<string, Dictionary<string, ForeignKeyInfo>> fkColumns = new Dictionary<string, Dictionary<string, ForeignKeyInfo>>();
			foreach (object[] row in ds) {
				string table_id = string.Concat(row[0]);
				string column = string.Concat(row[1]);
				string fk_id = string.Concat(row[2]);
				string ref_table_id = string.Concat(row[3]);
				bool is_foreign_key = string.Concat(row[4]) == "1";
				string referenced_column = string.Concat(row[5]);
				string referenced_db = string.Concat(row[6]);
				string referenced_table = string.Concat(row[7]);
				if (dbs.Length == 1) {
					table_id = table_id.Substring(table_id.IndexOf('.') + 1);
					ref_table_id = ref_table_id.Substring(ref_table_id.IndexOf('.') + 1);
				}
				if (loc3.ContainsKey(table_id) == false || loc3[table_id].ContainsKey(column) == false) continue;
				ColumnInfo loc9 = loc3[table_id][column];
				TableInfo loc10 = null;
				ColumnInfo loc11 = null;
				bool isThisSln = !string.IsNullOrEmpty(ref_table_id);
				if (isThisSln) {
					if (loc2.ContainsKey(ref_table_id) == false) continue;
					loc10 = loc2[ref_table_id];
					loc11 = loc3[ref_table_id][referenced_column];
				} else {

				}
				Dictionary<string, ForeignKeyInfo> loc12 = null;
				ForeignKeyInfo loc13 = null;
				if (!fkColumns.TryGetValue(table_id, out loc12)) {
					fkColumns.Add(table_id, loc12 = new Dictionary<string, ForeignKeyInfo>());
				}
				if (!loc12.TryGetValue(fk_id, out loc13)) {
					if (isThisSln) {
						loc13 = new ForeignKeyInfo(loc2[table_id], loc10);
					} else {
						loc13 = new ForeignKeyInfo(referenced_db, referenced_table, is_foreign_key);
					}
					loc12.Add(fk_id, loc13);
				}
				loc13.Columns.Add(loc9);

				if (isThisSln) {
					loc13.ReferencedColumns.Add(loc11);
				} else {
					loc13.ReferencedColumnNames.Add(referenced_column);
				}
			}
			foreach (string table_id in fkColumns.Keys) {
				foreach (ForeignKeyInfo fk in fkColumns[table_id].Values) {
					loc2[table_id].ForeignKeys.Add(fk);
				}
			}

			foreach (string table_id in loc3.Keys) {
				foreach (ColumnInfo loc5 in loc3[table_id].Values) {
					loc2[table_id].Columns.Add(loc5);
					if (loc5.IsIdentity) {
						loc2[table_id].Identitys.Add(loc5);
					}
					if (loc5.IsClustered) {
						loc2[table_id].Clustereds.Add(loc5);
					}
					if (loc5.IsPrimaryKey) {
						loc2[table_id].PrimaryKeys.Add(loc5);
					}
				}
			}
			loc1 = _tables = new List<TableInfo>();
			foreach (TableInfo loc4 in loc2.Values) {
				if (loc4.PrimaryKeys.Count == 0 && loc4.Uniques.Count > 0) {
					foreach (ColumnInfo loc5 in loc4.Uniques[0]) {
						loc5.IsPrimaryKey = true;
						loc4.PrimaryKeys.Add(loc5);
					}
				}
				this.Sort(loc4);
				loc1.Add(loc4);
			}
			loc1.Sort(delegate (TableInfo t1, TableInfo t2) {
				return t1.FullName.CompareTo(t2.FullName);
			});

			loc2.Clear();
			loc3.Clear();
			return loc1;
		}

		protected virtual void Sort(TableInfo table) {
			table.PrimaryKeys.Sort(delegate (ColumnInfo c1, ColumnInfo c2) {
				return c1.Name.CompareTo(c2.Name);
			});
			table.Columns.Sort(delegate(ColumnInfo c1, ColumnInfo c2) {
				int compare = c2.IsPrimaryKey.CompareTo(c1.IsPrimaryKey);
				if (compare == 0) {
					bool b1 = table.ForeignKeys.Find(delegate(ForeignKeyInfo fk) {
						return fk.Columns.Find(delegate(ColumnInfo c3) {
							return c3.Name == c1.Name;
						}) != null;
					}) != null;
					bool b2 = table.ForeignKeys.Find(delegate(ForeignKeyInfo fk) {
						return fk.Columns.Find(delegate(ColumnInfo c3) {
							return c3.Name == c2.Name;
						}) != null;
					}) != null;
					compare = b2.CompareTo(b1);
				}
				if (compare == 0) compare = c1.Name.CompareTo(c2.Name);
				return compare;
			});
		}

		#region IDisposable 成员

		public void Dispose() {
			if (_tables != null) {
				_tables.Clear();
			}
		}

		#endregion
	}
}
