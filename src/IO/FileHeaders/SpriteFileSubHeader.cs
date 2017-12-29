using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	internal class SpriteFileSubHeader
	{
		public SpriteFileSubHeader(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var data = file.ReadBytes(19);
			if (data.Length != 19) throw new ArgumentException("File is not long enough", nameof(file));

			m_nextoffset = BitConverter.ToInt32(data, 0);
			m_imagesize = BitConverter.ToInt32(data, 4);
			m_axis = new Point(BitConverter.ToInt16(data, 8), BitConverter.ToInt16(data, 10));
			m_id = new SpriteId(BitConverter.ToInt16(data, 12), BitConverter.ToInt16(data, 14));
			m_sharedindex = BitConverter.ToInt16(data, 16);
			m_copylastpalette = data[18] > 0;
		}

		public int NextOffset => m_nextoffset;

		public int ImageSize => m_imagesize;

		public Point Axis => m_axis;

		public SpriteId Id => m_id;

		public int SharedIndex => m_sharedindex;

		public bool CopyLastPalette => m_copylastpalette;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_nextoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_imagesize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteId m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_sharedindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_copylastpalette;

		#endregion
	}
}