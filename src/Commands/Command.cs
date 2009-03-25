using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("{Name}")]
	class Command
	{
		public Command(String name, String commandtext, Int32 time, Int32 buffertime, ReadOnlyList<CommandElement> elements)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (commandtext == null) throw new ArgumentNullException("commandtext");
			if (time < 0) throw new ArgumentOutOfRangeException("Time must be greater than or equal to zero");
			if (buffertime <= 0) throw new ArgumentOutOfRangeException("Buffertime must be greater than zero");
			if (elements == null) throw new ArgumentNullException("elements");

			m_name = name;
			m_commandtext = commandtext;
			m_time = time;
			m_buffertime = buffertime;
			m_elements = elements;
            m_isvalid = ValidCheck();
		}

		Boolean ValidCheck()
		{
			foreach (CommandElement element in Elements)
			{
				if (element.Buttons == CommandButton.None && element.Direction == CommandDirection.None) return false;
			}

			return true;
		}

		public String Name
		{
			get { return m_name; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		public Int32 BufferTime
		{
			get { return m_buffertime; }
		}

        public Boolean IsValid
        {
            get { return m_isvalid; }
        }

		public String Text
		{
			get { return m_commandtext; }
		}

		public ReadOnlyList<CommandElement> Elements
		{
			get { return m_elements; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_buffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_commandtext;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<CommandElement> m_elements;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Boolean m_isvalid;

		#endregion
	}
}