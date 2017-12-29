using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace xnaMugen
{
	internal enum LogLevel { Normal, Warning, Error }

	internal enum LogSystem { Main, FileSystem, AnimationSystem, EvaluationSystem, StringConverter, StateSystem, BackgroundCollection, SpriteSystem, CommandSystem, SoundSystem, InputSystem }

	internal static class Log
	{
		static Log()
		{
			s_stringbuilder = new StringBuilder(50);
		}

		public static void Start()
		{
			if (Debugger.IsAttached == false)
			{
				var logfilename = string.Format("{0:u}.txt", DateTime.Now).Replace(':', '-');
				s_logfile = new StreamWriter(logfilename);
				s_logfile.AutoFlush = true;
			}
				
			Debug.AutoFlush = true;

			Write(LogLevel.Normal, LogSystem.Main, "Starting xnaMugen");
		}

		public static void KillLog()
		{
			if (s_logfile == null) return;

			var filepath = (s_logfile.BaseStream as FileStream).Name;

			s_logfile.Dispose();
			s_logfile = null;

			File.Delete(filepath);
		}

		public static void WriteException(Exception exception)
		{
			if (exception == null)
			{
				WriteLine(string.Empty);
				WriteLine("Unhandled exception: NULL");
				WriteLine(string.Empty);
				WriteLine("Stace Trace:");
				WriteLine(Environment.StackTrace); 
				
				return;
			}

			WriteLine(string.Empty);
			WriteLine(exception.ToString());
		}

		public static void Write(LogLevel level, LogSystem system, string message)
		{
			if (message == null) return;

			s_stringbuilder.Length = 0;
			s_stringbuilder.AppendFormat("{0, -25:u}{1, -10}{2, -23}{3}", DateTime.Now, level, system, message);

			var line = s_stringbuilder.ToString();
			WriteLine(line);
		}

		public static void Write(LogLevel level, LogSystem system, string format, params object[] args)
		{
			if (format == null || args == null) return;

			s_stringbuilder.Length = 0;
			s_stringbuilder.AppendFormat("{0, -25:u}{1, -10}{2, -23}", DateTime.Now, level, system);
			s_stringbuilder.AppendFormat(format, args);

			var line = s_stringbuilder.ToString();
			WriteLine(line);
		}

		private static void WriteLine(string text)
		{
			if (text == null) return;

			if (Debugger.IsAttached)
			{
				Debug.WriteLine(text);
			}
			else if(s_logfile != null)
			{
				s_logfile.WriteLine(text);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static StreamWriter s_logfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly StringBuilder s_stringbuilder;
	}
}