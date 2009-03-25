using System;
using System.Diagnostics;

namespace xnaMugen.Commands
{
	class BufferCount
	{
		public BufferCount()
		{
		}

		public void Reset()
		{
			m_value = 0;
		}

		public void Set(Int32 time)
		{
			m_value = Math.Max(m_value, time);
		}

		public void Tick()
		{
			m_value = Math.Max(0, m_value - 1);
		}

		public override String ToString()
		{
			return m_value.ToString();
		}

		public Boolean IsActive
		{
			get { return m_value > 0; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_value;

		#endregion
	}
}