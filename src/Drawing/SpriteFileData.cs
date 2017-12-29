using System.Diagnostics;

namespace xnaMugen.Drawing
{
	[DebuggerDisplay("{" + nameof(Id) + "}")]
	internal class SpriteFileData
	{
		public SpriteFileData(int fileoffset, int pcxsize, Point axis, SpriteId id, int sharedindex, bool copylastpalette)
		{
			m_fileoffset = fileoffset;
			m_pcxsize = pcxsize;
			m_axis = axis;
			m_id = id;
			m_sharedindex = sharedindex;
			m_copylastpalette = copylastpalette;
			m_isvalid = null;
		}

		public int FileOffset => m_fileoffset;

		public int PcxSize => m_pcxsize;

		public Point Axis => m_axis;

		public SpriteId Id => m_id;

		public int SharedIndex => m_sharedindex;

		public bool CopyLastPalette => m_copylastpalette;

		public bool? IsValid
		{
			get => m_isvalid;

			set { m_isvalid = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_fileoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_pcxsize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteId m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_sharedindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_copylastpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool? m_isvalid;

		#endregion
	}
}