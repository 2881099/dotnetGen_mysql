using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PList;
using System.Runtime.Serialization.Formatters.Binary;

public delegate void AnonymousHandler();

/// <summary>
/// 常用函数库
/// </summary>
public class Lib {

	/// <summary>
	/// 当前程序类型是否为 Web Application
	/// </summary>

	public static string HtmlEncode(object input) { return WebUtility.HtmlEncode(string.Concat(input)); }
	public static string HtmlDecode(object input) { return WebUtility.HtmlDecode(string.Concat(input)); }
	public static string UrlEncode(object input) { return WebUtility.UrlEncode(string.Concat(input)); }
	public static string UrlDecode(object input) { return WebUtility.UrlDecode(string.Concat(input)); }

	public static string JSDecode(string input) { return JSDecoder.Decode(input); }

	#region 弥补 String.PadRight 和 String.PadLeft 对中文的 Bug
	public static string PadRight(object text, int length) { return PadRightLeft(text, length, ' ', true); }
	public static string PadRight(object text, char paddingChar, int length) { return PadRightLeft(text, length, paddingChar, true); }
	public static string PadLeft(object text, int length) { return PadRightLeft(text, length, ' ', false); }
	public static string PadLeft(object text, char paddingChar, int length) { return PadRightLeft(text, length, paddingChar, false); }
	static string PadRightLeft(object text, int length, char paddingChar, bool isRight) {
		string str = string.Concat(text);
		int len2 = Encoding.UTF8.GetBytes(str).Length;
		for (int a = 0; a < length - len2; a++) if (isRight) str += paddingChar; else str = paddingChar + str;
		return str;
	}
	#endregion

	#region 序列化/反序列化(二进制)
	public static byte[] Serialize(object obj) {
		var formatter = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		formatter.Serialize(ms, obj);
		byte[] data = ms.ToArray();
		ms.Close();
		return data;
		//using (MemoryStream ms = new MemoryStream()) {
		//	DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(object));
		//	js.WriteObject(ms, obj);
		//	return ms.ToArray();
		//}
	}
	public static object Deserialize(byte[] stream) {
		var formatter = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(stream);
		object obj = formatter.Deserialize(ms);
		ms.Close();
		return obj;
		//using (MemoryStream ms = new MemoryStream(stream)) {
		//	DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(object));
		//	return js.ReadObject(ms);
		//}
	}
	#endregion

	/// <summary>
	/// 将 plist 格式的 xml 内容，解析成 Newtowsoft.Json JObject 对象来访问
	/// </summary>
	/// <param name="plist">plist 格式的 xml 内容</param>
	/// <returns>JObject</returns>
	public static JObject ParsePListToJson(string plist) {
		NSObject obj = XmlPropertyListParser.Parse(Encoding.UTF8.GetBytes(plist));
		string json = JsonConvert.SerializeObject(obj.ToObject());
		return JsonConvert.DeserializeObject<JObject>(json);
	}

	/// <summary>
	/// 重试某过程 maxError 次，直到成功或失败
	/// </summary>
	/// <param name="handler">托管函数</param>
	/// <param name="maxError">允许失败的次数</param>
	/// <returns>如果执行成功，则返回 null, 否则返回该错误对象</returns>
	public static Exception Trys(AnonymousHandler handler, int maxError) {
		if (handler != null) {
			Exception ex = null;
			for (int a = 0; a < maxError; a++) {
				try {
					handler();
					return null;
				} catch (Exception e) {
					ex = e;
				}
			}
			return ex;
		}
		return null;
	}

	/// <summary>
	/// 延迟 milliSecond 毫秒后执行 handler，与 javascript 里的 setTimeout 相似
	/// </summary>
	/// <param name="handler">托管函数</param>
	/// <param name="milliSecond">毫秒</param>
	public static void SetTimeout(AnonymousHandler handler, int milliSecond) {
		new Timer((a) => {
			try {
				handler();
			} catch { }
		}, null, milliSecond, milliSecond);
	}

	/// <summary>
	/// 将服务器端数据转换成安全的JS字符串
	/// </summary>
	/// <param name="input">一个服务器端变量或字符串</param>
	/// <returns>安全的JS字符串</returns>
	public static string GetJsString(object input) {
		if (input == null) return string.Empty;
		return string.Concat(input).Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'");
	}

	static Dictionary<string, Type> _InvokeMethod_cache_type = new Dictionary<string, Type>();
	static object _InvokeMethod_cache_type_lock = new object();
	public static object InvokeMethod(string typeName, string method, params object[] parms) {
		Type type;
		if (!_InvokeMethod_cache_type.TryGetValue(typeName, out type)) {
			type = System.Type.GetType(typeName);
			lock (_InvokeMethod_cache_type_lock) {
				if (!_InvokeMethod_cache_type.TryGetValue(typeName, out type))
					_InvokeMethod_cache_type.Add(typeName, type);
			}
		}
		return type.GetMethod(method).Invoke(null, parms);
	}

	/// <summary>
	/// 获取对象属性
	/// </summary>
	/// <param name="obj">对象</param>
	/// <param name="property">属性，此属性可为多级属性，如：newsInfo.newsClassInfo...</param>
	/// <returns>对象的（子）属性</returns>
	public static object EvaluateValue(object obj, string property) {
		if (obj == null) return null;
		string prop = property;
		object ret = string.Empty;
		if (property.Contains(".")) {
			prop = property.Substring(0, property.IndexOf("."));
			PropertyInfo propa = EvaluateValue_GetProperty(obj, prop);
			if (propa != null) {
				object obja = propa.GetValue(obj, null);
				ret = EvaluateValue(obja, property.Substring(property.IndexOf(".") + 1));
			}
		} else {
			PropertyInfo propa = EvaluateValue_GetProperty(obj, prop);
			if (propa != null) {
				ret = propa.GetValue(obj, null);
			}
		}
		return ret;
	}
	private static PropertyInfo EvaluateValue_GetProperty(object obj, string property) {
		if (obj == null) return null;
		Type type = obj.GetType();
		PropertyInfo ret = type.GetProperty(property);
		if (ret == null) {
			PropertyInfo[] pis = type.GetProperties();
			foreach (PropertyInfo pi in pis) {
				if (string.Compare(pi.Name, property, true) == 0) {
					ret = pi;
					break;
				}
			}
		}
		return ret;
	}

	/// <summary>
	/// (安全转换)对象/值转换类型
	/// </summary>
	/// <typeparam name="T">转换后的类型</typeparam>
	/// <param name="input">转换的对象</param>
	/// <returns>转换后的对象/值</returns>
	public static T ConvertTo<T>(object input) {
		return ConvertTo<T>(input, default(T));
	}
	public static T ConvertTo<T>(object input, T defaultValue) {
		if (input == null) return defaultValue;
		object obj = null;

		if (defaultValue is System.Byte ||
			defaultValue is System.Decimal ||

			defaultValue is System.Int16 ||
			defaultValue is System.Int32 ||
			defaultValue is System.Int64 ||
			defaultValue is System.SByte ||
			defaultValue is System.Single ||

			defaultValue is System.UInt16 ||
			defaultValue is System.UInt32 ||
			defaultValue is System.UInt64) {
			decimal trydec = 0;
			if (decimal.TryParse(string.Concat(input), out trydec)) obj = trydec;
		} else {
			if (defaultValue is System.DateTime) {
				DateTime trydt = DateTime.Now;
				if (DateTime.TryParse(string.Concat(input), out trydt)) obj = trydt;
			} else {
				if (defaultValue is System.Boolean) {
					bool trybool = false;
					if (bool.TryParse(string.Concat(input), out trybool)) obj = trybool;
				} else {
					if (defaultValue is System.Double) {
						double trydb = 0;
						if (double.TryParse(string.Concat(input), out trydb)) obj = trydb;
					} else {
						obj = input;
					}
				}
			}
		}

		try {
			if (obj != null) return (T)Convert.ChangeType(obj, typeof(T));
		} catch { }

		return defaultValue;
	}
}