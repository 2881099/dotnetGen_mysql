using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Model {

	[Serializable]
	public class TableInfo {

		private string _id;
		private string _owner;
		private string _name;
		private List<ColumnInfo> _columns = new List<ColumnInfo>();
		private List<List<ColumnInfo>> _uniques = new List<List<ColumnInfo>>();
		private List<List<ColumnInfo>> _indexes = new List<List<ColumnInfo>>();
		private List<ForeignKeyInfo> _foreignKeys = new List<ForeignKeyInfo>();
		private List<ColumnInfo> _identitys = new List<ColumnInfo>();
		private List<ColumnInfo> _clustereds = new List<ColumnInfo>();
		private List<ColumnInfo> _primaryKeys = new List<ColumnInfo>();
		private string _Type;
		private bool _IsOutput;

		public TableInfo(string id, string owner, string name, string type) {
			_id = id;
			_owner = owner;
			_name = name;
			_Type = type;
		}

		public static string GetClassName(string name) {
			int rr = 0;
			string n = name.StartsWith(".") ? name.Substring(1) : Regex.Replace(name, @"\.", delegate(Match m) {
				if (rr++ > 0) return m.Groups[0].Value;
				return "_";
			});
			return char.IsLetter(n, 0) ? n : string.Concat("_", n);
		}
		public static string GetEntryName(string name) {
			int idx = name.IndexOf('.');
			return idx == -1 ? name : name.Substring(idx + 1);
		}

		public string Id {
			get { return _id; }
		}
		public string Owner {
			get { return _owner; }
		}
		public string Name {
			get { return _name; }
		}
		public string ClassName {
			get {
				return GetClassName(_owner.ToLower() + "." + _name);
			}
		}
		public string FullName {
			get { return string.IsNullOrEmpty(_owner) ? _name : string.Format("{0}.{1}", _owner, _name); }
		}
		public string Type {
			get { return _Type; }
		}

		public List<ColumnInfo> Columns {
			get { return _columns; }
		}
		public List<List<ColumnInfo>> Uniques {
			get {
				if (_uniques == null) {

				}
				return _uniques;
			}
		}
		public List<List<ColumnInfo>> Indexes {
			get {
				if (_indexes == null) {

				}
				return _indexes;
			}
		}
		public List<ForeignKeyInfo> ForeignKeys {
			get {
				if (_foreignKeys == null) {

				}
				return _foreignKeys;
			}
		}
		public List<ColumnInfo> PrimaryKeys {
			get {
				if (_primaryKeys == null) {

				}
				return _primaryKeys;
			}
		}
		public List<ColumnInfo> Clustereds {
			get {
				if (_clustereds == null) {

				}
				return _clustereds;
			}
		}
		public List<ColumnInfo> Identitys {
			get {
				if (_identitys == null) {

				}
				return _identitys;
			}
		}

		public bool IsOutput {
			get { return _IsOutput; }
			set { _IsOutput = value; }
		}
	}
}
