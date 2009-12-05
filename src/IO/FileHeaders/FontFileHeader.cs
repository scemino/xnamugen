using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	class FontFileHeader
	{
		public FontFileHeader(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			Byte[] data = file.ReadBytes(24);
			if (data.Length != 24) throw new ArgumentException("File is not long enough", "file");

			m_signature = System.Text.Encoding.Default.GetString(data, 0, 11);
			m_unknown = BitConverter.ToInt32(data, 12);
			m_imageoffset = BitConverter.ToInt32(data, 16);
			m_imagesize = BitConverter.ToInt32(data, 20);
		}

		public String Signature
		{
			get { return m_signature; }
		}

		public Int32 Unknown
		{
			get { return m_unknown; }
		}

		public Int32 ImageOffset
		{
			get { return m_imageoffset; }
		}

		public Int32 ImageSize
		{
			get { return m_imagesize; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_signature;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_unknown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_imageoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_imagesize;

		#endregion
	}
}