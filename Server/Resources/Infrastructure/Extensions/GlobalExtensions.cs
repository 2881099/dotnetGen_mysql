using Newtonsoft.Json;
using System.Text.RegularExpressions;

public static class GlobalExtensions {
	public static object Json(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, object obj) {
		string str = JsonConvert.SerializeObject(obj);
		if (!string.IsNullOrEmpty(str)) str = Regex.Replace(str, @"<(/?script[\s>])", "<\"+\"$1", RegexOptions.IgnoreCase);
		if (html == null) return str;
		return html.Raw(str);
	}
}
