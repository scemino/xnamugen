using System;
using System.Diagnostics;

namespace xnaMugen.Commands
{
	internal class BufferCount
	{
		public BufferCount()
		{
			m_value = 0;
			m_isactive = false;
		}

		public void Reset()
		{
			m_value = 0;
			m_isactive = false;
		}

		public void Set(int time)
		{
			m_value = Math.Max(m_value, time);
			m_isactive = m_value > 0;
		}

		public void Tick()
		{
			m_value = Math.Max(0, m_value - 1);
			m_isactive = m_value > 0;
		}

		public override string ToString()
		{
			return m_value.ToString();
		}

		public bool IsActive => m_isactive;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_value;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		#endregion
	}
}