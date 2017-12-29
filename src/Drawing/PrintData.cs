using System.Diagnostics;

namespace xnaMugen.Drawing
{
	[DebuggerDisplay("{Index} : {ColorIndex} : {Justification}")]
	internal struct PrintData
	{
		public PrintData(int index, int colorindex, PrintJustification justification)
		{
			m_index = index;
			m_colorindex = colorindex;
			m_justification = justification;
			m_isvalid = true;
		}

		public int Index => m_index;

		public int ColorIndex => m_colorindex;

		public PrintJustification Justification => m_justification;

		public bool IsValid => m_isvalid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_isvalid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_index;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_colorindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintJustification m_justification;

		#endregion
	}
}