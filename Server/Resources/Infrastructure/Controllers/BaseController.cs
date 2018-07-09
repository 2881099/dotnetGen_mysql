using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

[ServiceFilter(typeof(CustomExceptionFilter)), EnableCors("cors_all")]
public partial class BaseController : Controller {
	public ILogger _logger;
	public ISession Session { get { return HttpContext.Session; } }
	public HttpRequest Req { get { return Request; } }
	public HttpResponse Res { get { return Response; } }

	public string Ip => this.Request.Headers["X-Real-IP"].FirstOrDefault() ?? this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
	public IConfiguration Configuration => (IConfiguration) HttpContext.RequestServices.GetService(typeof(IConfiguration));
	//public SysuserInfo LoginUser { get; private set; }
	public BaseController(ILogger logger) { _logger = logger; }
	public override void OnActionExecuting(ActionExecutingContext context) {
		#region 参数验证
		if (context.ModelState.IsValid == false)
			foreach (var value in context.ModelState.Values)
				if (value.Errors.Any()) {
					context.Result = APIReturn.参数格式不正确.SetMessage($"参数格式不正确：{value.Errors.First().ErrorMessage}");
					return;
				}
		#endregion
		#region 初始化当前登陆账号
		//string username = Session.GetString("login.username");
		//if (!string.IsNullOrEmpty(username)) LoginUser = Sysuser.GetItemByUsername(username);

		//var method = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo;
		//if (method.GetCustomAttribute<需要登陆Attribute>() != null && LoginUser == null)
		//	context.Result = new RedirectResult("/signin");
		//else if (method.GetCustomAttribute<匿名访问Attribute>() == null && LoginUser == null)
		//	context.Result = new RedirectResult("/signin");
		//ViewBag.user = LoginUser;
		#endregion
		base.OnActionExecuting(context);
	}
	public override void OnActionExecuted(ActionExecutedContext context) {
		base.OnActionExecuted(context);
	}

	#region 角色权限验证
	//public bool sysrole_check(string url) {
	//	url = url.ToLower();
	//	//Response.Write(url + "<br>");
	//	if (url == "/" || url.IndexOf("/default.aspx") == 0) return true;
	//	foreach(var role in this.LoginUser.Obj_sysroles) {
	//		//Response.Write(role.ToString());
	//		foreach(var dir in role.Obj_sysdirs) {
	//			//Response.Write("-----------------" + dir.ToString() + "<br>");
	//			string tmp = dir.Url;
	//			if (tmp.EndsWith("/")) tmp += "default.aspx";
	//			if (url.IndexOf(tmp) == 0) return true;
	//		}
	//	}
	//	return false;
	//}
	#endregion
}

#region 需要登陆、匿名访问
public partial class 需要登陆Attribute : Attribute { }
public partial class 匿名访问Attribute : Attribute { }
#endregion

#region APIReturn
[JsonObject(MemberSerialization.OptIn)]
public partial class APIReturn : ContentResult {
	[JsonProperty("code")] public int Code { get; protected set; }
	[JsonProperty("message")] public string Message { get; protected set; }
	[JsonProperty("data")] public Hashtable Data { get; protected set; } = new Hashtable();
	[JsonProperty("success")] public bool Success { get { return this.Code == 0; } }

	public APIReturn() { }
	public APIReturn(int code) { this.SetCode(code); }
	public APIReturn(string message) { this.SetMessage(message); }
	public APIReturn(int code, string message, params object[] data) { this.SetCode(code).SetMessage(message).AppendData(data); }

	public APIReturn SetCode(int value) { this.Code = value;  return this; }
	public APIReturn SetMessage(string value) { this.Message = value;  return this; }
	public APIReturn SetData(params object[] value) {
		this.Data.Clear();
		return this.AppendData(value);
	}
	public APIReturn AppendData(params object[] value) {
		if (value == null || value.Length < 2 || value[0] == null) return this;
		for (int a = 0; a < value.Length; a += 2) {
			if (value[a] == null) continue;
			this.Data[value[a]] = a + 1 < value.Length ? value[a + 1] : null;
		}
		return this;
	}
	#region form 表单 target=iframe 提交回调处理
	private void Jsonp(ActionContext context) {
		string __callback = context.HttpContext.Request.HasFormContentType ? context.HttpContext.Request.Form["__callback"].ToString() : null;
		if (string.IsNullOrEmpty(__callback)) {
			this.ContentType = "text/json;charset=utf-8;";
			this.Content = JsonConvert.SerializeObject(this);
		}else {
			this.ContentType = "text/html;charset=utf-8";
			this.Content = $"<script>top.{__callback}({GlobalExtensions.Json(null, this)});</script>";
		}
	}
	public override void ExecuteResult(ActionContext context) {
		Jsonp(context);
		base.ExecuteResult(context);
	}
	public override Task ExecuteResultAsync(ActionContext context) {
		Jsonp(context);
		return base.ExecuteResultAsync(context);
	}
	#endregion

	public static APIReturn 成功 { get { return new APIReturn(0, "成功"); } }
	public static APIReturn 失败 { get { return new APIReturn(99, "失败"); } }
	public static APIReturn 记录不存在_或者没有权限 { get { return new APIReturn(98, "记录不存在，或者没有权限"); } }
	public static APIReturn 参数格式不正确 { get { return new APIReturn(97, "参数格式不正确"); } }
}
#endregion
