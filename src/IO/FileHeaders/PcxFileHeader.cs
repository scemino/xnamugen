using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	internal struct PcxFileHeader
	{
		public PcxFileHeader(File file)
		{
			if(file == null) throw new ArgumentNullException(nameof(file));

			var data = file.ReadBytes(HeaderSize);
			if (data.Length != HeaderSize) throw new ArgumentException("File is not long enough", nameof(file));

			m_manufacturer = data[0];
			m_version = data[1];
			m_encoding = data[2];
			m_bitsperpixel = data[3];
			m_xmin = BitConverter.ToInt16(data, 4);
			m_ymin = BitConverter.ToInt16(data, 6);
			m_xmax = BitConverter.ToInt16(data, 8);
			m_ymax = BitConverter.ToInt16(data, 10);
			m_horizontalDPI = BitConverter.ToInt16(data, 12);
			m_verticalDPI = BitConverter.ToInt16(data, 14);
			m_colorplanes = data[65];
			m_bytesperline = BitConverter.ToInt16(data, 66);
			m_palettetype = BitConverter.ToInt16(data, 68);
		}

		public PcxFileHeader(byte[] filebuffer)
		{
			if (filebuffer == null) throw new ArgumentNullException(nameof(filebuffer));

			m_manufacturer = filebuffer[0];
			m_version = filebuffer[1];
			m_encoding = filebuffer[2];
			m_bitsperpixel = filebuffer[3];
			m_xmin = BitConverter.ToInt16(filebuffer, 4);
			m_ymin = BitConverter.ToInt16(filebuffer, 6);
			m_xmax = BitConverter.ToInt16(filebuffer, 8);
			m_ymax = BitConverter.ToInt16(filebuffer, 10);
			m_horizontalDPI = BitConverter.ToInt16(filebuffer, 12);
			m_verticalDPI = BitConverter.ToInt16(filebuffer, 14);
			m_colorplanes = filebuffer[65];
			m_bytesperline = BitConverter.ToInt16(filebuffer, 66);
			m_palettetype = BitConverter.ToInt16(filebuffer, 68);
		}

		public static int HeaderSize => 128;

		public byte Manufacturer => m_manufacturer;

		public byte Version => m_version;

		public byte Encoding => m_encoding;

		public byte BitsPerPixel => m_bitsperpixel;

		public short XMin => m_xmin;

		public short YMin => m_ymin;

		public short XMax => m_xmax;

		public short YMax => m_ymax;

		public short HorizontalDPI => m_horizontalDPI;

		public short VerticalDPI => m_verticalDPI;

		public byte ColorPlanes => m_colorplanes;

		public short BytesPerLine => m_bytesperline;

		public short PaletteType => m_palettetype;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_manufacturer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_encoding;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_bitsperpixel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_xmin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_ymin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_xmax;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_ymax;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_horizontalDPI;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_verticalDPI;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_colorplanes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_bytesperline;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly short m_palettetype;

		#endregion
	}
}