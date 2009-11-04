using System;
using System.Diagnostics;

namespace xnaMugen.Commands
{
	class BufferCount
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

		public void Set(Int32 time)
		{
			m_value = Math.Max(m_value, time);
			m_isactive = m_value > 0;
		}

		public void Tick()
		{
			m_value = Math.Max(0, m_value - 1);
			m_isactive = m_value > 0;
		}

		public override String ToString()
		{
			return m_value.ToString();
		}

		public Boolean IsActive
		{
			get { return m_isactive; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_value;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		#endregion
	}
}