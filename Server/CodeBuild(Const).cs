using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Model;

namespace Server {

	internal partial class CodeBuild {

		protected class CONST {
			public static readonly string corePath = @"src\";
			public static readonly string moduleAdminPath = @"src\Module\Admin\";
			public static readonly string webHostPath = @"src\WebHost\";
			public static readonly string sln =
			#region 内容太长已被收起
 @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 14
VisualStudioVersion = 14.0.25420.1
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""src"", ""src"", ""{{{1}}}""
EndProject
Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""Solution Items"", ""Solution Items"", ""{{{2}}}""
	ProjectSection(SolutionItems) = preProject
		build.bat = build.bat
		readme.md = readme.md
	EndProjectSection
EndProject
Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""Module"", ""Module"", ""{{{3}}}""
EndProject
Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""Test"", ""Test"", ""{{{4}}}""
EndProject
Project(""{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}"") = ""{0}.db"", ""src\{0}.db\{0}.db.csproj"", ""{{{6}}}""
EndProject
Project(""{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}"") = ""Infrastructure"", ""src\Infrastructure\Infrastructure.csproj"", ""{{{7}}}""
EndProject
Project(""{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}"") = ""WebHost"", ""src\WebHost\WebHost.csproj"", ""{{{8}}}""
EndProject
Project(""{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}"") = ""Admin"", ""src\Module\Admin\Admin.csproj"", ""{{{9}}}""
EndProject
Project(""{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}"") = ""Test"", ""src\Module\Test\Test.csproj"", ""{{{10}}}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{{{6}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{6}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{6}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{6}}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{{7}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{7}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{7}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{7}}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{{8}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{8}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{8}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{8}}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{{9}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{9}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{9}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{9}}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{{10}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{10}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{10}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{10}}}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{{{3}}} = {{{1}}}
		{{{6}}} = {{{1}}}
		{{{7}}} = {{{1}}}
		{{{8}}} = {{{1}}}
		{{{9}}} = {{{3}}}
		{{{10}}} = {{{3}}}
	EndGlobalSection
EndGlobal
";
			#endregion

			public static readonly string DAL_DBUtility_SqlHelper_cs =
			#region 内容太长已被收起
 @"using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;

namespace {0}.BLL {{
	/// <summary>
	/// 数据库操作代理类，全部支持走事务
	/// </summary>
	public abstract partial class SqlHelper : {0}.DAL.SqlHelper {{
	}}
}}
namespace {0}.DAL {{
	public abstract partial class SqlHelper {{
		private static string _connectionString;
		public static string ConnectionString {{
			get {{
				if (string.IsNullOrEmpty(_connectionString)) _connectionString = BLL.RedisHelper.Configuration[""ConnectionStrings:MySql""];
				return _connectionString;
			}}
			set {{
				_connectionString = value;
				Instance.Pool.ConnectionString = value;
			}}
		}}
		public static Executer Instance {{ get; }} = new Executer(new LoggerFactory().CreateLogger(""{0}_DAL_sqlhelper""), ConnectionString);

		public static string Addslashes(string filter, params object[] parms) {{ return Executer.Addslashes(filter, parms); }}

		public static void ExecuteReader(Action<MySqlDataReader> readerHander, string cmdText, params MySqlParameter[] cmdParms) => Instance.ExecuteReader(readerHander, CommandType.Text, cmdText, cmdParms);
		public static object[][] ExeucteArray(string cmdText, params MySqlParameter[] cmdParms) => Instance.ExeucteArray(CommandType.Text, cmdText, cmdParms);
		public static int ExecuteNonQuery(string cmdText, params MySqlParameter[] cmdParms) => Instance.ExecuteNonQuery(CommandType.Text, cmdText, cmdParms);
		public static object ExecuteScalar(string cmdText, params MySqlParameter[] cmdParms) => Instance.ExecuteScalar(CommandType.Text, cmdText, cmdParms);

		public static Task ExecuteReaderAsync(Func<MySqlDataReader, Task> readerHander, string cmdText, params MySqlParameter[] cmdParms) => Instance.ExecuteReaderAsync(readerHander, CommandType.Text, cmdText, cmdParms);
		public static Task<object[][]> ExeucteArrayAsync(string cmdText, params MySqlParameter[] cmdParms) => Instance.ExeucteArrayAsync(CommandType.Text, cmdText, cmdParms);
		public static Task<int> ExecuteNonQueryAsync(string cmdText, params MySqlParameter[] cmdParms) => Instance.ExecuteNonQueryAsync(CommandType.Text, cmdText, cmdParms);
		public static Task<object> ExecuteScalarAsync(string cmdText, params MySqlParameter[] cmdParms) => Instance.ExecuteScalarAsync(CommandType.Text, cmdText, cmdParms);

		/// <summary>
		/// 开启事务（不支持异步），60秒未执行完将自动提交
		/// </summary>
		/// <param name=""handler"">事务体 () => {{}}</param>
		public static void Transaction(Action handler) {{
			Transaction(handler, TimeSpan.FromSeconds(60));
		}}
		/// <summary>
		/// 开启事务（不支持异步）
		/// </summary>
		/// <param name=""handler"">事务体 () => {{}}</param>
		/// <param name=""timeout"">超时，未执行完将自动提交</param>
		public static void Transaction(Action handler, TimeSpan timeout) {{
			try {{
				Instance.BeginTransaction(timeout);
				handler();
				Instance.CommitTransaction();
			}} catch (Exception ex) {{
				Instance.RollbackTransaction();
				throw ex;
			}}
		}}
	}}
}}";
			#endregion

			public static readonly string BLL_Build_ItemCache_cs =
			#region 内容太长已被收起
 @"using System;
using System.Collections.Generic;

namespace {0}.BLL {{
	public partial class ItemCache {{

		private static Dictionary<string, long> _dic1 = new Dictionary<string, long>();
		private static Dictionary<long, Dictionary<string, string>> _dic2 = new Dictionary<long, Dictionary<string, string>>();
		private static LinkedList<long> _linked = new LinkedList<long>();
		private static object _dic1_lock = new object();
		private static object _dic2_lock = new object();
		private static object _linked_lock = new object();

		public static void Clear() {{
			lock(_dic1_lock) {{
				_dic1.Clear();
			}}
			lock(_dic2_lock) {{
				_dic2.Clear();
			}}
			lock(_linked_lock) {{
				_linked.Clear();
			}}
		}}
		public static void Remove(string key) {{
			if (string.IsNullOrEmpty(key)) return;
			long time;
			if (_dic1.TryGetValue(key, out time) == false) return;

			lock (_dic1_lock) {{
				_dic1.Remove(key);
			}}
			if (_dic2.ContainsKey(time)) {{
				lock (_dic2_lock) {{
					_dic2.Remove(time);
				}}
			}}
			lock (_linked_lock) {{
				_linked.Remove(time);
			}}
		}}
		public static string Get(string key) {{
			if (string.IsNullOrEmpty(key)) return null;
			long time;
			if (_dic1.TryGetValue(key, out time) == false) return null;
			Dictionary<string, string> dic;
			if (_dic2.TryGetValue(time, out dic) == false) {{
				if (_dic1.ContainsKey(key)) {{
					lock (_dic1_lock) {{
						_dic1.Remove(key);
					}}
				}}
				return null;
			}}
			if (DateTime.Now.Subtract(new DateTime(2016, 5, 1)).TotalSeconds > time) {{
				if (_dic1.ContainsKey(key)) {{
					lock (_dic1_lock) {{
						_dic1.Remove(key);
					}}
				}}
				if (_dic2.ContainsKey(time)) {{
					lock (_dic2_lock) {{
						_dic2.Remove(time);
					}}
				}}
				lock (_linked_lock) {{
					_linked.Remove(time);
				}}
				return null;
			}}
			string ret;
			if (dic.TryGetValue(key, out ret) == false) return null;
			return ret;
		}}
		public static void Set(string key, string value, int expire) {{
			if (string.IsNullOrEmpty(key) || expire <= 0) return;
			long time_cur = (long)DateTime.Now.Subtract(new DateTime(2016, 5, 1)).TotalSeconds;
			long time = time_cur + expire;
			long time2;
			if (_dic1.TryGetValue(key, out time2) == false) {{
				lock (_dic1_lock) {{
					if (_dic1.TryGetValue(key, out time2) == false) {{
						_dic1.Add(key, time2 = time);
					}}
				}}
			}}
			if (time2 != time) {{
				lock (_dic1_lock) {{
					_dic1[key] = time;
				}}
				lock (_dic2_lock) {{
					_dic2.Remove(time2);
				}}
			}}
			Dictionary<string, string> dic;
			bool isNew = false;
			if (_dic2.TryGetValue(time, out dic) == false) {{
				lock (_dic2_lock) {{
					if (_dic2.TryGetValue(time, out dic) == false) {{
						_dic2.Add(time, dic = new Dictionary<string, string>());
						isNew = true;
					}}
					if (dic.ContainsKey(key) == false) dic.Add(key, value);
					else dic[key] = value;
				}}
			}} else {{
				lock (_dic2_lock) {{
					if (dic.ContainsKey(key) == false) dic.Add(key, value);
					else dic[key] = value;
				}}
			}}
			if (isNew == true) {{
				lock (_linked_lock) {{
					if (_linked.Count == 0) {{
						_linked.AddFirst(time);
					}} else {{
						LinkedListNode<long> node = _linked.First;
						while (node != null) {{
							if (node.Value < time_cur) {{
								_linked.Remove(node);
								Dictionary<string, string> dic_del;
								if (_dic2.TryGetValue(node.Value, out dic_del)) {{
									lock (_dic2_lock) {{
										_dic2.Remove(node.Value);
										foreach (KeyValuePair<string, string> dic_del_in in dic_del) {{
											if (_dic1.ContainsKey(dic_del_in.Key)) {{
												lock (_dic1_lock) {{
													_dic1.Remove(dic_del_in.Key);
												}}
											}}
										}}
									}}
								}}
								node = _linked.First;
							}} else break;
						}}
						if (node == null)
							_linked.AddFirst(time);
						else if (node != null && _linked.Last.Value < time)
							_linked.AddLast(time);
						else {{
							while (node != null && node.Value < time) node = node.Next;
							if (node != null && node.Value != time) {{
								_linked.AddBefore(node, time);
							}}
						}}
					}}
				}}
			}}
		}}
	}}
}}";
			#endregion
			public static readonly string BLL_Build_RedisHelper_cs =
			#region 内容太长已被收起
 @"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace {0}.BLL {{

	public partial class RedisHelper : CSRedis.QuickHelperBase {{
		public static IConfigurationRoot Configuration {{ get; internal set; }}
		public static void InitializeConfiguration(IConfigurationRoot cfg) {{
			Configuration = cfg;
			int port, poolsize, database;
			string ip, pass;
			if (!int.TryParse(cfg[""ConnectionStrings:redis:port""], out port)) port = 6379;
			if (!int.TryParse(cfg[""ConnectionStrings:redis:poolsize""], out poolsize)) poolsize = 50;
			if (!int.TryParse(cfg[""ConnectionStrings:redis:database""], out database)) database = 0;
			ip = cfg[""ConnectionStrings:redis:ip""];
			pass = cfg[""ConnectionStrings:redis:pass""];
			Name = cfg[""ConnectionStrings:redis:name""];
			Instance = new CSRedis.ConnectionPool(ip, port, poolsize);
			Instance.Connected += (s, o) => {{
				CSRedis.RedisClient rc = s as CSRedis.RedisClient;
				if (!string.IsNullOrEmpty(pass)) rc.Auth(pass);
				if (database > 0) rc.Select(database);
			}};
		}}
		#region 缓存壳
		/// <summary>
		/// 缓存壳
		/// </summary>
		/// <typeparam name=""T"">缓存类型</typeparam>
		/// <param name=""key"">不含prefix前辍RedisHelper.Name</param>
		/// <param name=""timeoutSeconds"">缓存秒数</param>
		/// <param name=""getData"">获取源数据的函数</param>
		/// <returns></returns>
		public static T Cache<T>(string key, int timeoutSeconds, Func<T> getData) => Cache(key, timeoutSeconds, getData, data => Newtonsoft.Json.JsonConvert.SerializeObject(data), cacheValue => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cacheValue));
		/// <summary>
		/// 缓存壳(哈希表)
		/// </summary>
		/// <typeparam name=""T"">缓存类型</typeparam>
		/// <param name=""key"">不含prefix前辍RedisHelper.Name</param>
		/// <param name=""field"">字段</param>
		/// <param name=""timeoutSeconds"">缓存秒数</param>
		/// <param name=""getData"">获取源数据的函数</param>
		/// <returns></returns>
		public static T Cache<T>(string key, string field, int timeoutSeconds, Func<T> getData) => Cache(key, field, timeoutSeconds, getData, data => Newtonsoft.Json.JsonConvert.SerializeObject(data), cacheValue => Newtonsoft.Json.JsonConvert.DeserializeObject<(T, DateTime)>(cacheValue));
		/// <summary>
		/// 缓存壳
		/// </summary>
		/// <typeparam name=""T"">缓存类型</typeparam>
		/// <param name=""key"">不含prefix前辍RedisHelper.Name</param>
		/// <param name=""timeoutSeconds"">缓存秒数</param>
		/// <param name=""getDataAsync"">获取源数据的函数</param>
		/// <returns></returns>
		async public static Task<T> CacheAsync<T>(string key, int timeoutSeconds, Func<Task<T>> getDataAsync) => await CacheAsync(key, timeoutSeconds, getDataAsync, data => Newtonsoft.Json.JsonConvert.SerializeObject(data), cacheValue => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cacheValue));
		/// <summary>
		/// 缓存壳(哈希表)
		/// </summary>
		/// <typeparam name=""T"">缓存类型</typeparam>
		/// <param name=""key"">不含prefix前辍RedisHelper.Name</param>
		/// <param name=""field"">字段</param>
		/// <param name=""timeoutSeconds"">缓存秒数</param>
		/// <param name=""getDataAsync"">获取源数据的函数</param>
		/// <returns></returns>
		async public static Task<T> CacheAsync<T>(string key, string field, int timeoutSeconds, Func<Task<T>> getDataAsync) => await CacheAsync(key, field, timeoutSeconds, getDataAsync, data => Newtonsoft.Json.JsonConvert.SerializeObject(data), cacheValue => Newtonsoft.Json.JsonConvert.DeserializeObject<(T, DateTime)>(cacheValue));
		#endregion
	}}

	//截至 1.2.6 版本仍然有 Timeout bug
	//public partial class RedisHelper : StackExchange.Redis.QuickHelperBase {{
	//	public static IConfigurationRoot Configuration {{ get; internal set; }}
	//	public static void InitializeConfiguration(IConfigurationRoot cfg) {{
	//		Configuration = cfg;
	//		int port, poolsize, database;
	//		string ip, pass;
	//		if (!int.TryParse(cfg[""ConnectionStrings:redis:port""], out port)) port = 6379;
	//		if (!int.TryParse(cfg[""ConnectionStrings:redis:poolsize""], out poolsize)) poolsize = 50;
	//		if (!int.TryParse(cfg[""ConnectionStrings:redis:database""], out database)) database = 0;
	//		ip = cfg[""ConnectionStrings:redis:ip""];
	//		pass = cfg[""ConnectionStrings:redis:pass""];
	//		Name = cfg[""ConnectionStrings:redis:name""];
	//		Instance = new StackExchange.Redis.ConnectionMultiplexerPool($""{{ip}}:{{port}},password={{pass}},name={{Name}},defaultdatabase={{database}}"", poolsize);
	//	}}
	//}}
}}

public static partial class {0}BLLExtensionMethods {{
	
}}
";
			#endregion
			public static readonly string Model_Build_ExtensionMethods_cs =
			#region 内容太长已被收起
 @"using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using {0}.Model;

public static partial class {0}ExtensionMethods {{
	public static double Distance(this MygisPoint that, MygisPoint point) {{
		double radLat1 = (double)(that.Y) * Math.PI / 180d;
		double radLng1 = (double)(that.X) * Math.PI / 180d;
		double radLat2 = (double)(point.Y) * Math.PI / 180d;
		double radLng2 = (double)(point.X) * Math.PI / 180d;
		return 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((radLat1 - radLat2) / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin((radLng1 - radLng2) / 2), 2))) * 6378137;
	}}
{1}
	public static string GetJson(IEnumerable items) {{
		StringBuilder sb = new StringBuilder();
		sb.Append(""["");
		IEnumerator ie = items.GetEnumerator();
		if (ie.MoveNext()) {{
			while (true) {{
				sb.Append(string.Concat(ie.Current));
				if (ie.MoveNext()) sb.Append("","");
				else break;
			}}
		}}
		sb.Append(""]"");
		return sb.ToString();
	}}
	public static IDictionary[] GetBson(IEnumerable items, Delegate func = null) {{
		List<IDictionary> ret = new List<IDictionary>();
		IEnumerator ie = items.GetEnumerator();
		while (ie.MoveNext()) {{
			if (ie.Current == null) ret.Add(null);
			else if (func == null) ret.Add(ie.Current.GetType().GetMethod(""ToBson"").Invoke(ie.Current, new object[] {{ false }}) as IDictionary);
			else {{
				object obj = func.GetMethodInfo().Invoke(func.Target, new object[] {{ ie.Current }});
				if (obj is IDictionary) ret.Add(obj as IDictionary);
				else {{
					Hashtable ht = new Hashtable();
					PropertyInfo[] pis = obj.GetType().GetProperties();
					foreach (PropertyInfo pi in pis) ht[pi.Name] = pi.GetValue(obj);
					ret.Add(ht);
				}}
			}}
		}}
		return ret.ToArray();
	}}
}}";
			#endregion

			public static readonly string Db_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>netstandard2.0</TargetFramework>
		<AssemblyName>{0}.db</AssemblyName>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include=""dng.Mysql"" Version=""1.0.3"" />
	</ItemGroup>
</Project>
";
			#endregion

            public static readonly string Infrastructure_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>netstandard2.0</TargetFramework>
		<WarningLevel>3</WarningLevel>
	</PropertyGroup>
	<ItemGroup>
		<ProjectReference Include=""..\{0}.db\{0}.db.csproj"" />
	</ItemGroup>
	<ItemGroup>
		<PackageReference Include=""Caching.CSRedis"" Version=""2.1.3"" />
		<PackageReference Include=""Microsoft.AspNetCore.Mvc"" Version=""2.1.0"" />
		<PackageReference Include=""Microsoft.AspNetCore.Session"" Version=""2.1.0"" />
		<PackageReference Include=""Microsoft.AspNetCore.Diagnostics"" Version=""2.1.0"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.EnvironmentVariables"" Version=""2.1.0"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.FileExtensions"" Version=""2.1.0"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.Json"" Version=""2.1.0"" />
		<PackageReference Include=""NLog.Extensions.Logging"" Version=""1.1.0"" />
		<PackageReference Include=""NLog.Web.AspNetCore"" Version=""4.5.4"" />
		<PackageReference Include=""Swashbuckle.AspNetCore"" Version=""2.5.0"" />
		<PackageReference Include=""System.Text.Encoding.CodePages"" Version=""4.5.0"" />
	</ItemGroup>
</Project>
";
			#endregion

			public static readonly string WebHost_Extensions_StarupExtensions_cs =
			#region 内容太长已被收起
 @"using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

public static class StarupExtensions {{
	public static ConfigurationBuilder LoadInstalledModules(this ConfigurationBuilder build, IList<ModuleInfo> modules, IHostingEnvironment env) {{
		var moduleRootFolder = new DirectoryInfo(Path.Combine(env.ContentRootPath, ""Module""));
		var moduleFolders = moduleRootFolder.GetDirectories();

		foreach (var moduleFolder in moduleFolders) {{
			Assembly assembly;
			try {{
				assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(moduleFolder.FullName, moduleFolder.Name + "".dll""));
			}} catch (FileLoadException) {{
				throw;
			}}
			if (assembly.FullName.Contains(moduleFolder.Name))
				modules.Add(new ModuleInfo {{
					Name = moduleFolder.Name,
					Assembly = assembly,
					Path = moduleFolder.FullName
				}});
		}}

		return build;
	}}

	public static ConfigurationBuilder AddCustomizedJsonFile(this ConfigurationBuilder build, IList<ModuleInfo> modules, IHostingEnvironment env, string productPath) {{
		build.SetBasePath(env.ContentRootPath).AddJsonFile(""appsettings.json"", true, true);
		foreach (var module in modules) {{
			var jsonpath = $""Module/{{module.Name}}/appsettings.json"";
			if (File.Exists(Path.Combine(env.ContentRootPath, jsonpath)))
				build.AddJsonFile(jsonpath, true, true);
		}}
		if (env.IsProduction()) {{
			build.AddJsonFile(Path.Combine(productPath, ""appsettings.json""), true, true);
			foreach (var module in modules) {{
				var jsonpath = Path.Combine(productPath, $""Module_{{module.Name}}_appsettings.json"");
				if (File.Exists(Path.Combine(env.ContentRootPath, jsonpath)))
					build.AddJsonFile(jsonpath, true, true);
			}}
		}}
		return build;
	}}

	public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules) {{
		var mvcBuilder = services.AddMvc().AddJsonOptions(a => {{
				a.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
				a.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
				a.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
			}})
			.AddRazorOptions(o => {{
				foreach (var module in modules) {{
					var a = MetadataReference.CreateFromFile(module.Assembly.Location);
					o.AdditionalCompilationReferences.Add(a);
				}}
			}})
			.AddViewLocalization()
			.AddDataAnnotationsLocalization();

		foreach (var module in modules)
			mvcBuilder.AddApplicationPart(module.Assembly);

		return services;
	}}

	public static IApplicationBuilder UseCustomizedMvc(this IApplicationBuilder app, IList<ModuleInfo> modules) {{
		foreach (var module in modules) {{
			var moduleInitializerType =
				module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
			if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer))) {{
				var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
				moduleInitializer.Init(app);
			}}
		}}
		return app.UseMvc();
	}}
	public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app, IList<ModuleInfo> modules) {{
		app.UseDefaultFiles();
		app.UseStaticFiles(new StaticFileOptions() {{
			OnPrepareResponse = (context) => {{
				var headers = context.Context.Response.GetTypedHeaders();
				headers.CacheControl = new CacheControlHeaderValue() {{
					Public = true,
					MaxAge = TimeSpan.FromDays(60)
				}};
			}}
		}});
		return app;
	}}
}}
";
			#endregion
			public static readonly string WebHost_Extensions_SwaggerExtensions_cs =
			#region 内容太长已被收起
 @"using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Linq;

namespace Swashbuckle.AspNetCore.Swagger {{
	public class FormDataOperationFilter : IOperationFilter {{
		public void Apply(Operation operation, OperationFilterContext context) {{
			var actattrs = context.ApiDescription.ActionAttributes();
			if (actattrs.OfType<HttpPostAttribute>().Any() ||
				actattrs.OfType<HttpPutAttribute>().Any())
				if (operation.Consumes.Count == 0)
					operation.Consumes.Add(""multipart/form-data"");
		}}
	}}

	public static class SwashbuckleSwaggerExtensions {{
		public static IServiceCollection AddCustomizedSwaggerGen(this IServiceCollection services) {{
			services.AddSwaggerGen(options => {{
				foreach (var doc in _docs) options.SwaggerDoc(doc, new Info {{ Version = doc }});
				options.DocInclusionPredicate((docName, apiDesc) => {{
					var versions = apiDesc.ControllerAttributes()
						.OfType<ApiExplorerSettingsAttribute>()
						.Select(attr => attr.GroupName);
					if (docName == ""未分类"" && versions.Count() == 0) return true;
					return versions.Any(v => v == docName);
				}});
				options.IgnoreObsoleteActions();
				//options.IgnoreObsoleteControllers(); // 类、方法标记 [Obsolete]，可以阻止【Swagger文档】生成
				options.DescribeAllEnumsAsStrings();
				options.CustomSchemaIds(a => a.FullName);
				options.OperationFilter<FormDataOperationFilter>();

				string root = Path.Combine(Directory.GetCurrentDirectory(), ""Module"");
				string xmlFile = string.Empty;
				string[] dirs = Directory.GetDirectories(root);
				foreach (var d in dirs) {{
					xmlFile = Path.Combine(d, $""{{new DirectoryInfo(d).Name}}.xml"");
					if (File.Exists(xmlFile))
						options.IncludeXmlComments(xmlFile); // 使用前需开启项目注释 xmldoc
				}}
				var InfrastructureXml = Directory.GetFiles(Directory.GetCurrentDirectory(), ""Infrastructure.xml"", SearchOption.AllDirectories);
				if (InfrastructureXml.Any())
					options.IncludeXmlComments(InfrastructureXml[0]);
			}});
			return services;
		}}
		static string[] _docs = new[] {{ ""未分类"", ""代理后台"", ""超级管理员后台"", ""APP后台"", ""餐饮"", ""APP后台_餐饮"", ""砍价"", ""APP后台_砍价"" }};
		public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app, IHostingEnvironment env) {{
			return app.UseSwagger().UseSwaggerUI(options => {{
				foreach (var doc in _docs) options.SwaggerEndpoint($""/swagger/{{doc}}/swagger.json"", doc);
			}});
		}}
	}}
}}
";
			#endregion
			public static readonly string WebHost_nlog_config =
			#region 内容太长已被收起
 @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	autoReload=""true""
	internalLogLevel=""Warn""
	internalLogFile=""internal-nlog.txt"">

	<!-- Load the ASP.NET Core plugin -->
	<extensions>
		<add assembly=""NLog.Web.AspNetCore""/>
	</extensions>

	<!-- Layout: https://github.com/NLog/NLog/wiki/Layout%20Renderers -->
	<targets>
		<target xsi:type=""File"" name=""allfile"" fileName=""../nlog/all-${{shortdate}}.log""
			layout=""${{longdate}}|${{logger}}|${{uppercase:${{level}}}}|${{message}} ${{exception}}|${{aspnet-Request-Url}}"" />

		<target xsi:type=""File"" name=""ownFile-web"" fileName=""../nlog/own-${{shortdate}}.log""
			layout=""${{longdate}}|${{logger}}|${{uppercase:${{level}}}}|  ${{message}} ${{exception}}|${{aspnet-Request-Url}}"" />

		<target xsi:type=""File"" name=""SQLExecuter"" fileName=""../nlog/SQLExecuter-${{shortdate}}.log""
			layout=""${{longdate}} ${{message}} ${{exception}}|${{aspnet-Request-Url}} ${{document-uri}} "" />

		<target xsi:type=""Null"" name=""blackhole"" />
	</targets>

	<rules>
		<logger name=""*"" minlevel=""Error"" writeTo=""allfile"" />
		<logger name=""Microsoft.*"" minlevel=""Error"" writeTo=""blackhole"" final=""true"" />
		<logger name=""*"" minlevel=""Error"" writeTo=""ownFile-web"" />
		<logger name=""{0}_DAL_sqlhelper"" minlevel=""Warn"" writeTo=""SQLExecuter"" />
	</rules>
</nlog>
";
			#endregion
			public static readonly string WebHost_appsettings_json =
			#region 内容太长已被收起
 @"{{
	""Logging"": {{
		""IncludeScopes"": false,
		""LogLevel"": {{
			""Default"": ""Debug"",
			""System"": ""Information"",
			""Microsoft"": ""Information""
		}}
	}},
	""ConnectionStrings"": {{
		""MySql"": ""{{connectionString}};Encrypt=False;Max pool size=32"",
		""redis"": {{
			""ip"": ""127.0.0.1"",
			""port"": 6379,
			""pass"": """",
			""database"": 13,
			""poolsize"": 50,
			""name"": ""{0}""
		}}
	}},
	""{0}_BLL_ITEM_CACHE"": {{
		""Timeout"": 180
	}}
}}
";
			#endregion
			public static readonly string WebHost_Program_cs =
			#region 内容太长已被收起
 @"using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace {0}.WebHost {{
	public class Program {{
		public static void Main(string[] args) {{
			var host = new WebHostBuilder()
				.UseUrls(""http://*:5000"", ""http://*:5001"")
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}}
	}}
}}
";
			#endregion

			public static readonly string WebHost_Startup_cs =
			#region 内容太长已被收起
 @"using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace {0}.WebHost {{
	public class Startup {{
		public Startup(IHostingEnvironment env) {{
			var builder = new ConfigurationBuilder()
				.LoadInstalledModules(Modules, env)
				.AddCustomizedJsonFile(Modules, env, ""/var/webos/{0}/"");

			this.Configuration = builder.AddEnvironmentVariables().Build();
			this.env = env;

			Newtonsoft.Json.JsonConvert.DefaultSettings = () => {{
				var st = new Newtonsoft.Json.JsonSerializerSettings();
				st.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
				st.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
				st.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
				return st;
			}};
		}}

		public static IList<ModuleInfo> Modules = new List<ModuleInfo>();
		public IConfigurationRoot Configuration {{ get; }}
		public IHostingEnvironment env {{ get; }}

		public void ConfigureServices(IServiceCollection services) {{
			services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.CSRedisCache());
			services.AddSingleton<IConfigurationRoot>(Configuration);
			services.AddSingleton<IHostingEnvironment>(env);
			services.AddScoped<CustomExceptionFilter>();

			services.AddSession(a => {{
				a.IdleTimeout = TimeSpan.FromMinutes(30);
				a.Cookie.Name = ""Session_{0}"";
			}});
			services.AddCors(options => options.AddPolicy(""cors_all"", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
			services.AddCustomizedMvc(Modules);
			services.Configure<RazorViewEngineOptions>(options => {{ options.ViewLocationExpanders.Add(new ModuleViewLocationExpander()); }});

			if (env.IsDevelopment())
				services.AddCustomizedSwaggerGen();
		}}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime) {{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Console.OutputEncoding = Encoding.GetEncoding(""GB2312"");
			Console.InputEncoding = Encoding.GetEncoding(""GB2312"");

			loggerFactory.AddConsole(Configuration.GetSection(""Logging""));
			loggerFactory.AddNLog().AddDebug();
			NLog.LogManager.LoadConfiguration(""nlog.config"");

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			{0}.BLL.RedisHelper.InitializeConfiguration(Configuration);
			{0}.DAL.SqlHelper.Instance.Log = loggerFactory.CreateLogger(""{0}_DAL_sqlhelper"");

			app.UseSession();
			app.UseCors(""cors_all"");
			app.UseCustomizedMvc(Modules);
			app.UseCustomizedStaticFiles(Modules);

			if (env.IsDevelopment())
				app.UseCustomizedSwagger(env);
		}}
	}}
}}
";
			#endregion
			public static readonly string WebHost_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
	<PropertyGroup>
		<TargetFramework>netcoreapp2.1</TargetFramework>
		<WarningLevel>3</WarningLevel>
		<PostBuildEvent>gulp --gulpfile ../../../gulpfile.js copy-module</PostBuildEvent>
	</PropertyGroup>
	<ItemGroup>
		<Folder Include=""wwwroot\"" />
		<Compile Remove=""Module\**"" />
		<Compile Remove=""wwwroot\module\**"" />
		<Content Remove=""Module\**"" />
		<Content Remove=""wwwroot\module\**"" />
		<EmbeddedResource Remove=""Module\**"" />
		<EmbeddedResource Remove=""wwwroot\module\**"" />
		<None Remove=""Module\**"" />
		<None Remove=""wwwroot\module\**"" />
		<Content Update=""nlog.config"">
			<CopyToOutputDirectory>Always</CopyToOutputDirectory>
		</Content>
	</ItemGroup>
	<ItemGroup>
		<ProjectReference Include=""..\Infrastructure\Infrastructure.csproj"" />
	</ItemGroup>
	<ItemGroup>
		<PackageReference Include=""Microsoft.AspNetCore.App"" />
	</ItemGroup>
</Project>
";
			#endregion

			public static readonly string Module_Admin_Controllers_SysController =
			#region 内容太长已被收起
 @"using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using {0}.BLL;
using {0}.Model;

namespace {0}.Module.Admin.Controllers {{
	[Route(""[controller]"")]
	[Obsolete]
	public class SysController : Controller {{
		[HttpGet(@""connection"")]
		public object Get_connection() {{
			List<Hashtable> ret = new List<Hashtable>();
			foreach (var conn in SqlHelper.Instance.Pool.AllConnections) {{
				ret.Add(new Hashtable() {{
						{{ ""数据库"", conn.SqlConnection.Database }},
						{{ ""状态"", conn.SqlConnection.State }},
						{{ ""最后活动"", conn.LastActive }},
						{{ ""获取次数"", conn.UseSum }}
					}});
			}}
			return new {{
				FreeConnections = SqlHelper.Instance.Pool.FreeConnections.Count,
				AllConnections = SqlHelper.Instance.Pool.AllConnections.Count,
				GetConnectionQueue = SqlHelper.Instance.Pool.GetConnectionQueue.Count,
				GetConnectionAsyncQueue = SqlHelper.Instance.Pool.GetConnectionAsyncQueue.Count,
				List = ret
			}};
		}}
		[HttpGet(@""connection/redis"")]
		public object Get_connection_redis() {{
			List<Hashtable> ret = new List<Hashtable>();
			foreach (var conn in RedisHelper.Instance.AllConnections) {{
				ret.Add(new Hashtable() {{
						{{ ""最后活动"", conn.LastActive }},
						{{ ""获取次数"", conn.UseSum }}
					}});
			}}
			return new {{
				FreeConnections = RedisHelper.Instance.FreeConnections.Count,
				AllConnections = RedisHelper.Instance.AllConnections.Count,
				GetConnectionQueue = RedisHelper.Instance.GetConnectionQueue.Count,
				GetConnectionAsyncQueue = RedisHelper.Instance.GetConnectionAsyncQueue.Count,
				List = ret
			}};
		}}

		[HttpGet(@""init_sysdir"")]
		public APIReturn Get_init_sysdir() {{
			/*
			if (Sysdir.SelectByParent_id(null).Count() > 0)
				return new APIReturn(-33, ""本系统已经初始化过，页面没经过任何操作退出。"");

			SysdirInfo dir1, dir2, dir3;
			dir1 = Sysdir.Insert(null, DateTime.Now, ""运营管理"", 1, null);{1}
			*/
			return new APIReturn(0, ""管理目录已初始化完成。"");
		}}
	}}
}}
";
			#endregion

			public static readonly string Module_Admin_Controllers_LoginController =
			#region 内容太长已被收起
 @"using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using {0}.BLL;
using {0}.Model;

namespace {0}.Module.Admin.Controllers {{
	[Route(""[controller]"")]
	[Obsolete]
	public class LoginController : BaseController {{

		public LoginController(ILogger<LoginController> logger) : base(logger) {{ }}

		[HttpGet]
		[匿名访问]
		public ViewResult Index() {{
			return View();
		}}
		[HttpPost]
		[匿名访问]
		public APIReturn Post(LoginModel data) {{
			HttpContext.Session.SetString(""login.username"", data.Username);
			return APIReturn.成功;
		}}

		public class LoginModel {{
			[FromForm]
			[Required(ErrorMessage = ""请输入登陆名"")]
			public string Username {{ get; set; }}

			[FromForm]
			[Required(ErrorMessage = ""请输入密码"")]
			public string Password {{ get; set; }}
		}}
	}}
}}
";
			#endregion
			public static readonly string Module_Admin_Views_Login_Index_cshtml =
			#region 内容太长已被收起
 @"@{{
	Layout = """";
}}

<!DOCTYPE html>
<html>
<head>
	<meta charset=""utf-8"">
	<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
	<title>{0}后台管理中心 - 登陆</title>
	<!-- Tell the browser to be responsive to screen width -->
	<meta content=""width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"" name=""viewport"">
	<link rel=""stylesheet"" href=""/module/admin/htm/bootstrap/css/bootstrap.min.css"">
	<link rel=""stylesheet"" href=""/module/admin/htm/plugins/font-awesome/css/font-awesome.min.css"" />
	<link rel=""stylesheet"" href=""/module/admin/htm/css/system.css"">
	<script type=""text/javascript"" src=""/module/admin/htm/js/jQuery-2.1.4.min.js""></script>
	<script type=""text/javascript"" src=""/module/admin/htm/js/lib.js""></script>
	<!--[if lt IE 9]>
	<script type='text/javascript' src='/module/admin/htm/plugins/html5shiv/html5shiv.min.js'></script>
	<script type='text/javascript' src='/module/admin/htm/plugins/respond/respond.min.js'></script>
	<![endif]-->

<style type=""text/css"">
.login-box-body--has-errors{{animation:shake .5s .25s 1;-webkit-animation:shake .5s .25s 1}}
@@keyframes shake{{0%,100%{{transform:translateX(0)}}20%,60%{{transform:translateX(-10px)}}40%,80%{{transform:translateX(10px)}}}}
@@-webkit-keyframes shake{{0%,100%{{-webkit-transform:translateX(0)}}20%,60%{{-webkit-transform:translateX(-10px)}}40%,80%{{-webkit-transform:translateX(10px)}}}}
</style>

</head>
<body class=""hold-transition login-page"">
	<div class=""login-box"">
		<div class=""login-logo"">
			<a href=""/module/admin/""><b>{0}</b>后台管理中心</a>
		</div>

		<div id=""error_msg"" style=""display:none;"">
			<div class=""alert alert-warning alert-dismissible"">
				<button type=""button"" class=""close"" data-dismiss=""alert"" aria-hidden=""true"">×</button>
				<h4><i class=""icon fa fa-warning""></i>警告!</h4>
				{{0}}
			</div>
		</div>

		<!-- /.login-logo -->
		<div class=""login-box-body"">
			<p class=""login-box-msg""></p>

			<iframe name=""iframe_form_login"" hidden></iframe>
			<form id=""form_login"" method=""post"" target=""iframe_form_login"">
				@Html.AntiForgeryToken()
				<input type=""hidden"" name=""__callback"" value=""login_callback"" />
				<div class=""form-group has-feedback"">
					<input name=""username"" type=""text"" class=""form-control"" placeholder=""Username"">
					<span class=""glyphicon glyphicon-envelope form-control-feedback""></span>
				</div>
				<div class=""form-group has-feedback"">
					<input name=""password"" type=""password"" class=""form-control"" placeholder=""Password"">
					<span class=""glyphicon glyphicon-lock form-control-feedback""></span>
				</div>
				<div class=""row"">
					<div class=""col-xs-8"">
					</div>
					<!-- /.col -->
					<div class=""col-xs-4"">
						<button type=""submit"" class=""btn btn-primary btn-block btn-flat"">登 陆</button>
					</div>
					<!-- /.col -->
				</div>
			</form>

		</div>
		<!-- /.login-box-body -->
	</div>
	<!-- /.login-box -->

	<!-- jQuery 2.2.0 -->
	<script src=""/module/admin/htm/plugins/jQuery/jQuery-2.2.0.min.js""></script>
	<script src=""/module/admin/htm/bootstrap/js/bootstrap.min.js""></script>

<script type=""text/javascript"">
	(function () {{
		var msgtpl = $('#error_msg').html();
		top.login_callback = function (rt) {{
			if (rt.success) return location.href = '/module/admin/';
			$('#error_msg').html(msgtpl.format(rt.message)).show();
			$('div.login-box-body').addClass('login-box-body--has-errors');
			setTimeout(function () {{
				$('div.login-box-body').removeClass('login-box-body--has-errors');
			}}, 2000);
		}};
	}})();
</script>
</body>
</html>
";
			#endregion

			public static readonly string Module_Admin_Controller =
			#region 内容太长已被收起
			@"using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using {0}.BLL;
using {0}.Model;

namespace {0}.Module.Admin.Controllers {{
	[Route(""[controller]"")]
	public class {1}Controller : BaseController {{
		public {1}Controller(ILogger<{1}Controller> logger) : base(logger) {{ }}

		[HttpGet]
		async public Task<ActionResult> List([FromServices]IConfigurationRoot cfg, {12}[FromQuery] int limit = 20, [FromQuery] int page = 1) {{
			var select = {19}{1}.Select{8};{9}
			long count;
			var items = await select.Count(out count){14}.Page(page, limit).ToListAsync();
			ViewBag.items = items;
			ViewBag.count = count;
			return View();
		}}

		[HttpGet(@""add"")]
		public ActionResult Edit() {{
			return View();
		}}
		[HttpGet(@""edit"")]
		async public Task<ActionResult> Edit({4}) {{
			{1}Info item = await {19}{1}.GetItemAsync({5});
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			ViewBag.item = item;
			return View();
		}}

		/***************************************** POST *****************************************/
		[HttpPost(@""add"")]
		[ValidateAntiForgeryToken]
		async public Task<APIReturn> _Add({10}) {{
			{1}Info item = new {1}Info();{13}{7}
			item = await {19}{1}.InsertAsync(item);{16}
			return APIReturn.成功.SetData(""item"", item.ToBson());
		}}
		[HttpPost(@""edit"")]
		[ValidateAntiForgeryToken]
		async public Task<APIReturn> _Edit({4}{11}) {{
			{1}Info item = await {19}{1}.GetItemAsync({5});
			if (item == null) return APIReturn.记录不存在_或者没有权限;{6}{7}
			int affrows = await {19}{1}.UpdateAsync(item);{17}
			if (affrows > 0) return APIReturn.成功.SetMessage($""更新成功，影响行数：{{affrows}}"");
			return APIReturn.失败;
		}}

		[HttpPost(""del"")]
		[ValidateAntiForgeryToken]{18}
	}}
}}
";
			#endregion

			public static readonly string Module_Admin_wwwroot_index_html =
			#region 内容太长已被收起
			@"<!DOCTYPE html>
<html lang=""zh-cmn-Hans"">
<head>
	<meta charset=""utf-8"" />
	<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
	<title>{0}管理系统</title>
	<meta content=""width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"" name=""viewport"" />
	<link href=""./htm/bootstrap/css/bootstrap.min.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/font-awesome/css/font-awesome.min.css"" rel=""stylesheet"" />
	<link href=""./htm/css/skins/_all-skins.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/pace/pace.min.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/datepicker/datepicker3.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/timepicker/bootstrap-timepicker.min.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/select2/select2.min.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/treetable/css/jquery.treetable.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/treetable/css/jquery.treetable.theme.default.css"" rel=""stylesheet"" />
	<link href=""./htm/plugins/multiple-select/multiple-select.css"" rel=""stylesheet"" />
	<link href=""./htm/css/system.css"" rel=""stylesheet"" />
	<link href=""./htm/css/index.css"" rel=""stylesheet"" />
	<script type=""text/javascript"" src=""./htm/js/jQuery-2.1.4.min.js""></script>
	<script type=""text/javascript"" src=""./htm/bootstrap/js/bootstrap.min.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/pace/pace.min.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/datepicker/bootstrap-datepicker.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/timepicker/bootstrap-timepicker.min.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/select2/select2.full.min.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/input-mask/jquery.inputmask.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/input-mask/jquery.inputmask.date.extensions.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/input-mask/jquery.inputmask.extensions.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/treetable/jquery.treetable.js""></script>
	<script type=""text/javascript"" src=""./htm/plugins/multiple-select/multiple-select.js""></script>
	<script type=""text/javascript"" src=""./htm/js/lib.js""></script>
	<script type=""text/javascript"" src=""./htm/js/bmw.js""></script>
	<!--[if lt IE 9]>
	<script type='text/javascript' src='./htm/plugins/html5shiv/html5shiv.min.js'></script>
	<script type='text/javascript' src='./htm/plugins/respond/respond.min.js'></script>
	<![endif]-->
</head>
<body class=""hold-transition skin-blue sidebar-mini"">
	<div class=""wrapper"">
		<!-- Main Header-->
		<header class=""main-header"">
			<!-- Logo--><a href=""./"" class=""logo"">
				<!-- mini logo for sidebar mini 50x50 pixels--><span class=""logo-mini""><b>{0}</b></span>
				<!-- logo for regular state and mobile devices--><span class=""logo-lg""><b>{0}管理系统</b></span>
			</a>
			<!-- Header Navbar-->
			<nav role=""navigation"" class=""navbar navbar-static-top"">
				<!-- Sidebar toggle button--><a href=""#"" data-toggle=""offcanvas"" role=""button"" class=""sidebar-toggle""><span class=""sr-only"">Toggle navigation</span></a>
				<!-- Navbar Right Menu-->
				<div class=""navbar-custom-menu"">
					<ul class=""nav navbar-nav"">
						<!-- User Account Menu-->
						<li class=""dropdown user user-menu"">
							<!-- Menu Toggle Button--><a href=""#"" data-toggle=""dropdown"" class=""dropdown-toggle"">
								<!-- The user image in the navbar--><img src=""/htm/img/user2-160x160.jpg"" alt=""User Image"" class=""user-image"">
								<!-- hidden-xs hides the username on small devices so only the image appears.--><span class=""hidden-xs""></span>
							</a>
							<ul class=""dropdown-menu"">
								<!-- The user image in the menu-->
								<li class=""user-header"">
									<img src=""/htm/img/user2-160x160.jpg"" alt=""User Image"" class=""img-circle"">
									<p></p>
								</li>
								<!-- Menu Footer-->
								<li class=""user-footer"">
									<div class=""pull-right"">
										<a href=""#"" onclick=""$('form#form_logout').submit();return false;"" class=""btn btn-default btn-flat"">安全退出</a>
										<form id=""form_logout"" method=""post"" action=""./exit.aspx""></form>
									</div>
								</li>
							</ul>
						</li>
					</ul>
				</div>
			</nav>
		</header>
		<!-- Left side column. contains the logo and sidebar-->
		<aside class=""main-sidebar"">
			<!-- sidebar: style can be found in sidebar.less-->
			<section class=""sidebar"">
				<!-- Sidebar Menu-->
				<ul class=""sidebar-menu"">
					<!-- Optionally, you can add icons to the links-->

					<li class=""treeview active"">
						<a href=""#""><i class=""fa fa-laptop""></i><span>通用管理</span><i class=""fa fa-angle-left pull-right""></i></a>
						<ul class=""treeview-menu"">{1}
						</ul>
					</li>

				</ul>
				<!-- /.sidebar-menu-->
			</section>
			<!-- /.sidebar-->
		</aside>
		<!-- Content Wrapper. Contains page content-->
		<div class=""content-wrapper"">
			<!-- Main content-->
			<section id=""right_content"" class=""content"">
				<div style=""display:none;"">
					<!-- Your Page Content Here-->
					<h1>这是一个测试首页</h1>
					<h2>swagger webapi：<a href='/swagger/' target='_blank'>/swagger/</a><h2>
					<h2>登陆地址：<a href='/login' target='_blank'>/login</a><h2>

					<h2><a href='/sys/connection' target='_blank'>查看 Mysql连接池</a><h2>
					<h2><a href='/sys/connection/redis' target='_blank'>查看 Redis连接池</a><h2>
				</div>
			</section>
			<!-- /.content-->
		</div>
		<!-- /.content-wrapper-->
	</div>
	<!-- ./wrapper-->
	<script type=""text/javascript"" src=""./htm/js/system.js""></script>
	<script type=""text/javascript"" src=""./htm/js/admin.js""></script>
	<script type=""text/javascript"">
		if (!location.hash) $('#right_content div:first').show();
		// 路由功能
		//针对上面的html初始化路由列表
		function hash_encode(str) {{ return url_encode(base64.encode(str)).replace(/%/g, '_'); }}
		function hash_decode(str) {{ return base64.decode(url_decode(str.replace(/_/g, '%'))); }}
		window.div_left_router = {{}};
		$('li.treeview.active ul li a').each(function(index, ele) {{
			var href = $(ele).attr('href');
			$(ele).attr('href', '#base64url' + hash_encode(href));
			window.div_left_router[href] = $(ele).text();
		}});
		(function () {{
			function Vipspa() {{
			}}
			Vipspa.prototype.start = function (config) {{
				Vipspa.mainView = $(config.view);
				startRouter();
				window.onhashchange = function () {{
					if (location._is_changed) return location._is_changed = false;
					startRouter();
				}};
			}};
			function startRouter() {{
				var hash = location.hash;
				if (hash === '') return //location.hash = $('li.treeview.active ul li a:first').attr('href');//'#base64url' + hash_encode('/resume_type/');
				if (hash.indexOf('#base64url') !== 0) return;
				var act = hash_decode(hash.substr(10, hash.length - 10));
				//叶湘勤增加的代码，加载或者提交form后，显示内容
				function ajax_success(refererUrl) {{
					if (refererUrl == location.pathname) {{ startRouter(); return function(){{}}; }}
					var hash = '#base64url' + hash_encode(refererUrl);
					if (location.hash != hash) {{
						location._is_changed = true;
						location.hash = hash;
					}}'\''
					return function (data, status, xhr) {{
						var div;
						Function.prototype.ajax = $.ajax;
						top.mainViewNav = {{
							url: refererUrl,
							trans: function (url) {{
								var act = url;
								act = act.substr(0, 1) === '/' || act.indexOf('://') !== -1 || act.indexOf('data:') === 0 ? act : join_url(refererUrl, act);
								return act;
							}},
							goto: function (url_or_form, target) {{
								var form = url_or_form;
								if (typeof form === 'string') {{
									var act = this.trans(form);
									if (String(target).toLowerCase() === '_blank') return window.open(act);
									location.hash = '#base64url' + hash_encode(act);
								}}
								else {{
									if (!window.ajax_form_iframe_max) window.ajax_form_iframe_max = 1;
									window.ajax_form_iframe_max++;
									var iframe = $('<iframe name=""ajax_form_iframe{{0}}""></iframe>'.format(window.ajax_form_iframe_max));
									Vipspa.mainView.append(iframe);
									var act = $(form).attr('action') || '';
									act = act.substr(0, 1) === '/' || act.indexOf('://') !== -1 ? act : join_url(refererUrl, act);
									if ($(form).find(':file[name]').length > 0) $(form).attr('enctype', 'multipart/form-data');
									$(form).attr('action', act);
									$(form).attr('target', iframe.attr('name'));
									iframe.on('load', function () {{
										var doc = this.contentWindow ? this.contentWindow.document : this.document;
										if (doc.body.innerHTML.length === 0) return;
										if (doc.body.innerHTML.indexOf('Error:') === 0) return alert(doc.body.innerHTML.substr(6));
										//以下 '<script ' + '是防止与本页面相匹配，不要删除
										if (doc.body.innerHTML.indexOf('<script ' + 'type=""text/javascript"">location.href=""') === -1) {{
											ajax_success(doc.location.pathname + doc.location.search)(doc.body.innerHTML, 200, null);
										}}
									}});
								}}
							}},
							reload: startRouter,
							query: qs_parseByUrl(refererUrl)
						}};
						top.mainViewInit = function () {{
							if (!div) return setTimeout(top.mainViewInit, 10);
							admin_init(function (selector) {{
								if (/<[^>]+>/.test(selector)) return $(selector);
								return div.find(selector);
							}}, top.mainViewNav);
						}};
						if (/<body[^>]*>/i.test(data))
							data = data.match(/<body[^>]*>(([^<]|<(?!\/body>))*)<\/body>/i)[1];
						div = Vipspa.mainView.html(data);
					}};
				}};
				$.ajax({{
					type: 'GET',
					url: act,
					dataType: 'html',
					success: ajax_success(act),
					error: function (jqXHR, textStatus, errorThrown) {{
						var data = jqXHR.responseText;
						if (/<body[^>]*>/i.test(data))
							data = data.match(/<body[^>]*>(([^<]|<(?!\/body>))*)<\/body>/i)[1];
						Vipspa.mainView.html(data);
					}}
				}});
			}}
			window.vipspa = new Vipspa();
		}})();
		$(function () {{
			vipspa.start({{
				view: '#right_content',
			}});
		}});
		// 页面加载进度条
		$(document).ajaxStart(function() {{ Pace.restart(); }});
	</script>
</body>
</html>";
			#endregion

			public static readonly string Module_Test_Controller =
			#region 内容太长已被收起
			@"using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using {0}.BLL;
using {0}.Model;

namespace {0}.Module.Admin.Controllers {{
	[Route(""[controller]"")]
	public class {1}Controller : BaseController {{
		public {1}Controller(ILogger<{1}Controller> logger) : base(logger) {{ }}

		[HttpGet]
		public APIReturn List([FromServices]IConfigurationRoot cfg) {{
			return APIReturn.成功;
		}}
	}}
}}
";
			#endregion

			public static readonly string Module_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>netstandard2.0</TargetFramework>
		<WarningLevel>3</WarningLevel>
	</PropertyGroup>
	<ItemGroup>
		<ProjectReference Include=""..\..\Infrastructure\Infrastructure.csproj"" />
	</ItemGroup>
</Project>
";
			#endregion
		}
	}
}
