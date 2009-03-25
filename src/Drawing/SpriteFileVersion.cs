using System;
using System.Diagnostics;

namespace xnaMugen.Drawing
{
	struct SpriteFileVersion
	{
		public SpriteFileVersion(Byte high, Byte low1, Byte low2, Byte low3)
		{
			m_high = high;
			m_low1 = low1;
			m_low2 = low2;
			m_low3 = low3;
		}

		public override String ToString()
		{
			return String.Format("{0} {1} {2} {3}", m_high, m_low1, m_low2, m_low3);
		}

		public Byte High
		{
			get { return m_high; }
		}

		public Byte Low1
		{
			get { return m_low1; }
		}

		public Byte Low2
		{
			get { return m_low2; }
		}

		public Byte Low3
		{
			get { return m_low3; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_high;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_low1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_low2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_low3;

		#endregion
	}
}