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
Project(""{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}"") = ""Common"", ""src\Common\Common.csproj"", ""{{{5}}}""
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
		{{{5}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{5}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{5}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{5}}}.Release|Any CPU.Build.0 = Release|Any CPU
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
		{{{5}}} = {{{1}}}
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
		public static void ExecuteReader(Action<IDataReader> readerHander, string cmdText, params MySqlParameter[] cmdParms) {{
			Instance.ExecuteReader(readerHander, CommandType.Text, cmdText, cmdParms);
		}}
		public static object[][] ExeucteArray(string cmdText, params MySqlParameter[] cmdParms) {{
			return Instance.ExeucteArray(CommandType.Text, cmdText, cmdParms);
		}}
		public static int ExecuteNonQuery(string cmdText, params MySqlParameter[] cmdParms) {{
			return Instance.ExecuteNonQuery(CommandType.Text, cmdText, cmdParms);
		}}
		public static object ExecuteScalar(string cmdText, params MySqlParameter[] cmdParms) {{
			return Instance.ExecuteScalar(CommandType.Text, cmdText, cmdParms);
		}}
		/// <summary>
		/// 开启事务（不支持异步），10秒未执行完将超时
		/// </summary>
		/// <param name=""handler"">事务体 () => {{}}</param>
		public static void Transaction(AnonymousHandler handler) {{
			Transaction(handler, TimeSpan.FromSeconds(10));
		}}
		/// <summary>
		/// 开启事务（不支持异步）
		/// </summary>
		/// <param name=""handler"">事务体 () => {{}}</param>
		/// <param name=""timeout"">超时</param>
		public static void Transaction(AnonymousHandler handler, TimeSpan timeout) {{
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
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace {0}.BLL {{

	public partial class RedisHelper : CSRedis.QuickHelperBase {{
		internal static IConfigurationRoot Configuration;
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
	}}

	public static partial class BLLExtensionMethods {{
		public static List<TReturnInfo> ToList<TReturnInfo>(this SelectBuild<TReturnInfo> select, int expireSeconds, string cacheKey = null) {{ return select.ToList(RedisHelper.Get, RedisHelper.Set, TimeSpan.FromSeconds(expireSeconds), cacheKey); }}
	}}
}}";
			#endregion
			public static readonly string Model_Build_ExtensionMethods_cs =
			#region 内容太长已被收起
 @"using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace {0}.Model {{
	public static partial class ExtensionMethods {{{1}
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
	}}
}}";
			#endregion

			public static readonly string Db_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>netstandard1.6</TargetFramework>
		<AssemblyName>{0}.db</AssemblyName>
	</PropertyGroup>
	<ItemGroup>
		<ProjectReference Include=""..\Common\Common.csproj"" />
	</ItemGroup>
</Project>
";
			#endregion

			public static readonly string Common_BmwNet_cs =
			#region 内容太长已被收起
 @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

public sealed class BmwNet : IDisposable {{
	public interface IBmwNetOutput {{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""tOuTpUt"">返回内容</param>
		/// <param name=""oPtIoNs"">渲染对象</param>
		/// <param name=""rEfErErFiLeNaMe"">当前文件路径</param>
		/// <param name=""bMwSeNdEr""></param>
		/// <returns></returns>
		BmwNetReturnInfo OuTpUt(StringBuilder tOuTpUt, IDictionary oPtIoNs, string rEfErErFiLeNaMe, BmwNet bMwSeNdEr);
	}}
	public class BmwNetReturnInfo {{
		public Dictionary<string, int[]> Blocks;
		public StringBuilder Sb;
	}}
	public delegate bool BmwNetIf(object exp);
	public delegate void BmwNetPrint(params object[] parms);

	private static int _view = 0;
	private static Regex _reg = new Regex(@""\{{(\$BMW__CODE|\/\$BMW__CODE|import\s+|module\s+|extends\s+|block\s+|include\s+|for\s+|if\s+|#|\/for|elseif|else|\/if|\/block|\/module)([^\}}]*)\}}"", RegexOptions.Compiled);
	private static Regex _reg_forin = new Regex(@""^([\w_]+)\s*,?\s*([\w_]+)?\s+in\s+(.+)"", RegexOptions.Compiled);
	private static Regex _reg_foron = new Regex(@""^([\w_]+)\s*,?\s*([\w_]+)?,?\s*([\w_]+)?\s+on\s+(.+)"", RegexOptions.Compiled);
	private static Regex _reg_forab = new Regex(@""^([\w_]+)\s+([^,]+)\s*,\s*(.+)"", RegexOptions.Compiled);
	private static Regex _reg_miss = new Regex(@""\{{\/?miss\}}"", RegexOptions.Compiled);
	private static Regex _reg_code = new Regex(@""(\{{%|%\}})"", RegexOptions.Compiled);
	private static Regex _reg_syntax = new Regex(@""<(\w+)\s+@(if|for|else)\s*=""""([^""""]*)"""""", RegexOptions.Compiled);
	private static Regex _reg_htmltag = new Regex(@""<\/?\w+[^>]*>"", RegexOptions.Compiled);
	private static Regex _reg_blank = new Regex(@""\s+"", RegexOptions.Compiled);
	private static Regex _reg_complie_undefined = new Regex(@""(当前上下文中不存在名称)?“(\w+)”"", RegexOptions.Compiled);

	private Dictionary<string, IBmwNetOutput> _cache = new Dictionary<string, IBmwNetOutput>();
	private object _cache_lock = new object();
	private string _viewDir;
	private FileSystemWatcher _fsw = new FileSystemWatcher();

	public BmwNet(string viewDir) {{
		_viewDir = IniHelper.TranslateUrl(viewDir);
		_fsw = new FileSystemWatcher(_viewDir);
		_fsw.IncludeSubdirectories = true;
		_fsw.Changed += ViewDirChange;
		_fsw.Renamed += ViewDirChange;
		_fsw.EnableRaisingEvents = true;
	}}
	public void Dispose() {{
		_fsw.Dispose();
	}}
	void ViewDirChange(object sender, FileSystemEventArgs e) {{
		string filename = e.FullPath.ToLower();
		lock (_cache_lock) {{
			_cache.Remove(filename);
		}}
	}}
	public BmwNetReturnInfo RenderFile2(StringBuilder sb, IDictionary options, string filename, string refererFilename) {{
		if (filename[0] == '/' || string.IsNullOrEmpty(refererFilename)) refererFilename = _viewDir;
		//else refererFilename = Path.GetDirectoryName(refererFilename);
		string filename2 = IniHelper.TranslateUrl(filename, refererFilename);
		IBmwNetOutput bmw;
		if (_cache.TryGetValue(filename2, out bmw) == false) {{
			string tplcode = File.Exists(filename2) == false ? string.Concat(""文件不存在 "", filename) : IniHelper.ReadTextFile(filename2);
			bmw = Parser(tplcode, options);
			lock (_cache_lock) {{
				if (_cache.ContainsKey(filename2) == false) {{
					_cache.Add(filename2, bmw);
				}}
			}}
		}}
		try {{
			return bmw.OuTpUt(sb, options, filename2, this);
		}} catch (Exception ex) {{
			BmwNetReturnInfo ret = sb == null ?
				new BmwNetReturnInfo {{ Sb = new StringBuilder(), Blocks = new Dictionary<string, int[]>() }} :
				new BmwNetReturnInfo {{ Sb = sb, Blocks = new Dictionary<string, int[]>() }};
			ret.Sb.Append(refererFilename);
			ret.Sb.Append("" -> "");
			ret.Sb.Append(filename);
			ret.Sb.Append(""\r\n"");
			ret.Sb.Append(ex.Message);
			ret.Sb.Append(""\r\n"");
			ret.Sb.Append(ex.StackTrace);
			return ret;
		}}
	}}
	public string RenderFile(string filename, IDictionary options) {{
		BmwNetReturnInfo ret = this.RenderFile2(null, options, filename, null);
		return ret.Sb.ToString();
	}}
	private static IBmwNetOutput Parser(string tplcode, IDictionary options) {{
		int view = Interlocked.Increment(ref _view);
		StringBuilder sb = new StringBuilder();
		IDictionary options_copy = new Hashtable();
		foreach (DictionaryEntry options_de in options) options_copy[options_de.Key] = options_de.Value;
		sb.AppendFormat(@""
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using {0}.BLL;
using {0}.Model;

namespace BmwDynamicCodeGenerate {{{{
	public class view{{0}} : BmwNet.IBmwNetOutput {{{{
		public BmwNet.BmwNetReturnInfo OuTpUt(StringBuilder tOuTpUt, IDictionary oPtIoNs, string rEfErErFiLeNaMe, BmwNet bMwSeNdEr) {{{{
			BmwNet.BmwNetReturnInfo rTn = tOuTpUt == null ? 
				new BmwNet.BmwNetReturnInfo {{{{ Sb = (tOuTpUt = new StringBuilder()), Blocks = new Dictionary<string, int[]>() }}}} :
				new BmwNet.BmwNetReturnInfo {{{{ Sb = tOuTpUt, Blocks = new Dictionary<string, int[]>() }}}};
			Dictionary<string, int[]> BMW__blocks = rTn.Blocks;
			Stack<int[]> BMW__blocks_stack = new Stack<int[]>();
			int[] BMW__blocks_stack_peek;
			List<IDictionary> BMW__forc = new List<IDictionary>();

			Func<IDictionary> pRoCeSsOpTiOnS = new Func<IDictionary>(delegate () {{{{
				IDictionary nEwoPtIoNs = new Hashtable();
				foreach (DictionaryEntry oPtIoNs_dE in oPtIoNs)
					nEwoPtIoNs[oPtIoNs_dE.Key] = oPtIoNs_dE.Value;
				foreach (IDictionary BMW__forc_dIc in BMW__forc)
					foreach (DictionaryEntry BMW__forc_dIc_dE in BMW__forc_dIc)
						nEwoPtIoNs[BMW__forc_dIc_dE.Key] = BMW__forc_dIc_dE.Value;
				return nEwoPtIoNs;
			}}}});
			BmwNet.BmwNetIf bMwIf = delegate(object exp) {{{{
				if (exp is bool) return (bool)exp;
				if (exp == null) return false;
				if (exp is int && (int)exp == 0) return false;
				if (exp is string && (string)exp == string.Empty) return false;

				if (exp is long && (long)exp == 0) return false;
				if (exp is short && (short)exp == 0) return false;
				if (exp is byte && (byte)exp == 0) return false;

				if (exp is double && (double)exp == 0) return false;
				if (exp is float && (float)exp == 0) return false;
				if (exp is decimal && (decimal)exp == 0) return false;
				return true;
			}}}};
			BmwNet.BmwNetPrint print = delegate(object[] pArMs) {{{{
				if (pArMs == null || pArMs.Length == 0) return;
				foreach (object pArMs_A in pArMs) if (pArMs_A != null) tOuTpUt.Append(pArMs_A);
			}}}};
			BmwNet.BmwNetPrint Print = print;"", view);

		#region {{miss}}...{{/miss}}块内容将不被解析
		string[] tmp_content_arr = _reg_miss.Split(tplcode);
		if (tmp_content_arr.Length > 1) {{
			sb.AppendFormat(@""
			string[] BMW__MISS = new string[{{0}}];"", Math.Ceiling(1.0 * (tmp_content_arr.Length - 1) / 2));
			int miss_len = -1;
			for (int a = 1; a < tmp_content_arr.Length; a += 2) {{
				sb.Append(string.Concat(@""
			BMW__MISS["", ++miss_len, @""] = """""", Utils.GetConstString(tmp_content_arr[a]), @"""""";""));
				tmp_content_arr[a] = string.Concat(""{{#BMW__MISS["", miss_len, ""]}}"");
			}}
			tplcode = string.Join("""", tmp_content_arr);
		}}
		#endregion
		#region 扩展语法如 <div @if=""表达式""></div>
		tplcode = htmlSyntax(tplcode, 3); //<div @if=""c#表达式"" @for=""index 1,100""></div>
										  //处理 {{% %}} 块 c#代码
		tmp_content_arr = _reg_code.Split(tplcode);
		if (tmp_content_arr.Length == 1) {{
			tplcode = Utils.GetConstString(tplcode)
				.Replace(""{{%"", ""{{$BMW__CODE}}"")
				.Replace(""%}}"", ""{{/$BMW__CODE}}"");
		}} else {{
			tmp_content_arr[0] = Utils.GetConstString(tmp_content_arr[0]);
			for (int a = 1; a < tmp_content_arr.Length; a += 4) {{
				tmp_content_arr[a] = ""{{$BMW__CODE}}"";
				tmp_content_arr[a + 2] = ""{{/$BMW__CODE}}"";
				tmp_content_arr[a + 3] = Utils.GetConstString(tmp_content_arr[a + 3]);
			}}
			tplcode = string.Join("""", tmp_content_arr);
		}}
		#endregion
		sb.Append(@""
			tOuTpUt.Append("""""");

		string error = null;
		int bmw_tmpid = 0;
		int forc_i = 0;
		string extends = null;
		Stack<string> codeTree = new Stack<string>();
		Stack<string> forEndRepl = new Stack<string>();
		sb.Append(_reg.Replace(tplcode, delegate (Match m) {{
			string _0 = m.Groups[0].Value;
			if (!string.IsNullOrEmpty(error)) return _0;

			string _1 = m.Groups[1].Value.Trim(' ', '\t');
			string _2 = m.Groups[2].Value
				.Replace(""\\\\"", ""\\"")
				.Replace(""\\\"""", ""\"""");
			_2 = Utils.ReplaceSingleQuote(_2);

			switch (_1) {{
				#region $BMW__CODE--------------------------------------------------
				case ""$BMW__CODE"":
					codeTree.Push(_1);
					return @"""""");
"";
				case ""/$BMW__CODE"":
					string pop = codeTree.Pop();
					if (pop != ""$BMW__CODE"") {{
						codeTree.Push(pop);
						error = ""编译出错，{{% 与 %}} 并没有配对"";
						return _0;
					}}
					return @""
			tOuTpUt.Append("""""";
				#endregion
				case ""include"":
					return string.Format(@"""""");
bMwSeNdEr.RenderFile2(tOuTpUt, pRoCeSsOpTiOnS(), """"{{0}}"""", rEfErErFiLeNaMe);
			tOuTpUt.Append("""""", _2);
				case ""import"":
					return _0;
				case ""module"":
					return _0;
				case ""/module"":
					return _0;
				case ""extends"":
					//{{extends ../inc/layout.html}}
					if (string.IsNullOrEmpty(extends) == false) return _0;
					extends = _2;
					return string.Empty;
				case ""block"":
					codeTree.Push(""block"");
					return string.Format(@"""""");
BMW__blocks_stack_peek = new int[] {{{{ tOuTpUt.Length, 0 }}}};
BMW__blocks_stack.Push(BMW__blocks_stack_peek);
BMW__blocks.Add(""""{{0}}"""", BMW__blocks_stack_peek);
tOuTpUt.Append("""""", _2.Trim(' ', '\t'));
				case ""/block"":
					codeTreeEnd(codeTree, ""block"");
					return @"""""");
BMW__blocks_stack_peek = BMW__blocks_stack.Pop();
BMW__blocks_stack_peek[1] = tOuTpUt.Length - BMW__blocks_stack_peek[0];
tOuTpUt.Append("""""";

				#region ##---------------------------------------------------------
				case ""#"":
					if (_2[0] == '#')
						return string.Format(@"""""");
			try {{{{ Print({{0}}); }}}} catch {{{{ }}}}
			tOuTpUt.Append("""""", _2.Substring(1));
					return string.Format(@"""""");
			Print({{0}});
			tOuTpUt.Append("""""", _2);
				#endregion
				#region for--------------------------------------------------------
				case ""for"":
					forc_i++;
					int cur_bmw_tmpid = bmw_tmpid;
					string sb_endRepl = string.Empty;
					StringBuilder sbfor = new StringBuilder();
					sbfor.Append(@"""""");"");
					Match mfor = _reg_forin.Match(_2);
					if (mfor.Success) {{
						string mfor1 = mfor.Groups[1].Value.Trim(' ', '\t');
						string mfor2 = mfor.Groups[2].Value.Trim(' ', '\t');
						sbfor.AppendFormat(@""
//new Action(delegate () {{{{
	IDictionary BMW__tmp{{0}} = new Hashtable();
	BMW__forc.Add(BMW__tmp{{0}});
	var BMW__tmp{{1}} = {{3}};
	var BMW__tmp{{2}} = {{4}};"", ++bmw_tmpid, ++bmw_tmpid, ++bmw_tmpid, mfor.Groups[3].Value, mfor1);
						sb_endRepl = string.Concat(sb_endRepl, string.Format(@""
	{{0}} = BMW__tmp{{1}};"", mfor1, cur_bmw_tmpid + 3));
						if (options_copy.Contains(mfor1) == false) options_copy[mfor1] = null;
						if (!string.IsNullOrEmpty(mfor2)) {{
							sbfor.AppendFormat(@""
	var BMW__tmp{{1}} = {{0}};
	{{0}} = 0;"", mfor2, ++bmw_tmpid);
							sb_endRepl = string.Concat(sb_endRepl, string.Format(@""
	{{0}} = BMW__tmp{{1}};"", mfor2, bmw_tmpid));
							if (options_copy.Contains(mfor2) == false) options_copy[mfor2] = null;
						}}
						sbfor.AppendFormat(@""
	if (BMW__tmp{{1}} != null)
	foreach (var BMW__tmp{{0}} in BMW__tmp{{1}}) {{{{"", ++bmw_tmpid, cur_bmw_tmpid + 2);
						if (!string.IsNullOrEmpty(mfor2))
							sbfor.AppendFormat(@""
		BMW__tmp{{1}}[""""{{0}}""""] = ++ {{0}};"", mfor2, cur_bmw_tmpid + 1);
						sbfor.AppendFormat(@""
		BMW__tmp{{1}}[""""{{0}}""""] = BMW__tmp{{2}};
		{{0}} = BMW__tmp{{2}};
		tOuTpUt.Append("""""", mfor1, cur_bmw_tmpid + 1, bmw_tmpid);
						codeTree.Push(""for"");
						forEndRepl.Push(sb_endRepl);
						return sbfor.ToString();
					}}
					mfor = _reg_foron.Match(_2);
					if (mfor.Success) {{
						string mfor1 = mfor.Groups[1].Value.Trim(' ', '\t');
						string mfor2 = mfor.Groups[2].Value.Trim(' ', '\t');
						string mfor3 = mfor.Groups[3].Value.Trim(' ', '\t');
						sbfor.AppendFormat(@""
//new Action(delegate () {{{{
	IDictionary BMW__tmp{{0}} = new Hashtable();
	BMW__forc.Add(BMW__tmp{{0}});
	var BMW__tmp{{1}} = {{3}};
	var BMW__tmp{{2}} = {{4}};"", ++bmw_tmpid, ++bmw_tmpid, ++bmw_tmpid, mfor.Groups[4].Value, mfor1);
						sb_endRepl = string.Concat(sb_endRepl, string.Format(@""
	{{0}} = BMW__tmp{{1}};"", mfor1, cur_bmw_tmpid + 3));
						if (options_copy.Contains(mfor1) == false) options_copy[mfor1] = null;
						if (!string.IsNullOrEmpty(mfor2)) {{
							sbfor.AppendFormat(@""
	var BMW__tmp{{1}} = {{0}};"", mfor2, ++bmw_tmpid);
							sb_endRepl = string.Concat(sb_endRepl, string.Format(@""
	{{0}} = BMW__tmp{{1}};"", mfor2, bmw_tmpid));
							if (options_copy.Contains(mfor2) == false) options_copy[mfor2] = null;
						}}
						if (!string.IsNullOrEmpty(mfor3)) {{
							sbfor.AppendFormat(@""
	var BMW__tmp{{1}} = {{0}};
	{{0}} = 0;"", mfor3, ++bmw_tmpid);
							sb_endRepl = string.Concat(sb_endRepl, string.Format(@""
	{{0}} = BMW__tmp{{1}};"", mfor3, bmw_tmpid));
							if (options_copy.Contains(mfor3) == false) options_copy[mfor3] = null;
						}}
						sbfor.AppendFormat(@""
	if (BMW__tmp{{2}} != null)
	foreach (DictionaryEntry BMW__tmp{{1}} in BMW__tmp{{2}}) {{{{
		{{0}} = BMW__tmp{{1}}.Key;
		BMW__tmp{{3}}[""""{{0}}""""] = {{0}};"", mfor1, ++bmw_tmpid, cur_bmw_tmpid + 2, cur_bmw_tmpid + 1);
						if (!string.IsNullOrEmpty(mfor2))
							sbfor.AppendFormat(@""
		{{0}} = BMW__tmp{{1}}.Value;
		BMW__tmp{{2}}[""""{{0}}""""] = {{0}};"", mfor2, bmw_tmpid, cur_bmw_tmpid + 1);
						if (!string.IsNullOrEmpty(mfor3))
							sbfor.AppendFormat(@""
		BMW__tmp{{1}}[""""{{0}}""""] = ++ {{0}};"", mfor3, cur_bmw_tmpid + 1);
						sbfor.AppendFormat(@""
		tOuTpUt.Append("""""");
						codeTree.Push(""for"");
						forEndRepl.Push(sb_endRepl);
						return sbfor.ToString();
					}}
					mfor = _reg_forab.Match(_2);
					if (mfor.Success) {{
						string mfor1 = mfor.Groups[1].Value.Trim(' ', '\t');
						sbfor.AppendFormat(@""
//new Action(delegate () {{{{
	IDictionary BMW__tmp{{0}} = new Hashtable();
	BMW__forc.Add(BMW__tmp{{0}});
	var BMW__tmp{{1}} = {{5}};
	{{5}} = {{3}} - 1;
	if ({{5}} == null) {{5}} = 0;
	var BMW__tmp{{2}} = {{4}} + 1;
	while (++{{5}} < BMW__tmp{{2}}) {{{{
		BMW__tmp{{0}}[""""{{5}}""""] = {{5}};
		tOuTpUt.Append("""""", ++bmw_tmpid, ++bmw_tmpid, ++bmw_tmpid, mfor.Groups[2].Value, mfor.Groups[3].Value, mfor1);
						sb_endRepl = string.Concat(sb_endRepl, string.Format(@""
	{{0}} = BMW__tmp{{1}};"", mfor1, cur_bmw_tmpid + 1));
						if (options_copy.Contains(mfor1) == false) options_copy[mfor1] = null;
						codeTree.Push(""for"");
						forEndRepl.Push(sb_endRepl);
						return sbfor.ToString();
					}}
					return _0;
				case ""/for"":
					if (--forc_i < 0) return _0;
					codeTreeEnd(codeTree, ""for"");
					return string.Format(@"""""");
	}}}}{{0}}
	BMW__forc.RemoveAt(BMW__forc.Count - 1);
//}}}})();
			tOuTpUt.Append("""""", forEndRepl.Pop());
				#endregion
				#region if---------------------------------------------------------
				case ""if"":
					codeTree.Push(""if"");
					return string.Format(@"""""");
			if ({{1}}bMwIf({{0}})) {{{{
				tOuTpUt.Append("""""", _2[0] == '!' ? _2.Substring(1) : _2, _2[0] == '!' ? '!' : ' ');
				case ""elseif"":
					codeTreeEnd(codeTree, ""if"");
					codeTree.Push(""if"");
					return string.Format(@"""""");
			}}}} else if ({{1}}bMwIf({{0}})) {{{{
				tOuTpUt.Append("""""", _2[0] == '!' ? _2.Substring(1) : _2, _2[0] == '!' ? '!' : ' ');
				case ""else"":
					codeTreeEnd(codeTree, ""if"");
					codeTree.Push(""if"");
					return @"""""");
			}} else {{
			tOuTpUt.Append("""""";
				case ""/if"":
					codeTreeEnd(codeTree, ""if"");
					return @"""""");
			}}
			tOuTpUt.Append("""""";
					#endregion
			}}
			return _0;
		}}));

		sb.Append(@"""""");"");
		if (string.IsNullOrEmpty(extends) == false) {{
			sb.AppendFormat(@""
BmwNet.BmwNetReturnInfo eXtEnDs_ReT = bMwSeNdEr.RenderFile2(null, pRoCeSsOpTiOnS(), """"{{0}}"""", rEfErErFiLeNaMe);
string rTn_Sb_string = rTn.Sb.ToString();
foreach(string eXtEnDs_ReT_blocks_key in eXtEnDs_ReT.Blocks.Keys) {{{{
	if (rTn.Blocks.ContainsKey(eXtEnDs_ReT_blocks_key)) {{{{
		int[] eXtEnDs_ReT_blocks_value = eXtEnDs_ReT.Blocks[eXtEnDs_ReT_blocks_key];
		eXtEnDs_ReT.Sb.Remove(eXtEnDs_ReT_blocks_value[0], eXtEnDs_ReT_blocks_value[1]);
		int[] rTn_blocks_value = rTn.Blocks[eXtEnDs_ReT_blocks_key];
		eXtEnDs_ReT.Sb.Insert(eXtEnDs_ReT_blocks_value[0], rTn_Sb_string.Substring(rTn_blocks_value[0], rTn_blocks_value[1]));
		foreach(string eXtEnDs_ReT_blocks_keyb in eXtEnDs_ReT.Blocks.Keys) {{{{
			if (eXtEnDs_ReT_blocks_keyb == eXtEnDs_ReT_blocks_key) continue;
			int[] eXtEnDs_ReT_blocks_valueb = eXtEnDs_ReT.Blocks[eXtEnDs_ReT_blocks_keyb];
			if (eXtEnDs_ReT_blocks_valueb[0] >= eXtEnDs_ReT_blocks_value[0])
				eXtEnDs_ReT_blocks_valueb[0] = eXtEnDs_ReT_blocks_valueb[0] - eXtEnDs_ReT_blocks_value[1] + rTn_blocks_value[1];
		}}}}
		eXtEnDs_ReT_blocks_value[1] = rTn_blocks_value[1];
	}}}}
}}}}
return eXtEnDs_ReT;
"", extends);
		}} else {{
			sb.Append(@""
return rTn;"");
		}}
		sb.Append(@""
		}}
	}}
}}
"");
		int dim_idx = sb.ToString().IndexOf(""BmwNet.BmwNetPrint Print = print;"") + 33;
		foreach (string dic_name in options_copy.Keys) {{
			sb.Insert(dim_idx, string.Format(@""
			dynamic {{0}} = oPtIoNs[""""{{0}}""""];"", dic_name));
		}}
		//Console.WriteLine(sb.ToString());
		return Complie(sb.ToString(), @""BmwDynamicCodeGenerate.view"" + view);
	}}
	private static string codeTreeEnd(Stack<string> codeTree, string tag) {{
		string ret = string.Empty;
		Stack<int> pop = new Stack<int>();
		foreach (string ct in codeTree) {{
			if (ct == ""import"" ||
				ct == ""include"") {{
				pop.Push(1);
			}} else if (ct == tag) {{
				pop.Push(2);
				break;
			}} else {{
				if (string.IsNullOrEmpty(tag) == false) pop.Clear();
				break;
			}}
		}}
		if (pop.Count == 0 && string.IsNullOrEmpty(tag) == false)
			return string.Concat(""语法错误，{{"", tag, ""}} {{/"", tag, ""}} 并没配对"");
		while (pop.Count > 0 && pop.Pop() > 0) codeTree.Pop();
		return ret;
	}}
	#region htmlSyntax
	private static string htmlSyntax(string tplcode, int num) {{

		while (num-- > 0) {{
			string[] arr = _reg_syntax.Split(tplcode);

			if (arr.Length == 1) break;
			for (int a = 1; a < arr.Length; a += 4) {{
				string tag = string.Concat('<', arr[a]);
				string end = string.Concat(""</"", arr[a], '>');
				int fc = 1;
				for (int b = a; fc > 0 && b < arr.Length; b += 4) {{
					if (b > a && arr[a].ToLower() == arr[b].ToLower()) fc++;
					int bpos = 0;
					while (true) {{
						int fa = arr[b + 3].IndexOf(tag, bpos);
						int fb = arr[b + 3].IndexOf(end, bpos);
						if (b == a) {{
							var z = arr[b + 3].IndexOf(""/>"");
							if ((fb == -1 || z < fb) && z != -1) {{
								var y = arr[b + 3].Substring(0, z + 2);
								if (_reg_htmltag.IsMatch(y) == false)
									fb = z - end.Length + 2;
							}}
						}}
						if (fa == -1 && fb == -1) break;
						if (fa != -1 && (fa < fb || fb == -1)) {{
							fc++;
							bpos = fa + tag.Length;
							continue;
						}}
						if (fb != -1) fc--;
						if (fc <= 0) {{
							var a1 = arr[a + 1];
							var end3 = string.Concat(""{{/"", a1, ""}}"");
							if (a1.ToLower() == ""else"") {{
								if (_reg_blank.Replace(arr[a - 4 + 3], """").EndsWith(""{{/if}}"", StringComparison.CurrentCultureIgnoreCase) == true) {{
									var idx = arr[a - 4 + 3].IndexOf(""{{/if}}"");
									arr[a - 4 + 3] = string.Concat(arr[a - 4 + 3].Substring(0, idx), arr[a - 4 + 3].Substring(idx + 5));
									//如果 @else=""有条件内容""，则变换成 elseif 条件内容
									if (_reg_blank.Replace(arr[a + 2], """").Length > 0) a1 = ""elseif"";
									end3 = ""{{/if}}"";
								}} else {{
									arr[a] = string.Concat(""指令 @"", arr[a + 1], ""='"", arr[a + 2], ""' 没紧接着 if/else 指令之后，无效. <"", arr[a]);
									arr[a + 1] = arr[a + 2] = string.Empty;
								}}
							}}
							if (arr[a + 1].Length > 0) {{
								if (_reg_blank.Replace(arr[a + 2], """").Length > 0 || a1.ToLower() == ""else"") {{
									arr[b + 3] = string.Concat(arr[b + 3].Substring(0, fb + end.Length), end3, arr[b + 3].Substring(fb + end.Length));
									arr[a] = string.Concat(""{{"", a1, "" "", arr[a + 2], ""}}<"", arr[a]);
									arr[a + 1] = arr[a + 2] = string.Empty;
								}} else {{
									arr[a] = string.Concat('<', arr[a]);
									arr[a + 1] = arr[a + 2] = string.Empty;
								}}
							}}
							break;
						}}
						bpos = fb + end.Length;
					}}
				}}
				if (fc > 0) {{
					arr[a] = string.Concat(""不严谨的html格式，请检查 "", arr[a], "" 的结束标签, @"", arr[a + 1], ""='"", arr[a + 2], ""' 指令无效. <"", arr[a]);
					arr[a + 1] = arr[a + 2] = string.Empty;
				}}
			}}
			if (arr.Length > 0) tplcode = string.Join(string.Empty, arr);
		}}
		return tplcode;
	}}
	#endregion
	#region Complie
	//private static string _db_dll_location;
	private static IBmwNetOutput Complie(string cscode, string typename) {{
		//// 1.CSharpCodePrivoder
		//CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
		//// 3.CompilerParameters
		//CompilerParameters objCompilerParameters = new CompilerParameters();
		//objCompilerParameters.ReferencedAssemblies.Add(""System.dll"");
		//objCompilerParameters.GenerateExecutable = false;
		//objCompilerParameters.GenerateInMemory = true;

		//if (string.IsNullOrEmpty(_db_dll_location)) _db_dll_location = Type.GetType(""{0}.DAL.SqlHelper, {0}.db"").Assembly.Location;
		//objCompilerParameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
		//objCompilerParameters.ReferencedAssemblies.Add(_db_dll_location);
		//objCompilerParameters.ReferencedAssemblies.Add(""System.Core.dll"");
		//objCompilerParameters.ReferencedAssemblies.Add(""Microsoft.CSharp.dll"");
		//// 4.CompilerResults
		//CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, cscode);

		//if (cr.Errors.HasErrors) {{
		//	StringBuilder sb = new StringBuilder();
		//	sb.Append(""编译错误："");
		//	int undefined_idx = 0;
		//	int undefined_cout = 0;
		//	Dictionary<string, bool> undefined_exists = new Dictionary<string, bool>();
		//	foreach (CompilerError err in cr.Errors) {{
		//		sb.Append(err.ErrorText + "" 在第"" + err.Line + ""行\r\n"");
		//		if (err.ErrorNumber == ""CS0103"") {{
		//			//如果未定义变量，则自定义变量后重新编译
		//			Match m = _reg_complie_undefined.Match(err.ErrorText);
		//			if (m.Success) {{
		//				string undefined_name = m.Groups[2].Value;
		//				if (undefined_exists.ContainsKey(undefined_name) == false) {{
		//					if (undefined_idx <= 0) undefined_idx = cscode.IndexOf(""BmwNet.BmwNetPrint Print = print;"") + 33;
		//					cscode = cscode.Insert(undefined_idx, string.Format(""\r\n\t\t\tdynamic {{0}} = oPtIoNs[\""{{0}}\""];"", undefined_name));
		//					undefined_exists.Add(undefined_name, true);
		//				}}
		//				undefined_cout++;
		//			}} else {{
		//				sb.AppendFormat(""错误编号：CS0103，但是 _reg_undefined({{0}}) 匹配不到 ErrorText({{1}})\r\n"", _reg_complie_undefined, err.ErrorText);
		//			}}
		//		}}
		//	}}
		//	if (cr.Errors.Count == undefined_cout) {{
		//		return Complie(cscode, typename);
		//	}} else {{
		//		sb.Append(cscode);
		//		throw new Exception(sb.ToString());
		//	}}
		//}} else {{
		//	object ret = cr.CompiledAssembly.CreateInstance(typename);
		//	return ret as IBmwNetOutput;
		//}}
		return null;
	}}
	#endregion

	#region Utils
	public class Utils {{
		public static string ReplaceSingleQuote(object exp) {{
			//将 ' 转换成 ""
			string exp2 = string.Concat(exp);
			int quote_pos = -1;
			while (true) {{
				int first_pos = quote_pos = exp2.IndexOf('\'', quote_pos + 1);
				if (quote_pos == -1) break;
				while (true) {{
					quote_pos = exp2.IndexOf('\'', quote_pos + 1);
					if (quote_pos == -1) break;
					int r_cout = 0;
					for (int p = 1; true; p++) {{
						if (exp2[quote_pos - p] == '\\') r_cout++;
						else break;
					}}
					if (r_cout % 2 == 0/* && quote_pos - first_pos > 2*/) {{
						string str1 = exp2.Substring(0, first_pos);
						string str2 = exp2.Substring(first_pos + 1, quote_pos - first_pos - 1);
						string str3 = exp2.Substring(quote_pos + 1);
						string str4 = str2.Replace(""\"""", ""\\\"""");
						quote_pos += str4.Length - str2.Length;
						exp2 = string.Concat(str1, ""\"""", str4, ""\"""", str3);
						break;
					}}
				}}
				if (quote_pos == -1) break;
			}}
			return exp2;
		}}
		public static string GetConstString(object obj) {{
			return string.Concat(obj)
				.Replace(""\\"", ""\\\\"")
				.Replace(""\"""", ""\\\"""")
				.Replace(""\r"", ""\\r"")
				.Replace(""\n"", ""\\n"");
		}}
	}}
	#endregion
}}";
			#endregion

			public static readonly string Common_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>netstandard1.6</TargetFramework>
		<AssemblyName>Common</AssemblyName>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include=""Google.Protobuf"" Version=""3.1.0"" />
		<PackageReference Include=""Microsoft.Extensions.Caching.Abstractions"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Logging"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Logging.Abstractions"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Options.ConfigurationExtensions"" Version=""1.0.2"" />
		<PackageReference Include=""MySql.Data"" Version=""7.0.5-IR21"" />
		<PackageReference Include=""Npgsql"" Version=""3.1.9"" />
		<PackageReference Include=""Newtonsoft.Json"" Version=""9.0.1"" />
		<PackageReference Include=""System.Collections.Specialized"" Version=""4.0.1"" />
		<PackageReference Include=""System.Diagnostics.TextWriterTraceListener"" Version=""4.0.0"" />
		<PackageReference Include=""System.IO.FileSystem.Watcher"" Version=""4.0.0"" />
		<PackageReference Include=""System.Runtime.Serialization.Formatters"" Version=""4.3.0"" />
		<PackageReference Include=""System.Runtime.Serialization.Json"" Version=""4.0.2"" />
		<PackageReference Include=""System.Threading.Thread"" Version=""4.0.0"" />
		<PackageReference Include=""System.ValueTuple"" Version=""4.3.0"" />
		<PackageReference Include=""System.Xml.XmlDocument"" Version=""4.0.1"" />
	</ItemGroup>
</Project>
";
			#endregion

			public static readonly string Infrastructure_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>netstandard1.6</TargetFramework>
		<AssemblyName>Infrastructure</AssemblyName>
	</PropertyGroup>
	<ItemGroup>
		<ProjectReference Include=""..\{0}.db\{0}.db.csproj"" />
	</ItemGroup>
	<ItemGroup>
		<PackageReference Include=""Microsoft.AspNetCore.Mvc"" Version=""1.0.3"" />
		<PackageReference Include=""Microsoft.AspNetCore.Diagnostics"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.EnvironmentVariables"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.FileExtensions"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.Json"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.AspNetCore.Session"" Version=""1.0.2"" />
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
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

public static class StarupExtensions {{
	public static ConfigurationBuilder LoadInstalledModules(this ConfigurationBuilder build, IList<ModuleInfo> modules, IHostingEnvironment env) {{
		var moduleRootFolder = new DirectoryInfo(Path.Combine(env.ContentRootPath, ""Module""));
		var moduleFolders = moduleRootFolder.GetDirectories();

		foreach (var moduleFolder in moduleFolders) {{
			var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, ""bin""));
			if (!binFolder.Exists) continue;

			foreach (var file in binFolder.GetFileSystemInfos(""*.dll"", SearchOption.AllDirectories)) {{
				Assembly assembly;
				try {{
					assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
				}} catch (FileLoadException) {{
					assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));
					if (assembly == null) throw;
				}}
				if (assembly.FullName.Contains(moduleFolder.Name))
					modules.Add(new ModuleInfo {{
						Name = moduleFolder.Name,
						Assembly = assembly,
						Path = moduleFolder.FullName
					}});
			}}
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
			build.AddJsonFile(Path.Combine(productPath, ""/appsettings.json""), true, true);
			foreach (var module in modules) {{
				var jsonpath = Path.Combine(productPath, $""/Module_{{module.Name}}_appsettings.json"");
				if (File.Exists(Path.Combine(env.ContentRootPath, jsonpath)))
					build.AddJsonFile(jsonpath, true, true);
			}}
		}}
		return build;
	}}

	public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules) {{
		var mvcBuilder = services.AddMvc()
			.AddRazorOptions(o => {{
				foreach (var module in modules) {{
					var a = MetadataReference.CreateFromFile(module.Assembly.Location);
					o.AdditionalCompilationReferences.Add(a);
				}}
			}})
			.AddViewLocalization()
			.AddDataAnnotationsLocalization();

		foreach (var module in modules) {{
			mvcBuilder.AddApplicationPart(module.Assembly);

			var moduleInitializerType =
				module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
			if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer))) {{
				var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
				moduleInitializer.Init(services);
			}}
		}}

		return services;
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
 @"using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Swashbuckle.AspNetCore.Swagger {{
	public class FormDataOperationFilter : IOperationFilter {{
		public void Apply(Operation operation, OperationFilterContext context) {{
			var actattrs = context.ApiDescription.ActionAttributes();
			if (actattrs.OfType<HttpPostAttribute>().Any() ||
				actattrs.OfType<HttpPutAttribute>().Any())
				operation.Consumes = new[] {{ ""multipart/form-data"" }};
		}}
	}}

	public static class SwashbuckleSwaggerExtensions {{
		public static IServiceCollection AddCustomizedSwaggerGen(this IServiceCollection services) {{
			services.AddSwaggerGen(options => {{
				options.SwaggerDoc(""v1"", new Info {{
					Version = ""v1"",
					Title = ""WebAPI"",
					Description = ""项目webapi接口说明"",
					TermsOfService = ""None"",
					Contact = new Contact {{ Name = ""duoyi"", Email = """", Url = ""http://duoyi.com"" }},
					License = new License {{ Name = ""duoyi"", Url = ""http://duoyi.com"" }}
				}});
				options.IgnoreObsoleteActions();
				//options.IgnoreObsoleteControllers(); // 类、方法标记 [Obsolete]，可以阻止【Swagger文档】生成
				options.DescribeAllEnumsAsStrings();
				//options.IncludeXmlComments(AppContext.BaseDirectory + @""/Admin.xml""); // 使用前需开启项目注释 xmldoc
				options.OperationFilter<FormDataOperationFilter>();
			}});
			return services;
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

		<target xsi:type=""Null"" name=""blackhole"" />
	</targets>

	<rules>
		<logger name=""*"" minlevel=""Error"" writeTo=""allfile"" />
		<logger name=""Microsoft.*"" minlevel=""Error"" writeTo=""blackhole"" final=""true"" />
		<logger name=""*"" minlevel=""Error"" writeTo=""ownFile-web"" />
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
		""MySql"": ""{{connectionString}};Max pool size=32"",
		""redis"": {{
			""ip"": ""172.16.1.220"",
			""port"": 6379,
			""pass"": ""123456"",
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
using Microsoft.Extensions.Caching.Redis;
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
				return st;
			}};
		}}

		public static IList<ModuleInfo> Modules = new List<ModuleInfo>();
		public IConfigurationRoot Configuration {{ get; }}
		public IHostingEnvironment env {{ get; }}

		public void ConfigureServices(IServiceCollection services) {{
			services.AddSingleton<IDistributedCache>(new RedisCache());
			services.AddSingleton<IConfigurationRoot>(Configuration);
			services.AddSingleton<IHostingEnvironment>(env);

			services.AddSession(a => {{
				a.IdleTimeout = TimeSpan.FromMinutes(30);
				a.CookieName = ""Session_{0}"";
			}});
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
			env.ConfigureNLog(""nlog.config"");

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			{0}.BLL.RedisHelper.InitializeConfiguration(Configuration);
			{0}.DAL.SqlHelper.Instance.Log = loggerFactory.CreateLogger(""{0}_DAL_sqlhelper"");

			app.UseSession();
			app.UseMvc();
			app.UseCustomizedStaticFiles(Modules);

			if (env.IsDevelopment())
				app.UseSwagger().UseSwaggerUI(options => {{
					options.SwaggerEndpoint(""/swagger/v1/swagger.json"", ""V1 Docs"");
				}});
		}}
	}}
}}
";
			#endregion
			public static readonly string WebHost_csproj =
			#region 内容太长已被收起
 @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
	<PropertyGroup>
		<TargetFramework>netcoreapp1.0</TargetFramework>
		<PreserveCompilationContext>true</PreserveCompilationContext>
		<AssemblyName>WebHost</AssemblyName>
		<OutputType>Exe</OutputType>
		<RuntimeFrameworkVersion>1.0.4</RuntimeFrameworkVersion>
		<PackageTargetFallback>$(PackageTargetFallback);dotnet5.6;portable-net45+win8</PackageTargetFallback>
	</PropertyGroup>
	<ItemGroup>
		<None Update=""wwwroot\**\*;Views;Module\**\*;nlog.config"">
			<CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
		</None>
	</ItemGroup>
	<ItemGroup>
		<ProjectReference Include=""..\Infrastructure\Infrastructure.csproj"" />
	</ItemGroup>
	<ItemGroup>
		<PackageReference Include=""Microsoft.AspNetCore.Mvc"" Version=""1.0.3"" />
		<PackageReference Include=""Microsoft.AspNetCore.Server.IISIntegration"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.AspNetCore.Server.Kestrel"" Version=""1.0.3"" />
		<PackageReference Include=""Microsoft.AspNetCore.Diagnostics"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.EnvironmentVariables"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.FileExtensions"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Configuration.Json"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Logging.Console"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.Extensions.Logging.Debug"" Version=""1.0.2"" />
		<PackageReference Include=""Microsoft.AspNetCore.Session"" Version=""1.0.2"" />
		<PackageReference Include=""NLog.Extensions.Logging"" Version=""1.0.0-rtm-alpha4"" />
		<PackageReference Include=""System.Text.Encoding.CodePages"" Version=""4.0.1"" />
		<PackageReference Include=""Swashbuckle.AspNetCore.SwaggerGen"" Version=""1.0.0-*"" />
		<PackageReference Include=""Swashbuckle.AspNetCore.SwaggerUi"" Version=""1.0.0-*"" />
	</ItemGroup>
	<ItemGroup>
		<DotNetCliToolReference Include=""Microsoft.DotNet.Watcher.Tools"" Version=""1.0.0"" />
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
				List = ret
			}};
		}}
		[HttpGet(@""connection/redis"")]
		public object Get_connection_redis() {{
			List<Hashtable> ret = new List<Hashtable>();
			foreach (var conn in RedisHelper.Instance.AllConnections) {{
				ret.Add(new Hashtable() {{
						{{ ""数据库"", conn.Client.ClientGetName() }},
						{{ ""最后活动"", conn.LastActive }},
						{{ ""获取次数"", conn.UseSum }}
					}});
			}}
			return new {{
				FreeConnections = RedisHelper.Instance.FreeConnections.Count,
				AllConnections = RedisHelper.Instance.AllConnections.Count,
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
		public ActionResult List([FromServices]IConfigurationRoot cfg, {12}[FromQuery] int limit = 20, [FromQuery] int page = 1) {{
			var select = {1}.Select{8};{9}
			int count;
			var items = select.Count(out count){14}.Skip((page - 1) * limit).Limit(limit).ToList();
			ViewBag.items = items;
			ViewBag.count = count;
			return View();
		}}

		[HttpGet(@""add"")]
		public ActionResult Edit() {{
			return View();
		}}
		[HttpGet(@""edit"")]
		public ActionResult Edit({4}) {{
			{1}Info item = {1}.GetItem({5});
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			ViewBag.item = item;
			return View();
		}}

		/***************************************** POST *****************************************/
		[HttpPost(@""add"")]
		[ValidateAntiForgeryToken]
		public APIReturn _Add({10}) {{
			{1}Info item = new {1}Info();{13}{7}
			item = {1}.Insert(item);{16}
			return APIReturn.成功.SetData(""item"", item.ToBson());
		}}
		[HttpPost(@""edit"")]
		[ValidateAntiForgeryToken]
		public APIReturn _Edit({4}{11}) {{
			{1}Info item = {1}.GetItem({5});
			if (item == null) return APIReturn.记录不存在_或者没有权限;{6}{7}
			int affrows = {1}.Update(item);{17}
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
using NpgsqlTypes;
using {0}.BLL;
using {0}.Model;

namespace {0}.Module.Admin.Controllers {{
	[Route(""[controller]"")]
	public class {1}Controller : BaseController {{
		public {1}Controller(ILogger<{1}Controller> logger) : base(logger) {{ }}

		[HttpGet]
		public ActionResult List([FromServices]IConfigurationRoot cfg) {{
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
		<TargetFramework>netstandard1.6</TargetFramework>
		<AssemblyName>{0}</AssemblyName>
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
