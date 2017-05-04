using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

/// <summary>
/// 作业调度器，一般运行在控制台
/// </summary>
public class Robot : IDisposable {

	private string _def_path;
	private List<RobotDef> _robots;
	private Dictionary<string, RobotDef> _dic_robots = new Dictionary<string, RobotDef>();
	private object _robots_lock = new object();
	private FileSystemWatcher _defWatcher;
	public event RobotErrorHandler Error;
	public event RobotRunHandler Run;

	public Robot()
		: this(Path.Combine(AppContext.BaseDirectory, @"robot.txt")) {
	}
	public Robot(string path) {
		_def_path = path;
	}

	public void Start() {
		lock (_robots_lock) {
			_dic_robots.Clear();
			if (_robots != null) {
				for (int a = 0; a < _robots.Count; a++)
					_dic_robots.Add(_robots[a].Name, _robots[a]);
				_robots.Clear();
			}
		}
		if (!File.Exists(_def_path)) return;
		lock (_robots_lock) {
			_robots = LoadDef();
			foreach (RobotDef bot in _robots)
				if (bot._timer == null) bot.RunNow();
		}
		if (_defWatcher == null) {
			_defWatcher = new FileSystemWatcher(Path.GetDirectoryName(_def_path), Path.GetFileName(_def_path));
			_defWatcher.Changed += delegate(object sender, FileSystemEventArgs e) {
				_defWatcher.EnableRaisingEvents = false;
				if (_robots.Count > 0) {
					Start();
				}
				_defWatcher.EnableRaisingEvents = true;
			};
			_defWatcher.EnableRaisingEvents = true;
		}
	}
	public void Stop() {
		lock (_robots_lock) {
			if (_robots != null) {
				for (int a = 0; a < _robots.Count; a++)
					_robots[a].Dispose();
				_robots.Clear();
			}
		}
	}

	#region IDisposable 成员

	public void Dispose() {
		if (_defWatcher != null)
			_defWatcher.Dispose();
		Stop();
	}

	#endregion

	public List<RobotDef> LoadDef() {
		string defDoc = Encoding.UTF8.GetString(readFile(_def_path));
		return LoadDef(defDoc);
	}
	public List<RobotDef> LoadDef(string defDoc) {
		Dictionary<string, RobotDef> dic = new Dictionary<string, RobotDef>();
		string[] defs = defDoc.Split(new string[] { "\n" }, StringSplitOptions.None);
		int row = 1;
		foreach (string def in defs) {
			string loc1 = def.Trim().Trim('\r');
			if (string.IsNullOrEmpty(loc1) || loc1[0] == 65279 || loc1[0] == ';' || loc1[0] == '#') continue;
			string pattern = @"([^\s]+)\s+(NONE|SEC|MIN|HOUR|DAY|RunOnDay|RunOnWeek|RunOnMonth)\s+([^\s]+)\s+([^\s]+)";
			Match m = Regex.Match(loc1, pattern, RegexOptions.IgnoreCase);
			if (!m.Success) {
				onError(new Exception("Robot配置错误“" + loc1 + "”, 第" + row + "行"));
				continue;
			}
			string name = m.Groups[1].Value.Trim('\t', ' ');
			RobotRunMode mode = getMode(m.Groups[2].Value.Trim('\t', ' '));
			string param = m.Groups[3].Value.Trim('\t', ' ');
			string runParam = m.Groups[4].Value.Trim('\t', ' ');
			if (dic.ContainsKey(name)) {
				onError(new Exception("Robot配置存在重复的名字“" + name + "”, 第" + row + "行"));
				continue;
			}
			if (mode == RobotRunMode.NONE) continue;

			RobotDef rd = null;
			if (_dic_robots.ContainsKey(name)) {
				rd = _dic_robots[name];
				rd.Update(mode, param, runParam);
				_dic_robots.Remove(name);
			} else rd = new RobotDef(this, name, mode, param, runParam);
			if (rd.Interval < 0) {
				onError(new Exception("Robot配置参数错误“" + def + "”, 第" + row + "行"));
				continue;
			}
			dic.Add(rd.Name, rd);
			row++;
		}
		List<RobotDef> rds = new List<RobotDef>();
		foreach (RobotDef rd in dic.Values)
			rds.Add(rd);
		foreach (RobotDef stopBot in _dic_robots.Values)
			stopBot.Dispose();

		return rds;
	}

	private void onError(Exception ex) {
		onError(ex, null);
	}
	internal void onError(Exception ex, RobotDef def) {
		if (Error != null)
			Error(this, new RobotErrorEventArgs(ex, def));
	}
	internal void onRun(RobotDef def) {
		if (Run != null)
			Run(this, def);
	}
	private byte[] readFile(string path) {
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
	private RobotRunMode getMode(string mode) {
		mode = string.Concat(mode).ToUpper();
		switch (mode) {
			case "SEC": return RobotRunMode.SEC;
			case "MIN": return RobotRunMode.MIN;
			case "HOUR": return RobotRunMode.HOUR;
			case "DAY": return RobotRunMode.DAY;
			case "RUNONDAY": return RobotRunMode.RunOnDay;
			case "RUNONWEEK": return RobotRunMode.RunOnWeek;
			case "RUNONMONTH": return RobotRunMode.RunOnMonth;
			default: return RobotRunMode.NONE;
		}
	}
}

public class RobotDef : IDisposable {
	private string _name;
	private RobotRunMode _mode = RobotRunMode.NONE;
	private string _param;
	private string _runParam;
	private int _runTimes = 0;
	private int _errTimes = 0;

	private Robot _onwer;
	internal Timer _timer;
	private bool _timerIntervalOverflow = false;

	public RobotDef(Robot onwer, string name, RobotRunMode mode, string param, string runParam) {
		_onwer = onwer;
		_name = name;
		_mode = mode;
		_param = param;
		_runParam = runParam;
	}
	public void Update(RobotRunMode mode, string param, string runParam) {
		if (_mode != mode || _param != param || _runParam != runParam) {
			_mode = mode;
			_param = param;
			_runParam = runParam;
			if (_timer != null) {
				_timer.Dispose();
				_timer = null;
			}
			RunNow();
		}
	}

	public void RunNow() {
		double tmp = this.Interval;
		_timerIntervalOverflow = tmp > int.MaxValue;
		int interval = _timerIntervalOverflow ? int.MaxValue : (int)tmp;
		if (interval <= 0) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("		{0} Interval <= 0", _name);
			Console.ResetColor();
			return;
		}
		//Console.WriteLine(interval);
		if (_timer == null) {
			_timer = new Timer(a => {
				if (_timerIntervalOverflow) {
					RunNow();
					return;
				}
				_runTimes++;
				string logObj = this.ToString();
				try {
					_onwer.onRun(this);
				} catch (Exception ex) {
					_errTimes++;
					_onwer.onError(ex, this);
				}
				RunNow();
			}, null, interval, -1);
		} else {
			_timer.Change(interval, -1);
		}
		if (tmp > 1000 * 9) {
			DateTime nextTime = DateTime.Now.AddMilliseconds(tmp);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("		{0} 下次触发时间：{1:yyyy-MM-dd HH:mm:ss}", _name, nextTime);
			Console.ResetColor();
		}
	}

	public override string ToString() {
		return Name + ", " + Mode + ", " + Param + ", " + RunParam;
	}

	#region IDisposable 成员

	public void Dispose() {
		if (_timer != null) {
			_timer.Dispose();
			_timer = null;
		}
	}

	#endregion

	public string Name { get { return _name; } }
	public RobotRunMode Mode { get { return _mode; } }
	public string Param { get { return _param; } }
	public string RunParam { get { return _runParam; } }
	public int RunTimes { get { return _runTimes; } }
	public int ErrTimes { get { return _errTimes; } }

	public double Interval {
		get {
			DateTime now = DateTime.Now;
			DateTime curt = DateTime.MinValue;
			TimeSpan ts = TimeSpan.Zero;
			uint ww = 0, dd = 0, hh = 0, mm = 0, ss = 0;
			double interval = -1;
			switch (_mode) {
				case RobotRunMode.SEC:
					double.TryParse(_param, out interval);
					interval *= 1000;
					break;
				case RobotRunMode.MIN:
					double.TryParse(_param, out interval);
					interval *= 60 * 1000;
					break;
				case RobotRunMode.HOUR:
					double.TryParse(_param, out interval);
					interval *= 60 * 60 * 1000;
					break;
				case RobotRunMode.DAY:
					double.TryParse(_param, out interval);
					interval *= 24 * 60 * 60 * 1000;
					break;
				case RobotRunMode.RunOnDay:
					List<string> hhmmss = new List<string>(string.Concat(_param).Split(':'));
					if (hhmmss.Count == 3)
						if (uint.TryParse(hhmmss[0], out hh) && hh < 24 &&
							uint.TryParse(hhmmss[1], out mm) && mm < 60 &&
							uint.TryParse(hhmmss[2], out ss) && ss < 60) {
							curt = now.Date.AddHours(hh).AddMinutes(mm).AddSeconds(ss);
							ts = curt.Subtract(now);
							while (!(ts.TotalMilliseconds > 0)) {
								curt = curt.AddDays(1);
								ts = curt.Subtract(now);
							}
							interval = ts.TotalMilliseconds;
						}
					break;
				case RobotRunMode.RunOnWeek:
					string[] wwhhmmss = string.Concat(_param).Split(':');
					if (wwhhmmss.Length == 4)
						if (uint.TryParse(wwhhmmss[0], out ww) && ww < 7 &&
							uint.TryParse(wwhhmmss[1], out hh) && hh < 24 &&
							uint.TryParse(wwhhmmss[2], out mm) && mm < 60 &&
							uint.TryParse(wwhhmmss[3], out ss) && ss < 60) {
							curt = now.Date.AddHours(hh).AddMinutes(mm).AddSeconds(ss);
							ts = curt.Subtract(now);
							while(!(ts.TotalMilliseconds > 0 && (int)curt.DayOfWeek == ww)) {
								curt = curt.AddDays(1);
								ts = curt.Subtract(now);
							}
							interval = ts.TotalMilliseconds;
						}
					break;
				case RobotRunMode.RunOnMonth:
					string[] ddhhmmss = string.Concat(_param).Split(':');
					if (ddhhmmss.Length == 4)
						if (uint.TryParse(ddhhmmss[0], out dd) && dd > 0 && dd < 32 &&
							uint.TryParse(ddhhmmss[1], out hh) && hh < 24 &&
							uint.TryParse(ddhhmmss[2], out mm) && mm < 60 &&
							uint.TryParse(ddhhmmss[3], out ss) && ss < 60) {
							curt = new DateTime(now.Year, now.Month, (int)dd).AddHours(hh).AddMinutes(mm).AddSeconds(ss);
							ts = curt.Subtract(now);
							while (!(ts.TotalMilliseconds > 0)) {
								curt = curt.AddMonths(1);
								ts = curt.Subtract(now);
							}
							interval = ts.TotalMilliseconds;
						}
					break;
			}
			if (interval == 0) interval = 1;
			return interval;
		}
	}
}
/*
; 和 # 匀为行注释
;SEC：					按秒触发
;MIN：					按分触发
;HOUR：					按时触发
;DAY：					按天触发
;RunOnDay：				每天 什么时间 触发
;RunOnWeek：			星期几 什么时间 触发
;RunOnMonth：			每月 第几天 什么时间 触发

;Name1		SEC			2				/schedule/test002.aspx
;Name2		MIN			2				/schedule/test002.aspx
;Name3		HOUR		1				/schedule/test002.aspx
;Name4		DAY			2				/schedule/test002.aspx
;Name5		RunOnDay	15:55:59		/schedule/test002.aspx
;每天15点55分59秒
;Name6		RunOnWeek	1:15:55:59		/schedule/test002.aspx
;每星期一15点55分59秒
;Name7		RunOnMonth	1:15:55:59		/schedule/test002.aspx
;每月1号15点55分59秒
*/
public enum RobotRunMode {
	NONE = 0,
	SEC = 1,
	MIN = 2,
	HOUR = 3,
	DAY = 4,
	RunOnDay = 11,
	RunOnWeek = 12,
	RunOnMonth = 13
}

public delegate void RobotErrorHandler(object sender, RobotErrorEventArgs e);
public delegate void RobotRunHandler(object sender, RobotDef e);
public class RobotErrorEventArgs : EventArgs {

	private Exception _exception;
	private RobotDef _def;

	public RobotErrorEventArgs(Exception exception, RobotDef def) {
		_exception = exception;
		_def = def;
	}

	public Exception Exception {
		get { return _exception; }
	}
	public RobotDef Def {
		get { return _def; }
	}
}