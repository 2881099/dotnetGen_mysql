using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;

namespace Model {

	[Serializable]
	public class ColumnInfo {

		private string _name;
		private MySqlDbType _type;
		private long _length;
		private string _sqlType;
		private DataSort _orderby;
		private bool _isNullable;
		private bool _isIdentity;
		private bool _isClustered;
		private bool _isPrimaryKey;

		public ColumnInfo() { }
		public ColumnInfo(string name, MySqlDbType type, long length, string sqlType, DataSort orderby, bool isNullable, bool isIdentity, bool isClustered, bool isPrimaryKey) {
			_name = name;
			_type = type;
			_length = length;
			_sqlType = sqlType;
			_orderby = orderby;
			_isNullable = isNullable;
			_isIdentity = isIdentity;
			_isClustered = isClustered;
			_isPrimaryKey = isPrimaryKey;
		}

		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		public MySqlDbType Type {
			get { return _type; }
			set { _type = value; }
		}
		public long Length {
			get { return _length; }
			set { _length = value; }
		}
		public string SqlType {
			get { return _sqlType; }
			set { _sqlType = value; }
		}
		public DataSort Orderby {
			get { return _orderby; }
			set { _orderby = value; }
		}
		public bool IsNullable {
			get { return _isNullable; }
			set { _isNullable = value; }
		}
		public bool IsIdentity {
			get { return _isIdentity; }
			set { _isIdentity = value; }
		}
		public bool IsClustered {
			get { return _isClustered; }
			set { _isClustered = value; }
		}
		public bool IsPrimaryKey {
			get { return _isPrimaryKey; }
			set { _isPrimaryKey = value; }
		}
	}
}
