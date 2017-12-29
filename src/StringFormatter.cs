using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace xnaMugen
{
	internal class StringFormatter: SubSystem
	{
		public StringFormatter(SubSystems systems)
			: base(systems)
		{
			m_lock = new object();
			m_builder = new StringBuilder(100);
			m_args = new List<object>();
		}

		public string BuildString(string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException(nameof(format));
			if (args == null) throw new ArgumentNullException(nameof(args));

			lock (m_lock)
			{
				m_args.Clear();
				m_args.AddRange(args);

				return Build(format);
			}
		}

		public string BuildString(string format, List<object> args)
		{
			if (format == null) throw new ArgumentNullException(nameof(format));
			if (args == null) throw new ArgumentNullException(nameof(args));

			lock (m_lock)
			{
				m_args.Clear();
				m_args.AddRange(args);

				return Build(format);
			}
		}

		public string BuildString(string format, Evaluation.Number[] args)
		{
			if (format == null) throw new ArgumentNullException(nameof(format));
			if (args == null) throw new ArgumentNullException(nameof(args));

			lock(m_lock)
			{
				m_args.Clear();

				for (var i = 0; i != args.Length; ++i)
				{
					if (args[i].NumberType == NumberType.Int) m_args.Add(args[i].IntValue);
					else if (args[i].NumberType == NumberType.Float) m_args.Add(args[i].FloatValue);
					else m_args.Add(null);
				}

				return Build(format);
			}
		}

		private string Build(string format)
		{
			if (format == null) throw new ArgumentNullException(nameof(format));

			m_builder.Length = 0;
			var currentparam = 0;

			for (var i = 0; i < format.Length; ++i)
			{
				var current = format[i];
				var next = i + 1 < format.Length ? format[i + 1] : (char)0;

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
							var arg = m_args[currentparam];
							if (arg is int || arg is float) m_builder.Append(arg);

							++currentparam;
							++i;
						}
						else
						{
							return string.Empty;
						}
					}
					else if (next == 'f' || next == 'F')
					{
						if (currentparam < m_args.Count)
						{
							var arg = m_args[currentparam];
							if (arg is int || arg is float) m_builder.Append(arg);

							++currentparam;
							++i;
						}
						else
						{
							return string.Empty;
						}
					}
					else if (next == 's' || next == 'S')
					{
						if (currentparam < m_args.Count)
						{
							var arg = m_args[currentparam];
							if (arg is string) m_builder.Append(arg);

							++currentparam;
							++i;
						}
						else
						{
							return string.Empty;
						}
					}
					else
					{
						return string.Empty;
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
						return string.Empty;
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
		private readonly object m_lock;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StringBuilder m_builder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<object> m_args;

		#endregion
	}
}