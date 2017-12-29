using System;
using System.Diagnostics;

namespace xnaMugen.Drawing
{
	internal struct SpriteFileVersion
	{
		public SpriteFileVersion(byte high, byte low1, byte low2, byte low3)
		{
			m_high = high;
			m_low1 = low1;
			m_low2 = low2;
			m_low3 = low3;
		}

		public SpriteFileVersion(byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));
			if (bytes.Length != 4) throw new ArgumentException("Array length must be 4.", nameof(bytes));

			m_high = bytes[0];
			m_low1 = bytes[1];
			m_low2 = bytes[2];
			m_low3 = bytes[4];
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3}", m_high, m_low1, m_low2, m_low3);
		}

		public byte High => m_high;

		public byte Low1 => m_low1;

		public byte Low2 => m_low2;

		public byte Low3 => m_low3;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_high;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_low1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_low2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_low3;

		#endregion
	}
}