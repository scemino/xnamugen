using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace xnaMugen
{
	enum LogLevel { Normal, Warning, Error }

	enum LogSystem { Main, FileSystem, AnimationSystem, EvaluationSystem, StringConverter, StateSystem, BackgroundCollection, SpriteSystem, CommandSystem, SoundSystem, InputSystem }

	static class Log
	{
		static Log()
		{
			s_stringbuilder = new StringBuilder(50);
		}

		public static void Start()
		{
			if (Debugger.IsAttached == false)
			{
				String logfilename = String.Format("{0:u}.txt", DateTime.Now).Replace(':', '-');
				s_logfile = new StreamWriter(logfilename);
				s_logfile.AutoFlush = true;
			}
				
			Debug.AutoFlush = true;

			Write(LogLevel.Normal, LogSystem.Main, "Starting xnaMugen");
		}

		public static void KillLog()
		{
			if (s_logfile == null) return;

			String filepath = (s_logfile.BaseStream as FileStream).Name;

			s_logfile.Dispose();
			s_logfile = null;

			File.Delete(filepath);
		}

		public static void WriteException(Exception exception)
		{
			if (exception == null)
			{
				WriteLine(String.Empty);
				WriteLine("Unhandled exception: NULL");
				WriteLine(String.Empty);
				WriteLine("Stace Trace:");
				WriteLine(Environment.StackTrace); 
				
				return;
			}

			WriteLine(String.Empty);
			WriteLine(exception.ToString());
		}

		public static void Write(LogLevel level, LogSystem system, String message)
		{
			if (message == null) return;

			s_stringbuilder.Length = 0;
			s_stringbuilder.AppendFormat("{0, -25:u}{1, -10}{2, -23}{3}", DateTime.Now, level, system, message);

			String line = s_stringbuilder.ToString();
			WriteLine(line);
		}

		public static void Write(LogLevel level, LogSystem system, String format, params Object[] args)
		{
			if (format == null || args == null) return;

			s_stringbuilder.Length = 0;
			s_stringbuilder.AppendFormat("{0, -25:u}{1, -10}{2, -23}", DateTime.Now, level, system);
			s_stringbuilder.AppendFormat(format, args);

			String line = s_stringbuilder.ToString();
			WriteLine(line);
		}

		static void WriteLine(String text)
		{
			if (text == null) return;

			if (Debugger.IsAttached == true)
			{
				Debug.WriteLine(text);
			}
			else if(s_logfile != null)
			{
				s_logfile.WriteLine(text);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static StreamWriter s_logfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static StringBuilder s_stringbuilder;
	}
}