using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("{" + nameof(Name) + "}")]
	internal class Command
	{
		public Command(string name, string commandtext, int time, int buffertime, ReadOnlyList<CommandElement> elements)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (commandtext == null) throw new ArgumentNullException(nameof(commandtext));
			if (time < 0) throw new ArgumentOutOfRangeException(nameof(time), "Time must be greater than or equal to zero");
			if (buffertime <= 0) throw new ArgumentOutOfRangeException(nameof(buffertime), "Buffertime must be greater than zero");
			if (elements == null) throw new ArgumentNullException(nameof(elements));

			m_name = name;
			m_commandtext = commandtext;
			m_time = time;
			m_buffertime = buffertime;
			m_elements = elements;
            m_isvalid = ValidCheck();
		}

		private bool ValidCheck()
		{
			foreach (var element in Elements)
			{
				if (element.Buttons == CommandButton.None && element.Direction == CommandDirection.None) return false;
			}

			return true;
		}

		public string Name => m_name;

		public int Time => m_time;

		public int BufferTime => m_buffertime;

		public bool IsValid => m_isvalid;

		public string Text => m_commandtext;

		public ReadOnlyList<CommandElement> Elements => m_elements;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_buffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_commandtext;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<CommandElement> m_elements;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool m_isvalid;

		#endregion
	}
}