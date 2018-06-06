using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public delegate void AnonymousHandler();

/// <summary>
/// 常用函数库
/// </summary>
public class Lib {

	public static long Ip2Long(string ip) {
		char[] separator = new char[] { '.' };
		string[] items = ip.Split(separator);
		return long.Parse(items[0]) << 24
				| long.Parse(items[1]) << 16
				| long.Parse(items[2]) << 8
				| long.Parse(items[3]);
	}
	public static string Long2Ip(long ipInt) {
		StringBuilder sb = new StringBuilder();
		sb.Append((ipInt >> 24) & 0xFF).Append(".");
		sb.Append((ipInt >> 16) & 0xFF).Append(".");
		sb.Append((ipInt >> 8) & 0xFF).Append(".");
		sb.Append(ipInt & 0xFF);
		return sb.ToString();
	}

	#region WebUtility
	public static string HtmlEncode(object input) { return WebUtility.HtmlEncode(string.Concat(input)); }
	public static string HtmlDecode(object input) { return WebUtility.HtmlDecode(string.Concat(input)); }
	public static string UrlEncode(object input) { return WebUtility.UrlEncode(string.Concat(input)); }
	public static string UrlDecode(object input) { return WebUtility.UrlDecode(string.Concat(input)); }

	public static string JSDecode(string input) { return JSDecoder.Decode(input); }
	#endregion

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
		using (MemoryStream ms = new MemoryStream()) {
			DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(object));
			js.WriteObject(ms, obj);
			return ms.ToArray();
		}
	}
	public static object Deserialize(byte[] stream) {
		using (MemoryStream ms = new MemoryStream(stream)) {
			DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(object));
			return js.ReadObject(ms);
		}
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

	static HttpClientHandler clientsslHandler;
	static HttpClient clientssl;
	static HttpClient client;
	static Lib() {
		clientsslHandler = new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic };
		clientsslHandler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
		clientssl = new HttpClient(clientsslHandler);
		client = new HttpClient();
	}

	public static JToken HttpsPost(string url, object postData) {
		StringContent data = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData));
		data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		return Newtonsoft.Json.JsonConvert.DeserializeObject(clientssl.PostAsync(url, data).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult()) as JToken;
	}
	public static JToken HttpPost(string url, object postData) {
		StringContent data = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData));
		data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		return Newtonsoft.Json.JsonConvert.DeserializeObject(client.PostAsync(url, data).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult()) as JToken;
	}
	public static JToken HttpsGet(string url) {
		return Newtonsoft.Json.JsonConvert.DeserializeObject(clientssl.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult()) as JToken;
	}
	public static JToken HttpGet(string url) {
		return Newtonsoft.Json.JsonConvert.DeserializeObject(client.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult()) as JToken;
	}
	public static byte[] HttpsGetBytes(string url) {
		return clientssl.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
	}
	public static byte[] HttpGetBytes(string url) {
		return client.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
	}
	public static byte[] HttpsPostBytes(string url, object postData) {
		StringContent data = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData));
		data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		return clientssl.PostAsync(url, data).GetAwaiter().GetResult().Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
	}
	public static byte[] HttpPostBytes(string url, object postData) {
		StringContent data = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData));
		data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		return client.PostAsync(url, data).GetAwaiter().GetResult().Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
	}

	public static string GenerateNumberCode(int size) {
		string ret = "";
		for (int a = 0; a < size; a++) ret += new Random().Next(0, 10);
		return ret;
	}

	public static object Hash_HMAC(string signatureString, string secretKey, bool raw_output = false) {
		HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
		hmac.Initialize();
		byte[] buffer = Encoding.UTF8.GetBytes(signatureString);
		if (raw_output) return hmac.ComputeHash(buffer);
		return BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
	}

	public static string SHA1(string text, Encoding encode) {
		try {
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			byte[] bytes_in = encode.GetBytes(text);
			byte[] bytes_out = sha1.ComputeHash(bytes_in);
			sha1.Dispose();
			string result = BitConverter.ToString(bytes_out);
			result = result.Replace("-", "");
			return result.ToLower();
		} catch (Exception ex) {
			throw new Exception("SHA1加密出错：" + ex.Message);
		}
	}

	public static string MD5(string source) {
		byte[] sor = Encoding.UTF8.GetBytes(source);
		MD5 md5 = System.Security.Cryptography.MD5.Create();
		byte[] result = md5.ComputeHash(sor);
		StringBuilder strbul = new StringBuilder(40);
		for (int i = 0; i < result.Length; i++)
			strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
		return strbul.ToString();
	}

	public static string AESEncrypt(string text, byte[] key, byte[] iv) {
		RijndaelManaged rijndaelCipher = new RijndaelManaged();
		rijndaelCipher.Mode = CipherMode.CBC;
		rijndaelCipher.Padding = PaddingMode.PKCS7;
		rijndaelCipher.KeySize = 128;
		rijndaelCipher.BlockSize = 128;
		byte[] keyBytes = new byte[16];
		Array.Copy(key, keyBytes, Math.Min(keyBytes.Length, key.Length));
		rijndaelCipher.Key = keyBytes;
		byte[] ivBytes = new byte[16];
		Array.Copy(iv, ivBytes, Math.Min(ivBytes.Length, iv.Length));
		rijndaelCipher.IV = ivBytes;
		ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
		byte[] plainText = Encoding.UTF8.GetBytes(text);
		byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
		return Convert.ToBase64String(cipherBytes);
	}
	public static string AESDecrypt(string base64_text, byte[] key, byte[] iv) {
		RijndaelManaged rijndaelCipher = new RijndaelManaged();
		rijndaelCipher.Mode = CipherMode.CBC;
		rijndaelCipher.Padding = PaddingMode.PKCS7;
		rijndaelCipher.KeySize = 128;
		rijndaelCipher.BlockSize = 128;
		byte[] encryptedData = Convert.FromBase64String(base64_text);
		byte[] keyBytes = new byte[16];
		Array.Copy(key, keyBytes, Math.Min(keyBytes.Length, key.Length));
		rijndaelCipher.Key = keyBytes;
		byte[] ivBytes = new byte[16];
		Array.Copy(iv, ivBytes, Math.Min(ivBytes.Length, iv.Length));
		rijndaelCipher.IV = ivBytes;
		ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
		byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
		return Encoding.UTF8.GetString(plainText);
	}
}