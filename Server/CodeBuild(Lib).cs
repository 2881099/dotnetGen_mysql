using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Text.RegularExpressions;
using Model;

namespace Server {

	internal partial class CodeBuild {
		protected static MySqlDbType GetDBType(string strType, bool is_unsigned) {
			switch (strType.ToLower()) {
				case "bit": return MySqlDbType.Bit;

				case "tinyint": return is_unsigned ? MySqlDbType.UByte : MySqlDbType.Byte;
				case "smallint": return is_unsigned ? MySqlDbType.UInt16 : MySqlDbType.Int16;
				case "mediumint": return is_unsigned ? MySqlDbType.UInt24 : MySqlDbType.Int24;
				case "int": return is_unsigned ? MySqlDbType.UInt32 : MySqlDbType.Int32;
				case "bigint": return is_unsigned ? MySqlDbType.UInt64 : MySqlDbType.Int64;

				case "real":
				case "double": return MySqlDbType.Double;
				case "float": return MySqlDbType.Float;
				case "numeric":
				case "decimal": return MySqlDbType.Decimal;

				case "year": return MySqlDbType.Year;
				case "time": return MySqlDbType.Time;
				case "date": return MySqlDbType.Date;
				case "timestamp": return MySqlDbType.Timestamp;
				case "datetime": return MySqlDbType.Datetime;

				case "tinyblob": return MySqlDbType.TinyBlob;
				case "blob": return MySqlDbType.Blob;
				case "mediumblob": return MySqlDbType.MediumBlob;
				case "longblob": return MySqlDbType.LongBlob;

				case "binary": return MySqlDbType.Binary;
				case "varbinary": return MySqlDbType.VarBinary;

				case "tinytext": return MySqlDbType.TinyText;
				case "text": return MySqlDbType.Text;
				case "mediumtext": return MySqlDbType.MediumText;
				case "longtext": return MySqlDbType.LongText;

				case "char": return MySqlDbType.String;
				case "varchar": return MySqlDbType.VarChar;

				case "set": return MySqlDbType.Set;
				case "enum": return MySqlDbType.Enum;

				case "point": return MySqlDbType.Geometry;
				case "linestring": return MySqlDbType.Geometry;
				case "polygon": return MySqlDbType.Geometry;
				case "geometry": return MySqlDbType.Geometry;
				case "multipoint": return MySqlDbType.Geometry;
				case "multilinestring": return MySqlDbType.Geometry;
				case "multipolygon": return MySqlDbType.Geometry;
				case "geometrycollection": return MySqlDbType.Geometry;
				default: return MySqlDbType.String;
			}
		}

		protected static string GetDbToCsConvert(MySqlDbType type) {
			switch (type) {
				case MySqlDbType.Bit: return "(bool?)";

				case MySqlDbType.Byte: return "(byte?)";
				case MySqlDbType.Int16: return "(short?)";
				case MySqlDbType.Int24:
				case MySqlDbType.Int32: return "(int?)";
				case MySqlDbType.Int64: return "(long?)";

				case MySqlDbType.UByte: return "(byte?)";
				case MySqlDbType.UInt16: return "(ushort?)";
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32: return "(uint?)";
				case MySqlDbType.UInt64: return "(ulong?)";

				case MySqlDbType.Double: return "(double?)";
				case MySqlDbType.Float: return "(float?)";
				case MySqlDbType.Decimal: return "(decimal?)";

				case MySqlDbType.Year: return "(int?)";
				case MySqlDbType.Time: return "(TimeSpan?)";
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return "(DateTime?)";

				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return "(byte[])";

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar: return "";

				case MySqlDbType.Set:
				case MySqlDbType.Enum: return "(long?)";

				case MySqlDbType.Geometry: return "(MygisGeometry)";
				default: return "";
			}
		}

		protected static string GetCSTypeValue(MySqlDbType type) {
			switch (type) {
				case MySqlDbType.Bit:

				case MySqlDbType.Byte:
				case MySqlDbType.Int16:
				case MySqlDbType.Int24:
				case MySqlDbType.Int32:
				case MySqlDbType.Int64:

				case MySqlDbType.UByte:
				case MySqlDbType.UInt16:
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32:
				case MySqlDbType.UInt64:

				case MySqlDbType.Double:
				case MySqlDbType.Float:
				case MySqlDbType.Decimal:
				
				case MySqlDbType.Year:
				case MySqlDbType.Time:
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return "{0}.Value";

				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return "Convert.ToBase64String({0})";

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar: return "{0}";

				case MySqlDbType.Set:
				case MySqlDbType.Enum: return "{0}";

				case MySqlDbType.Geometry: return "{0}";
				default: return "";
			}
		}
		protected static string GetCSType(MySqlDbType type, string enumType, string sqlType) {
			switch (type) {
				case MySqlDbType.Bit: return "bool?";

				case MySqlDbType.Byte: return "byte?";
				case MySqlDbType.Int16: return "short?";
				case MySqlDbType.Int24:
				case MySqlDbType.Int32: return "int?";
				case MySqlDbType.Int64: return "long?";

				case MySqlDbType.UByte: return "byte?";
				case MySqlDbType.UInt16: return "ushort?";
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32: return "uint?";
				case MySqlDbType.UInt64: return "ulong?";

				case MySqlDbType.Double: return "double?";
				case MySqlDbType.Float: return "float?";
				case MySqlDbType.Decimal: return "decimal?";

				case MySqlDbType.Year: return "int?";
				case MySqlDbType.Time: return "TimeSpan?";
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return "DateTime?";

				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return "byte[]";

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar: return "string";

				case MySqlDbType.Set: return enumType + "?";
				case MySqlDbType.Enum: return enumType + "?";

				case MySqlDbType.Geometry: return GetCSTypeGeometry(sqlType);
				default: return "object";
			}
		}

		protected static string GetCSTypeGeometry(string sqlType) {
			switch (sqlType) {
				case "point": return "MygisPoint";
				case "linestring": return "MygisLineString";
				case "polygon": return "MygisPolygon";
				case "multipoint": return "MygisMultiPoint";
				case "multilinestring": return "MygisMultiLineString";
				case "multipolygon": return "MygisMultiPolygon";
				case "geometrycollection": return "MygisGeometryCollection";
				case "geometry": return "MygisGeometry";
			}
			return "MygisGeometry";
		}

		protected static string GetDataReaderMethod(MySqlDbType type) {
			switch (type) {
				case MySqlDbType.Bit: return "GetBoolean";

				case MySqlDbType.Byte: return "GetByte";
				case MySqlDbType.Int16: return "GetInt16";
				case MySqlDbType.Int24:
				case MySqlDbType.Int32: return "GetInt32";
				case MySqlDbType.Int64: return "GetInt64";

				case MySqlDbType.UByte: return "GetByte";
				case MySqlDbType.UInt16: return "GetInt16";
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32: return "GetInt32";
				case MySqlDbType.UInt64: return "GetInt64";

				case MySqlDbType.Double: return "GetDouble";
				case MySqlDbType.Float: return "GetFloat";
				case MySqlDbType.Decimal: return "GetDecimal";

				case MySqlDbType.Year: return "GetInt32";
				case MySqlDbType.Time: return "GetValue";
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return "GetDateTime";

				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return "GetBytes";

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar: return "GetString";

				case MySqlDbType.Set:
				case MySqlDbType.Enum: return "GetString";

				case MySqlDbType.Geometry: return "GetString";
				default: return "GetValue";
			}
		}

		protected static string GetToStringFieldConcat(ColumnInfo columnInfo, string csType) {
			switch (columnInfo.Type) {
				case MySqlDbType.Bit: return string.Format("{0} == null ? \"null\" : ({0} == true ? \"true\" : \"false\")", CodeBuild.UFString(columnInfo.Name));

				case MySqlDbType.Byte:
				case MySqlDbType.Int16:
				case MySqlDbType.Int24:
				case MySqlDbType.Int32:
				case MySqlDbType.Int64:

				case MySqlDbType.UByte:
				case MySqlDbType.UInt16:
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32:
				case MySqlDbType.UInt64:

				case MySqlDbType.Double:
				case MySqlDbType.Float:
				case MySqlDbType.Decimal:

				case MySqlDbType.Year: return string.Format("{0} == null ? \"null\" : {0}.ToString()", CodeBuild.UFString(columnInfo.Name));
				case MySqlDbType.Time: return string.Format("{0} == null ? \"null\" : {0}.Value.TotalSeconds.ToString()", CodeBuild.UFString(columnInfo.Name));
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return string.Format("{0} == null ? \"null\" : {0}.Value.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString()", CodeBuild.UFString(columnInfo.Name));

				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return string.Format("{0} == null ? \"null\" : Convert.ToBase64String({0})", CodeBuild.UFString(columnInfo.Name));

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar: return string.Format("{0} == null ? \"null\" : string.Format(\"'{{0}}'\", {0}.Replace(\"\\\\\", \"\\\\\\\\\").Replace(\"\\r\\n\", \"\\\\r\\\\n\").Replace(\"'\", \"\\\\'\"))", CodeBuild.UFString(columnInfo.Name));

				case MySqlDbType.Set: return string.Format("{0} == null ? \"null\" : string.Format(\"[ '{{0}}' ]\", string.Join(\"', '\", {0}.ToInt64().ToSet<{1}>().Select<{1}, string>(a => a.ToDescriptionOrString().Replace(\"\\\\\", \"\\\\\\\\\").Replace(\"\\r\\n\", \"\\\\r\\\\n\").Replace(\"'\", \"\\\\'\"))))", CodeBuild.UFString(columnInfo.Name), csType);
				case MySqlDbType.Enum: return string.Format("{0} == null ? \"null\" : string.Format(\"'{{0}}'\", {0}.ToDescriptionOrString().Replace(\"\\\\\", \"\\\\\\\\\").Replace(\"\\r\\n\", \"\\\\r\\\\n\").Replace(\"'\", \"\\\\'\"))", CodeBuild.UFString(columnInfo.Name));

				case MySqlDbType.Geometry: return string.Format("{0} == null ? \"null\" : string.Format(\"'{{0}}'\", {0}.Replace(\"\\\\\", \"\\\\\\\\\").Replace(\"\\r\\n\", \"\\\\r\\\\n\").Replace(\"'\", \"\\\\'\"))", CodeBuild.UFString(columnInfo.Name));
				default: return string.Format("{0} == null ? \"null\" : {0}.ToString()", CodeBuild.UFString(columnInfo.Name));
			}
		}

		protected static string GetToStringStringify(ColumnInfo columnInfo)
        {
            switch (columnInfo.Type)
            {
				case MySqlDbType.Bit: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : (_" + CodeBuild.UFString(columnInfo.Name) + " == true ? \"1\" : \"0\")";

				case MySqlDbType.Byte:
				case MySqlDbType.Int16:
				case MySqlDbType.Int24:
				case MySqlDbType.Int32:
				case MySqlDbType.Int64:

				case MySqlDbType.UByte:
				case MySqlDbType.UInt16:
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32:
				case MySqlDbType.UInt64:

				case MySqlDbType.Double:
				case MySqlDbType.Float:
				case MySqlDbType.Decimal:

				case MySqlDbType.Year: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".ToString()";
				case MySqlDbType.Time: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".Value.TotalSeconds.ToString()";
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".Value.Ticks.ToString()";

				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : Convert.ToBase64String(_" + CodeBuild.UFString(columnInfo.Name) + ")";

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".Replace(\"|\", StringifySplit)";

				case MySqlDbType.Set:
				case MySqlDbType.Enum: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".ToInt64().ToString()";

				case MySqlDbType.Geometry: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".Replace(\"|\", StringifySplit)";
				default: return "_" + CodeBuild.UFString(columnInfo.Name) + " == null ? \"null\" : _" + CodeBuild.UFString(columnInfo.Name) + ".ToString().Replace(\"|\", StringifySplit)";
            }
        }
        protected static string GetStringifyParse(MySqlDbType type, string sqlType)
        {
			switch (type)
            {
				case MySqlDbType.Bit: return "{0} == \"1\"";

				case MySqlDbType.Byte: return "byte.Parse({0})";
				case MySqlDbType.Int16: return "short.Parse({0})";
				case MySqlDbType.Int24: 
				case MySqlDbType.Int32: return "int.Parse({0})";
				case MySqlDbType.Int64: return "long.Parse({0})";

				case MySqlDbType.UByte: return "byte.Parse({0})";
				case MySqlDbType.UInt16: return "ushort.Parse({0})";
				case MySqlDbType.UInt24:
				case MySqlDbType.UInt32: return "uint.Parse({0})";
				case MySqlDbType.UInt64: return "ulong.Parse({0})";

				case MySqlDbType.Double: return "double.Parse({0})";
				case MySqlDbType.Float: return "float.Parse({0})";
				case MySqlDbType.Decimal: return "decimal.Parse({0})";
				
				case MySqlDbType.Year: return "int.Parse({0})";
				case MySqlDbType.Time: return "TimeSpan.FromSeconds(double.Parse({0}))";
				case MySqlDbType.Date:
				case MySqlDbType.Timestamp:
				case MySqlDbType.Datetime: return "new DateTime(long.Parse({0}))";
				
				case MySqlDbType.TinyBlob:
				case MySqlDbType.Blob:
				case MySqlDbType.MediumBlob:
				case MySqlDbType.LongBlob:

				case MySqlDbType.Binary:
				case MySqlDbType.VarBinary: return "Convert.FromBase64String({0})";

				case MySqlDbType.TinyText:
				case MySqlDbType.Text:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:

				case MySqlDbType.String:
				case MySqlDbType.VarString:
				case MySqlDbType.VarChar:

				case MySqlDbType.Set: return "{0}.Replace(StringifySplit, \"|\")";
				case MySqlDbType.Enum: return "{0}.Replace(StringifySplit, \"|\")";

				case MySqlDbType.Geometry: return "{0}.Replace(StringifySplit, \"|\")";
				default: return "{0}";
            }
        }

        protected static string UFString(string text) {
			if (text.Length <= 1) return text.ToUpper();
			else return text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
		}

		protected static string LFString(string text) {
			if (text.Length <= 1) return text.ToLower();
			else return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
		}

		protected static string GetCSName(string name) {
			name = Regex.Replace(name.TrimStart('@'), @"[^\w]", "_");
			return char.IsLetter(name, 0) ? name : string.Concat("_", name);
		}

		protected static string AppendParameter(ColumnInfo columnInfo, string value, string place) {
			if (columnInfo == null) return "";

			string returnValue = place + string.Format("GetParameter(\"{0}{1}\", MySqlDbType.{2}, {3}, {4}), \r\n",
				columnInfo.Name.StartsWith("?") ? null : "?", columnInfo.Name, columnInfo.Type.ToString().Replace("Datetime", "DateTime").Replace("Geometry", "Text"),
				columnInfo.Type == MySqlDbType.Geometry ? "-1" : columnInfo.Length.ToString(),
				//columnInfo.Type == MySqlDbType.Image ? string.Format("{0} == null ? 0 : {0}.Length", value + Lib.UFString(columnInfo.Name)) : columnInfo.Length.ToString(),
				value + CodeBuild.UFString(columnInfo.Name) + (columnInfo.Type == MySqlDbType.Enum || columnInfo.Type == MySqlDbType.Set ? "?.ToInt64()" : (
					columnInfo.Type == MySqlDbType.Geometry ? "ToGeometry()?.AsText()" : "")));

			return returnValue;
		}
		protected static string AppendParameters(List<ColumnInfo> columnInfos, string value, string place) {
			string returnValue = "";

			foreach (ColumnInfo columnInfo in columnInfos) {
				returnValue += AppendParameter(columnInfo, value, place);
			}

			return returnValue == "" ? "" : returnValue.Substring(0, returnValue.Length - 4);
		}
		protected static string AppendParameters(List<ColumnInfo> columnInfos, string place) {
			return AppendParameters(columnInfos, "", place);
		}
		protected static string AppendParameters(TableInfo table, string place) {
			return AppendParameters(table.Columns, "item.", place);
		}
		protected static string AppendParameters(ColumnInfo columnInfo, string value, string place) {
			string returnValue = AppendParameter(columnInfo, value, place);
			return returnValue == "" ? "" : returnValue.Substring(0, returnValue.Length - 4);
		}

		protected static string AppendAddslashes(ColumnInfo columnInfo, string value, string place) {
			if (columnInfo == null) return "";

			string returnValue = place + value + CodeBuild.UFString(columnInfo.Name) + ", ";

			return returnValue;
		}
		protected static string AppendAddslashes(List<ColumnInfo> columnInfos, string value, string place) {
			string returnValue = "";

			foreach (ColumnInfo columnInfo in columnInfos) {
				returnValue += AppendAddslashes(columnInfo, value, place);
			}

			return returnValue == "" ? "" : returnValue.Substring(0, returnValue.Length - 2);
		}
		protected static string AppendAddslashes(List<ColumnInfo> columnInfos, string place) {
			return AppendAddslashes(columnInfos, "", place);
		}
		protected static string AppendAddslashes(TableInfo table, string place) {
			return AppendAddslashes(table.Columns, "item.", place);
		}
		protected static string AppendAddslashes(ColumnInfo columnInfo, string place) {
			string returnValue = AppendParameter(columnInfo, "", place);
			return returnValue == "" ? "" : returnValue.Substring(0, returnValue.Length - 2);
		}
	}
}
