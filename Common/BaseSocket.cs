/**********************************************************************************
 *
 * 此文件代码由 NicPetShop.exe 自动生成，您没有必要修改它或删除它
 * NicPetShop.exe 能将数据库的关系映射到 c#，让您使用更方便，您无需要担心它的性能
 * NicPetShop.exe 将永久免费给大家使用
 *
 * Author：	Nic
 * QQ：		2881099
 * Email：	kellynic@163.com
 * 帮助：	http://www.kellynic.com/default.asp?tag=NicPetShop
 *
 **********************************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class BaseSocket {

	protected void Write(Stream stream, SocketMessager messager) {
		MemoryStream ms = new MemoryStream();
		byte[] buff = Encoding.UTF8.GetBytes(messager.GetCanParseString());
		ms.Write(buff, 0, buff.Length);
		if (messager.Arg != null) {
			buff = Deflate.Compress(BaseSocket.Serialize(messager.Arg));
			ms.Write(buff, 0, buff.Length);
		}
		this.Write(stream, ms.ToArray());
		ms.Close();
	}
	private void Write(Stream stream, byte[] data) {
		MemoryStream ms = new MemoryStream();
		byte[] buff = Encoding.UTF8.GetBytes(Convert.ToString(data.Length + 8, 16).PadRight(8));
		ms.Write(buff, 0, buff.Length);
		ms.Write(data, 0, data.Length);
		buff = ms.ToArray();
		ms.Close();
		stream.Write(buff, 0, buff.Length);
	}

	protected SocketMessager Read(Stream stream) {
		byte[] data = new byte[8];
		int bytes = 0;
		int overs = data.Length;
		string size = string.Empty;
		while (overs > 0) {
			bytes = stream.Read(data, 0, overs);
			overs -= bytes;
			size += Encoding.UTF8.GetString(data, 0, bytes);
		}

		if (int.TryParse(size, NumberStyles.HexNumber, null, out overs) == false) {
			return null;
		}
		overs -= data.Length;
		MemoryStream ms = new MemoryStream();
		data = new Byte[1024];
		while (overs > 0) {
			bytes = stream.Read(data, 0, overs < data.Length ? overs : data.Length);
			overs -= bytes;
			ms.Write(data, 0, bytes);
		}
		data = ms.ToArray();
		ms.Close();
		return SocketMessager.Parse(data);
	}

	public static int findBytes(byte[] source, byte[] find, int startIndex) {
		if (find == null) return -1;
		if (find.Length == 0) return -1;
		if (source == null) return -1;
		if (source.Length == 0) return -1;
		if (startIndex < 0) startIndex = 0;
		int idx = -1, idx2 = startIndex - 1;
		do {
			idx2 = idx = Array.FindIndex<byte>(source, Math.Min(idx2 + 1, source.Length), delegate(byte b) {
				return b == find[0];
			});
			if (idx2 != -1) {
				for (int a = 1; a < find.Length; a++) {
					if (++idx2 >= source.Length || source[idx2] != find[a]) {
						idx = -1;
						break;
					}
				}
				if (idx != -1) break;
			}
		} while (idx2 != -1);
		return idx;
	}

	public static byte[] Serialize(object obj) {
		IFormatter formatter = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		formatter.Serialize(ms, obj);
		byte[] data = ms.ToArray();
		ms.Close();
		return data;
	}
	public static object Deserialize(byte[] stream) {
		IFormatter formatter = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(stream);
		object obj = formatter.Deserialize(ms);
		ms.Close();
		return obj;
	}
}

public class SocketMessager {
	private static int _identity;
	public static readonly SocketMessager SYS_TEST_LINK = new SocketMessager("\0");
	public static readonly SocketMessager SYS_HELLO_WELCOME = new SocketMessager("Hello, Welcome!");
	public static readonly SocketMessager SYS_ACCESS_DENIED = new SocketMessager("Access Denied.");

	private int _id;
	public bool _isChangeId;
	private string _action;
	private string _permission;
	private DateTime _remoteTime;
	private object _arg;
	private Exception _exception;

	public SocketMessager(string action)
		: this(action, null, null) {
	}
	public SocketMessager(string action, object arg)
		: this(action, null, arg) {
	}
	public SocketMessager(string action, string permission, object arg) {
		this._id = Interlocked.Increment(ref _identity);
		this._action = action == null ? string.Empty : action;
		this._permission = permission == null ? string.Empty : permission;
		this._arg = arg;
		this._remoteTime = DateTime.Now;
	}

	public override string ToString() {
		return
			this._remoteTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
			this._id + "\t" +
			this._action.Replace("\t", "\\t") + "\t" +
			this._permission.Replace("\t", "\\t") + "\t" +
			this._arg;
	}

	public string GetCanParseString() {
		if (string.Compare(this._action, SocketMessager.SYS_TEST_LINK.Action) == 0) {
			return this.Action;
		} else if (
			string.Compare(this._action, SocketMessager.SYS_HELLO_WELCOME.Action) == 0 ||
			string.Compare(this._action, SocketMessager.SYS_ACCESS_DENIED.Action) == 0) {
			return
				this._id + "\t" +
				this.Action + "\r\n";
		} else {
			return
				this._id + "\t" +
				this._action.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r\n", "\\n") + "\t" +
				this._permission.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r\n", "\\n") + "\t" +
				this._remoteTime.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
		}
	}

	public static SocketMessager Parse(byte[] data) {
		if (data == null) return new SocketMessager("NULL");
		if (data.Length == 1 && data[0] == 0) return SocketMessager.SYS_TEST_LINK;
		int idx = BaseSocket.findBytes(data, new byte[] { 13, 10 }, 0);
		string text = Encoding.UTF8.GetString(data, 0, idx);
		string[] loc1 = text.Split(new string[] { "\t" }, 4, StringSplitOptions.None);
		string loc2 = loc1[0];
		string loc3 = loc1.Length > 1 ? loc1[1].Replace("\\\\", "\\").Replace("\\t", "\t").Replace("\\n", "\r\n") : null;
		string loc4 = loc1.Length > 2 ? loc1[2].Replace("\\\\", "\\").Replace("\\t", "\t").Replace("\\n", "\r\n") : null;
		string loc5 = loc1.Length > 3 ? loc1[3] : null;
		MemoryStream ms = new MemoryStream();
		ms.Write(data, idx + 2, data.Length - idx - 2);
		SocketMessager messager = new SocketMessager(loc3, loc4,
			ms.Length > 0 ? BaseSocket.Deserialize(Deflate.Decompress(ms.ToArray())) : null);
		if (int.TryParse(loc2, out idx)) messager._id = idx;
		if (!string.IsNullOrEmpty(loc5)) DateTime.TryParse(loc5, out messager._remoteTime);
		if (messager._arg is Exception) messager._exception = messager._arg as Exception;
		return messager;
	}

	/// <summary>
	/// 服务端为 -，客户端为 +
	/// </summary>
	public int Id {
		get { return _id; }
		set {
			if (_id != value) {
				_isChangeId = true;
			}
			_id = value;
		}
	}
	public string Action {
		get { return _action; }
	}
	public string Permission {
		get { return _permission; }
	}
	public DateTime RemoteTime {
		get { return _remoteTime; }
	}
	public object Arg {
		get { return _arg; }
	}
	public Exception Exception {
		get { return _exception; }
	}
}