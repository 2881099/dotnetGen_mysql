using System;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace Server {
	/// <summary>
	/// 日志管理器
	/// </summary>
	[Serializable]
	public class Logger {
		protected readonly string _Name;
		private log4net.ILog _Log;
		protected log4net.ILog Log {
			get {
				if (_Log == null) _Log = log4net.LogManager.GetLogger(_Name);
				return _Log;
			}
		}

		protected Logger() { }
		public Logger(string name) { this._Name = name; }

		/// <summary>
		/// 全局日志
		/// </summary>
		public static readonly Logger remotor = new Logger("remotor");

		public void Debug(object message, Exception exception) {
			Log.Debug(message, exception);
		}

		public void Debug(object message) {
			Log.Debug(message);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
			Log.DebugFormat(provider, format, args);
		}

		public void DebugFormat(string format, params object[] args) {
			Log.DebugFormat(format, args);
		}

		public void Error(object message, Exception exception) {
			Log.Error(message, exception);
		}

		public void Error(object message) {
			Log.Error(message);
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
			Log.ErrorFormat(provider, format, args);
		}

		public void ErrorFormat(string format, params object[] args) {
			Log.ErrorFormat(format, args);
		}

		public void Fatal(object message, Exception exception) {
			Log.Fatal("〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓" + message, exception);
		}

		public void Fatal(object message) {
			Log.Fatal("〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓" + message);
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
			Log.FatalFormat(provider, format, args);
		}

		public void FatalFormat(string format, params object[] args) {
			Log.FatalFormat(format, args);
		}

		public void Info(object message, Exception exception) {
			Log.Info(message, exception);
		}

		public void Info(object message) {
			Log.Info(message);
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
			Log.InfoFormat(provider, format, args);
		}

		public void InfoFormat(string format, params object[] args) {
			Log.InfoFormat(format, args);
		}

		public bool IsDebugEnabled {
			get { return Log.IsDebugEnabled; }
		}

		public bool IsErrorEnabled {
			get { return Log.IsErrorEnabled; }
		}

		public bool IsFatalEnabled {
			get { return Log.IsFatalEnabled; }
		}

		public bool IsInfoEnabled {
			get { return Log.IsInfoEnabled; }
		}

		public bool IsWarnEnabled {
			get { return Log.IsWarnEnabled; }
		}

		public void Warn(object message, Exception exception) {
			Log.Warn(message, exception);
		}

		public void Warn(object message) {
			Log.Warn(message);
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
			Log.WarnFormat(provider, format, args);
		}

		public void WarnFormat(string format, params object[] args) {
			Log.WarnFormat(format, args);
		}
	}
}
