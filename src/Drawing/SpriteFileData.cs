using System;
using System.Diagnostics;

namespace xnaMugen.Drawing
{
	[DebuggerDisplay("{Id}")]
	class SpriteFileData
	{
		public SpriteFileData(Int32 fileoffset, Int32 pcxsize, Point axis, SpriteId id, Int32 sharedindex, Boolean copylastpalette)
		{
			m_fileoffset = fileoffset;
			m_pcxsize = pcxsize;
			m_axis = axis;
			m_id = id;
			m_sharedindex = sharedindex;
			m_copylastpalette = copylastpalette;
			m_killbit = false;
		}

		public Int32 FileOffset
		{
			get { return m_fileoffset; }
		}

		public Int32 PcxSize
		{
			get { return m_pcxsize; }
		}

		public Point Axis
		{
			get { return m_axis; }
		}

		public SpriteId Id
		{
			get { return m_id; }
		}

		public Int32 SharedIndex
		{
			get { return m_sharedindex; }
		}

		public Boolean CopyLastPalette
		{
			get { return m_copylastpalette; }
		}

		public Boolean Killbit
		{
			get { return m_killbit; }

			set { m_killbit = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_fileoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_pcxsize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteId m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_sharedindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_copylastpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_killbit;

		#endregion
	}
}