using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

public static class GlobalExtensions {
	public static object Json(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, object obj) {
		string str = JsonConvert.SerializeObject(obj);
		if (!string.IsNullOrEmpty(str)) str = Regex.Replace(str, @"<(/?script[\s>])", "<\"+\"$1", RegexOptions.IgnoreCase);
		if (html == null) return str;
		return html.Raw(str);
	}

	/// <summary>
	/// 转格林时间，并以ISO8601格式化字符串
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public static string ToGmtISO8601(this DateTime time) {
		return time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
	}

	/// <summary>
	/// 获取时间戳，按1970-1-1
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public static long GetTime(this DateTime time) {
		return (long)time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
	}

	static DateTime dt19700101 = new DateTime(1970, 1, 1);
	/// <summary>
	/// 获取时间戳毫秒数，按1970-1-1
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public static long GetTimeMilliseconds(this DateTime time) {
		return (long)time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
	}
}
