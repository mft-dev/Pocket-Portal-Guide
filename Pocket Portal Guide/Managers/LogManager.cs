using System;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Manages access to the built in Logger that comes with BepInEx
	/// </summary>
	class LogManager
	{
		private Action<BepInEx.Logging.LogLevel, string> _log = null;
		public static LogManager Instance { get; private set; }

		public LogManager(Action<BepInEx.Logging.LogLevel, string> logAction)
		{
			_log = logAction;
		}

		public static void Init(Action<BepInEx.Logging.LogLevel, string> logAction)
		{
			Instance = new LogManager(logAction);
		}
		/// <summary>
		/// Produce a log entry with the given level and message
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Log(BepInEx.Logging.LogLevel level, string message)
		{
			if (_log != null)
			{
				_log(level, message);
			}
		}
		/// <summary>
		/// Produces a log entry with Info level severity and message as text
		/// </summary>
		/// <param name="message"></param>
		public void Log(string message)
		{
			Log(BepInEx.Logging.LogLevel.Info, message);
		}
	}
}
