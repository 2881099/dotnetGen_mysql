using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

public class IniHelper {
	private static Dictionary<string, object> _cache = new Dictionary<string, object>();
	private static Dictionary<string, FileSystemWatcher> _watcher = new Dictionary<string, FileSystemWatcher>();
	private static object _lock = new object();

	private static object loadAndCache(string path) {
		path = TranslateUrl(path);
		object ret = null;
		if (!_cache.TryGetValue(path, out ret)) {
			object value2 = LoadIniNotCache(path);
			string dir = Path.GetDirectoryName(path);
			string name = Path.GetFileName(path);
			FileSystemWatcher fsw = new FileSystemWatcher(dir, name);
			fsw.IncludeSubdirectories = false;
			fsw.Changed += watcher_handler;
			fsw.Renamed += watcher_handler;
			fsw.EnableRaisingEvents = false;
			lock (_lock) {
				if (!_cache.TryGetValue(path, out ret)) {
					_cache.Add(path, ret = value2);
					_watcher.Add(path, fsw);
					fsw.EnableRaisingEvents = true;
				} else {
					fsw.Dispose();
				}
			}
		}
		return ret;
	}
	private static void watcher_handler(object sender, FileSystemEventArgs e) {
		lock (_lock) {
			_cache.Remove(e.FullPath);
			FileSystemWatcher fsw = null;
			if (_watcher.TryGetValue(e.FullPath, out fsw)) {
				fsw.EnableRaisingEvents = false;
				fsw.Dispose();
			}
		}
	}

	public static Dictionary<string, NameValueCollection> LoadIni(string path) {
		return loadAndCache(path) as Dictionary<string, NameValueCollection>;
	}
	public static Dictionary<string, NameValueCollection> LoadIniNotCache(string path) {
		Dictionary<string, NameValueCollection> ret = new Dictionary<string, NameValueCollection>();
		string[] lines = ReadTextFile(path).Split(new string[] { "\n" }, StringSplitOptions.None);
		string key = "";
		foreach (string line2 in lines) {
			string line = line2.Trim();
			if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith(";")) continue;

			Match m = Regex.Match(line, @"^\[([^\]]+)\]$");
			if (m.Success) {
				key = m.Groups[1].Value;
				continue;
			}
			if (!ret.ContainsKey(key)) ret.Add(key, new NameValueCollection());
			string[] kv = line.Split(new char[] { '=' }, 2);
			if (!string.IsNullOrEmpty(kv[0])) {
				ret[key][kv[0]] = kv.Length > 1 ? kv[1] : null;
			}
		}
		return ret;
	}

	public static string ReadTextFile(string path) {
		byte[] bytes = ReadFile(path);
		return Encoding.UTF8.GetString(bytes).TrimStart((char)65279);
	}
	public static byte[] ReadFile(string path) {
		if (File.Exists(path)) {
			string destFileName = Path.GetTempFileName();
			File.Copy(path, destFileName, true);
			int read = 0;
			byte[] data = new byte[1024];
			using (MemoryStream ms = new MemoryStream()) {
				using (FileStream fs = new FileStream(destFileName, FileMode.OpenOrCreate, FileAccess.Read)) {
					do {
						read = fs.Read(data, 0, data.Length);
						if (read <= 0) break;
						ms.Write(data, 0, read);
					} while (true);
				}
				File.Delete(destFileName);
				data = ms.ToArray();
			}
			return data;
		}
		return new byte[] { };
	}

	public static string TranslateUrl(string url) {
		return TranslateUrl(url, null);
	}
	public static string TranslateUrl(string url, string baseDir) {
		if (string.IsNullOrEmpty(baseDir)) baseDir = AppContext.BaseDirectory + "/";
		if (string.IsNullOrEmpty(url)) return Path.GetDirectoryName(baseDir);
		if (url.StartsWith("~/")) url = url.Substring(1);
		if (url.StartsWith("/")) return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(baseDir), url.TrimStart('/')));
		if (url.StartsWith("\\")) return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(baseDir), url.TrimStart('\\')));
		if (url.IndexOf(":\\") != -1) return url;
		return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(baseDir), url));
	}
}