using Model;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace GenMy {
	public class ConsoleApp {

		ClientInfo _client;
		ClientSocket _socket;
		public string ConnectionString {
			get {
				string connStr = "Data Source={0};Port={1};User ID={2};Password={3};Initial Catalog={4};Charset=utf8";
				return string.Format(connStr, this._client.Server, this._client.Port, this._client.Username, this._client.Password, string.IsNullOrEmpty(this._client.Database) ? "" : this._client.Database.Split(',')[0]);
			}
		}
		public string Server;
		public int Port;
		public string Username;
		public string Password;
		public string Database;

		public string SolutionName;
		public bool IsMakeSolution;
		public bool IsMakeWebAdmin;
		public bool IsDownloadRes;
		public string OutputPath;

		public ConsoleApp(string[] args, ManualResetEvent wait) {
			this.OutputPath = Directory.GetCurrentDirectory();
			string args0 = args[0].Trim().ToLower();
			if (args[0] == "?" || args0 == "--help" || args0 == "-help") {
				var bgcolor = Console.BackgroundColor;
				var fgcolor = Console.ForegroundColor;

				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.Write("#################################");
				Console.Write("##");
				Console.BackgroundColor = bgcolor;
				Console.ForegroundColor = fgcolor;
				Console.WriteLine("");

				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Write("                                 ");
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.BackgroundColor = bgcolor;
				Console.ForegroundColor = fgcolor;
				Console.WriteLine("");

				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Write("   .NETCore 2.1 + MySQL 生成器   ");
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.BackgroundColor = bgcolor;
				Console.ForegroundColor = fgcolor;
				Console.WriteLine("");

				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Write("                                 ");
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.BackgroundColor = bgcolor;
				Console.ForegroundColor = fgcolor;
				Console.WriteLine("");

				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("##");
				Console.Write("#################################");
				Console.Write("##");

				Console.BackgroundColor = bgcolor;
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.Write(@"

用于快速创建和更新 .NETCore 2.1 + MySQL 项目，非常合适敏捷开发；
Github: https://github.com/2881099/dotnetgen_mysql

");
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.Write("Example：");
				Console.ForegroundColor = fgcolor;
				Console.WriteLine(@"

	> GenMy 127.0.0.1[:3306] -U root -P 123456 -D dyschool -N dyschool -S -A -R -O ""c:/dyschool/""

		-U	MySql账号
		-P	MySql密码
		-D	需要生成的数据库，多个使用,分隔

		-N	字符串，生成代码的解决方案名，命名空间
		-S	生成解决方案，在项目第一次生成时使用
		-A	生成后台管理
		-R	下载资源

		-O	输出路径(默认:当前目录)");
				wait.Set();
				return;
			}
			string[] ss = args[0].Split(new char[] { ':' }, 2);
			this.Server = ss[0];
			if (int.TryParse(ss.Length == 2 ? ss[1] : "3306", out this.Port) == false) this.Port = 3306;
			for (int a = 1; a < args.Length; a++) {
				switch (args[a]) {
					case "-U":
						if (a + 1 >= args.Length) Console.WriteLine("-U 参数错误");
						else this.Username = args[a + 1];
						a++;
						break;
					case "-P":
						if (a + 1 >= args.Length) Console.WriteLine("-P 参数错误");
						else this.Password = args[a + 1];
						a++;
						break;
					case "-D":
						if (a + 1 >= args.Length) Console.WriteLine("-D 参数错误");
						else this.Database = args[a + 1];
						a++;
						break;
					case "-O":
						if (a + 1 >= args.Length) Console.WriteLine("-O 参数错误");
						else this.OutputPath = args[a + 1];
						a++;
						break;
					case "-N":
						if (a + 1 >= args.Length) Console.WriteLine("-N 参数错误");
						else this.SolutionName = args[a + 1];
						a++;
						break;
					case "-S":
						this.IsMakeSolution = true;
						break;
					case "-A":
						this.IsMakeWebAdmin = true;
						break;
					case "-R":
						this.IsDownloadRes = true;
						break;
				}
			}
			this._client = new ClientInfo(this.Server, this.Port, this.Username, this.Password);
			StreamReader sr = new StreamReader(System.Net.WebRequest.Create("https://files.cnblogs.com/files/kellynic/GenMy_server.css").GetResponse().GetResponseStream(), Encoding.UTF8);
			string server = sr.ReadToEnd()?.Trim();
			//server = "127.0.0.1:18888";
			Uri uri = new Uri("tcp://" + server + "/");
			this._socket = new ClientSocket();
			this._socket.Error += Socket_OnError;
			this._socket.Receive += Socket_OnReceive;
			this._socket.Connect(uri.Host, uri.Port);
			Thread.CurrentThread.Join(TimeSpan.FromSeconds(1));
			if (this._socket.Running == false) {
				wait.Set();
				return;
			}

			SocketMessager messager = new SocketMessager("GetDatabases", this._client);
			this._socket.Write(messager, delegate (object sender2, ClientSocketReceiveEventArgs e2) {
				List<DatabaseInfo> dbs = e2.Messager.Arg as List<DatabaseInfo>;
			});
			this._client.Database = this.Database;
			List<TableInfo> tables = null;
			messager = new SocketMessager("GetTablesByDatabase", this._client.Database);
			this._socket.Write(messager, delegate (object sender2, ClientSocketReceiveEventArgs e2) {
				tables = e2.Messager.Arg as List<TableInfo>;
			});
			if (tables == null) {
				Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "] 无法读取表");
				this._socket.Close();
				this._socket.Dispose();

				wait.Set();
				return;
			}
			tables.ForEach(a => a.IsOutput = true);
			List<BuildInfo> bs = null;
			messager = new SocketMessager("Build", new object[] {
				SolutionName,
				IsMakeSolution,
				string.Join("", tables.ConvertAll<string>(delegate(TableInfo table){
					return string.Concat(table.IsOutput ? 1 : 0);
				}).ToArray()),
				IsMakeWebAdmin,
				IsDownloadRes
			});
			this._socket.Write(messager, delegate (object sender2, ClientSocketReceiveEventArgs e2) {
				bs = e2.Messager.Arg as List<BuildInfo>;
				if (e2.Messager.Arg is Exception) throw e2.Messager.Arg as Exception;
			}, TimeSpan.FromSeconds(60 * 5));
			if (bs != null) {
				foreach (BuildInfo b in bs) {
					string path = Path.Combine(OutputPath, b.Path);
					Directory.CreateDirectory(Path.GetDirectoryName(path));
					string fileName = Path.GetFileName(b.Path);
					string ext = Path.GetExtension(b.Path);
					Encoding encode = Encoding.UTF8;

					if (fileName.EndsWith(".rar") || fileName.EndsWith(".zip") || fileName.EndsWith(".dll")) {
						using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
							fs.Write(b.Data, 0, b.Data.Length);
							fs.Close();
						}
						continue;
					}

					byte[] data = Deflate.Decompress(b.Data);
					string content = Encoding.UTF8.GetString(data);

					if (string.Compare(fileName, "web.config") == 0) {
						string place = System.Web.HttpUtility.HtmlEncode(this.ConnectionString);
						content = content.Replace("{connectionString}", place);
					}
					if (fileName.EndsWith(".json")) {
						content = content.Replace("{connectionString}", this.ConnectionString);
					}
					if (string.Compare(ext, ".refresh") == 0) {
						encode = Encoding.Unicode;
					}
					using (StreamWriter sw = new StreamWriter(path, false, encode)) {
						sw.Write(content);
						sw.Close();
					}
				}
				var appsettingsPath = Path.Combine(OutputPath, "appsettings.json");
				var appsettingsPathWebHost = Path.Combine(OutputPath, @"src\WebHost\appsettings.json");
				//如果三个选项为false，并且 src\WebHost\appsettings.json 不存在，则在当前目录使用 appsettings.json
				if (this.IsDownloadRes == false && this.IsMakeSolution == false && this.IsMakeWebAdmin == false && File.Exists(appsettingsPathWebHost) == false) {
					var appsettings = Newtonsoft.Json.JsonConvert.DeserializeObject(File.Exists(appsettingsPath) ? File.ReadAllText(appsettingsPath) : "{}") as JToken;
					var oldtxt = appsettings.ToString();
					if (appsettings["ConnectionStrings"] == null) appsettings["ConnectionStrings"] = new JObject();
					if (appsettings["ConnectionStrings"]["MySql"] == null) appsettings["ConnectionStrings"]["MySql"] = this.ConnectionString + ";Encrypt=False;Max pool size=100";
					if (appsettings["ConnectionStrings"]["redis"] == null) appsettings["ConnectionStrings"]["redis"] = JToken.FromObject(new {
						ip = "127.0.0.1", port = 6379, pass = "", database = 13, poolsize = 50, name = this.SolutionName
					});
					if (appsettings[$"{this.SolutionName}_BLL_ITEM_CACHE"] == null) appsettings[$"{this.SolutionName}_BLL_ITEM_CACHE"] = JToken.FromObject(new {
						Timeout = 180
					});
					if (appsettings["Logging"] == null) appsettings["Logging"] = new JObject();
					if (appsettings["Logging"]["IncludeScopes"] == null) appsettings["Logging"]["IncludeScopes"] = false;
					if (appsettings["Logging"]["LogLevel"] == null) appsettings["Logging"]["LogLevel"] = new JObject();
					if (appsettings["Logging"]["LogLevel"]["Default"] == null) appsettings["Logging"]["LogLevel"]["Default"] = "Debug";
					if (appsettings["Logging"]["LogLevel"]["System"] == null) appsettings["Logging"]["LogLevel"]["System"] = "Information";
					if (appsettings["Logging"]["LogLevel"]["Microsoft"] == null) appsettings["Logging"]["LogLevel"]["Microsoft"] = "Information";
					var newtxt = appsettings.ToString();
					if (newtxt != oldtxt) File.WriteAllText(appsettingsPath, newtxt, Encoding.UTF8);
					//增加当前目录 .csproj nuguet 引用 <PackageReference Include="dng.Mysql" Version="1.0.4" />
					string csprojPath = Directory.GetFiles(OutputPath, "*.csproj").FirstOrDefault();
					if (!string.IsNullOrEmpty(csprojPath) && File.Exists(csprojPath)) {
						if (Regex.IsMatch(File.ReadAllText(csprojPath), @"dng\.Mysql""\s+Version=""1\.0\.4", RegexOptions.IgnoreCase) == false) {
							System.Diagnostics.Process pro = new System.Diagnostics.Process();
							pro.StartInfo = new System.Diagnostics.ProcessStartInfo("dotnet", "add package dng.Mysql --version 1.0.4") {
								WorkingDirectory = OutputPath
							};
							pro.Start();
							pro.WaitForExit();
						}
					}
				}
				if (File.Exists(Path.Combine(OutputPath, "GenMy只更新db.bat")) == false) {
					var batPath = Path.Combine(OutputPath, $"GenMy_{this.SolutionName}_{this.Server}_{this.Database}.bat");
					if (File.Exists(batPath) == false) File.WriteAllText(batPath, $@"
GenMy {this.Server}:{this.Port} -U {this.Username} -P {this.Password} -D {this.Database} -N {this.SolutionName}");
				}
			}
			this._socket.Close();
			this._socket.Dispose();
			GC.Collect();

			ConsoleColor fc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "] The code files be maked in \"" + OutputPath + "\", please check.");
			Console.ForegroundColor = fc;
			wait.Set();
		}
		private void Socket_OnError(object sender, ClientSocketErrorEventArgs e) {
			Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "] " + e.Exception.Message);
		}

		private void Socket_OnReceive(object sender, ClientSocketReceiveEventArgs e) {
			SocketMessager messager = null;
			switch (e.Messager.Action) {
				case "ExecuteDataSet":
					string sql = e.Messager.Arg.ToString();
					DataSet ds = null;
					try {
						ds = ConsoleApp.ExecuteDataSet(this.ConnectionString, sql);
					} catch (Exception ex) {
						this.Socket_OnError(this, new ClientSocketErrorEventArgs(ex, 0));
					}
					messager = new SocketMessager(e.Messager.Action, ds);
					messager.Id = e.Messager.Id;
					this._socket.Write(messager);
					break;
				case "ExecuteNonQuery":
					string sql2 = e.Messager.Arg.ToString();
					int val = 0;
					try {
						val = ConsoleApp.ExecuteNonQuery(this.ConnectionString, sql2);
					} catch (Exception ex) {
						this.Socket_OnError(this, new ClientSocketErrorEventArgs(ex, 0));
					}
					messager = new SocketMessager(e.Messager.Action, val);
					messager.Id = e.Messager.Id;
					this._socket.Write(messager);
					break;
				default:
					Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "] " + "您当前使用的版本未能实现功能！");
					break;
			}
		}

		public static int ExecuteNonQuery(string connectionString, string cmdText) {
			int val = 0;
			using (MySqlConnection conn = new MySqlConnection(connectionString + ";Encrypt=False")) {
				MySqlCommand cmd = new MySqlCommand(cmdText, conn);
				try {
					cmd.Connection.Open();
					val = cmd.ExecuteNonQuery();
				} catch {
					cmd.Parameters.Clear();
					cmd.Connection.Close();
					throw;
				}
			}
			return val;
		}
		public static DataSet ExecuteDataSet(string connectionString, string cmdText) {
			DataSet ds = new DataSet();
			using (MySqlConnection conn = new MySqlConnection(connectionString + ";Encrypt=False")) {
				MySqlCommand cmd = new MySqlCommand(cmdText, conn);
				MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
				try {
					cmd.Connection.Open();
					sda.Fill(ds);
				} catch {
					cmd.Parameters.Clear();
					cmd.Connection.Close();
					throw;
				}
				cmd.Connection.Close();
				cmd.Parameters.Clear();
			}
			return ds;
		}
	}
}

