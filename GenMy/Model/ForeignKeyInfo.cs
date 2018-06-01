using System;
using System.Text;
using System.Collections.Generic;

namespace Model {

	[Serializable]
	public class ForeignKeyInfo {
		private TableInfo _table;
		private List<ColumnInfo> _columns = new List<ColumnInfo>();

		private TableInfo _referencedTable;
		private List<ColumnInfo> _referencedColumns = new List<ColumnInfo>();

		private string _referencedDBName;
		private string _referencedTableName;
		private List<string> _referencedColumnNames = new List<string>();
		private bool _referencedIsPrimaryKey;

		public ForeignKeyInfo(TableInfo table, TableInfo referencedTable) {
			_table = table;
			_referencedTable = referencedTable;
		}
		public ForeignKeyInfo(string referencedSln, string referencedTableName, bool referencedIsPK) {
			_referencedDBName = referencedSln;
			_referencedTableName = referencedTableName;
			_referencedIsPrimaryKey = referencedIsPK;
		}

		public TableInfo Table {
			get { return _table; }
			set { _table = value; }
		}
		public List<ColumnInfo> Columns {
			get { return _columns; }
			set { _columns = value; }
		}

		public TableInfo ReferencedTable {
			get { return _referencedTable; }
			set { _referencedTable = value; }
		}
		public List<ColumnInfo> ReferencedColumns {
			get { return _referencedColumns; }
			set { _referencedColumns = value; }
		}

		public string ReferencedDBName {
			get { return _referencedDBName; }
			set { _referencedDBName = value; }
		}
		public string ReferencedTableName {
			get { return _referencedTableName; }
			set { _referencedTableName = value; }
		}
		public List<string> ReferencedColumnNames {
			get { return _referencedColumnNames; }
			set { _referencedColumnNames = value; }
		}
		public bool ReferencedIsPrimaryKey {
			get { return _referencedIsPrimaryKey; }
			set { _referencedIsPrimaryKey = value; }
		}
	}
}
