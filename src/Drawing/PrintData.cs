using System;
using xnaMugen.IO;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace xnaMugen.Drawing
{
	[DebuggerDisplay("{Index} : {ColorIndex} : {Justification}")]
	struct PrintData
	{
		public PrintData(Int32 index, Int32 colorindex, PrintJustification justification)
		{
			m_index = index;
			m_colorindex = colorindex;
			m_justification = justification;
			m_isvalid = true;
		}

		public Int32 Index
		{
			get { return m_index; }
		}

		public Int32 ColorIndex
		{
			get { return m_colorindex; }
		}

		public PrintJustification Justification
		{
			get { return m_justification; }
		}

		public Boolean IsValid
		{
			get { return m_isvalid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_isvalid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_index;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_colorindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PrintJustification m_justification;

		#endregion
	}
}