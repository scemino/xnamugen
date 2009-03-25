using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace xnaMugen
{
	class StringFormatter: SubSystem
	{
		public StringFormatter(SubSystems systems)
			: base(systems)
		{
			m_lock = new Object();
			m_builder = new StringBuilder(100);
			m_args = new List<Object>();
		}

		public String BuildString(String format, params Object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			lock (m_lock)
			{
				m_args.Clear();
				m_args.AddRange(args);

				return Build(format);
			}
		}

		public String BuildString(String format, List<Object> args)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			lock (m_lock)
			{
				m_args.Clear();
				m_args.AddRange(args);

				return Build(format);
			}
		}

		public String BuildString(String format, Evaluation.Result args)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			lock(m_lock)
			{
				m_args.Clear();

				for (Int32 i = 0; i != args.Count; ++i)
				{
					if (args[i].NumberType == NumberType.Int) m_args.Add(args[i].IntValue);
					else if (args[i].NumberType == NumberType.Float) m_args.Add(args[i].FloatValue);
					else m_args.Add(null);
				}

				return Build(format);
			}
		}

		String Build(String format)
		{
			if (format == null) throw new ArgumentNullException("format");

			m_builder.Length = 0;
			Int32 currentparam = 0;

			for (Int32 i = 0; i < format.Length; ++i)
			{
				Char current = format[i];
				Char next = (i + 1 < format.Length) ? format[i + 1] : (Char)0;

				if (current == '%')
				{
					if (next == '%')
					{
						m_builder.Append('%');
					}
					else if (next == 'i' || next == 'I' || next == 'd' || next == 'D')
					{
						if (currentparam < m_args.Count)
						{
							Object arg = m_args[currentparam];
							if (arg is Int32 || arg is Single) m_builder.Append(arg.ToString());

							++currentparam;
							++i;
						}
						else
						{
							return String.Empty;
						}
					}
					else if (next == 'f' || next == 'F')
					{
						if (currentparam < m_args.Count)
						{
							Object arg = m_args[currentparam];
							if (arg is Int32 || arg is Single) m_builder.Append(arg.ToString());

							++currentparam;
							++i;
						}
						else
						{
							return String.Empty;
						}
					}
					else if (next == 's' || next == 'S')
					{
						if (currentparam < m_args.Count)
						{
							Object arg = m_args[currentparam];
							if (arg is String) m_builder.Append(arg.ToString());

							++currentparam;
							++i;
						}
						else
						{
							return String.Empty;
						}
					}
					else
					{
						return String.Empty;
					}
				}
				else if (current == '\\')
				{
					if (next == 'n' || next == 'N')
					{
						m_builder.Append('\n');
						++i;
					}
					else if (next == 't' || next == 't')
					{
						m_builder.Append("    ");
						++i;
					}
					else
					{
						return String.Empty;
					}
				}
				else
				{
					m_builder.Append(current);
				}
			}

			return m_builder.ToString();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Object m_lock;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StringBuilder m_builder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Object> m_args;

		#endregion
	}
}