using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	struct PCX
	{
		public PCX(File file)
		{
			if(file == null) throw new ArgumentNullException("file");

			m_manufacturer = file.ReadByte();
			m_version = file.ReadByte();
			m_encoding = file.ReadByte();
			m_bitsperpixel = file.ReadByte();
			m_xmin = file.ReadInt16();
			m_ymin = file.ReadInt16();
			m_xmax = file.ReadInt16();
			m_ymax = file.ReadInt16();
			m_horizontalDPI = file.ReadInt16();
			m_verticalDPI = file.ReadInt16();

			file.SeekFromCurrent(48); // Colormap
			file.SeekFromCurrent(1); // Reserved
			
			m_colorplanes = file.ReadByte();
			m_bytesperline = file.ReadInt16();
			m_palettetype = file.ReadInt16();

			file.SeekFromCurrent(2); // Horizontal Screensize
			file.SeekFromCurrent(2); // Vertical Screensize

			file.SeekFromCurrent(54); // Filler
		}

		public PCX(Byte[] filebuffer)
		{
			if (filebuffer == null) throw new ArgumentNullException("filebuffer");

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

			//file.SeekFromCurrent(48); // Colormap
			//file.SeekFromCurrent(1); // Reserved

			m_colorplanes = filebuffer[65];
			m_bytesperline = BitConverter.ToInt16(filebuffer, 66);
			m_palettetype = BitConverter.ToInt16(filebuffer, 68);

			//file.SeekFromCurrent(2); // Horizontal Screensize
			//file.SeekFromCurrent(2); // Vertical Screensize

			//file.SeekFromCurrent(54); // Filler
		}

		static public Int32 HeaderSize
		{
			get { return 128; }
		}

		public Byte Manufacturer
		{
			get { return m_manufacturer; }
		}

		public Byte Version
		{
			get { return m_version; }
		}

		public Byte Encoding
		{
			get { return m_encoding; }
		}

		public Byte BitsPerPixel
		{
			get { return m_bitsperpixel; }
		}

		public Int16 XMin
		{
			get { return m_xmin; }
		}

		public Int16 YMin
		{
			get { return m_ymin; }
		}

		public Int16 XMax
		{
			get { return m_xmax; }
		}

		public Int16 YMax
		{
			get { return m_ymax; }
		}

		public Int16 HorizontalDPI
		{
			get { return m_horizontalDPI; }
		}

		public Int16 VerticalDPI
		{
			get { return m_verticalDPI; }
		}

		public Byte ColorPlanes
		{
			get { return m_colorplanes; }
		}

		public Int16 BytesPerLine
		{
			get { return m_bytesperline; }
		}

		public Int16 PaletteType
		{
			get { return m_palettetype; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_manufacturer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_encoding;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_bitsperpixel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_xmin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_ymin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_xmax;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_ymax;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_horizontalDPI;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_verticalDPI;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_colorplanes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_bytesperline;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int16 m_palettetype;

		#endregion
	}
}