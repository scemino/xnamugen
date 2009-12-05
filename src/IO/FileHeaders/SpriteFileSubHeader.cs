using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	class SpriteFileSubHeader
	{
		public SpriteFileSubHeader(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			Byte[] data = file.ReadBytes(19);
			if (data.Length != 19) throw new ArgumentException("File is not long enough", "file");

			m_nextoffset = BitConverter.ToInt32(data, 0);
			m_imagesize = BitConverter.ToInt32(data, 4);
			m_axis = new Point(BitConverter.ToInt16(data, 8), BitConverter.ToInt16(data, 10));
			m_id = new SpriteId(BitConverter.ToInt16(data, 12), BitConverter.ToInt16(data, 14));
			m_sharedindex = BitConverter.ToInt16(data, 16);
			m_copylastpalette = data[18] > 0;
		}

		public Int32 NextOffset
		{
			get { return m_nextoffset; }
		}

		public Int32 ImageSize
		{
			get { return m_imagesize; }
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

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_nextoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_imagesize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteId m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_sharedindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_copylastpalette;

		#endregion
	}
}