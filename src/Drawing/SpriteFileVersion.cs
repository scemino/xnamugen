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

		public SpriteFileVersion(Byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException("bytes");
			if (bytes.Length != 4) throw new ArgumentException("Array length must be 4.", "bytes");

			m_high = bytes[0];
			m_low1 = bytes[1];
			m_low2 = bytes[2];
			m_low3 = bytes[4];
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