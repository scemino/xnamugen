using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	internal class FontFileHeader
	{
		public FontFileHeader(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var data = file.ReadBytes(24);
			if (data.Length != 24) throw new ArgumentException("File is not long enough", nameof(file));

			m_signature = System.Text.Encoding.Default.GetString(data, 0, 11);
			m_unknown = BitConverter.ToInt32(data, 12);
			m_imageoffset = BitConverter.ToInt32(data, 16);
			m_imagesize = BitConverter.ToInt32(data, 20);
		}

		public string Signature => m_signature;

		public int Unknown => m_unknown;

		public int ImageOffset => m_imageoffset;

		public int ImageSize => m_imagesize;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_signature;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_unknown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_imageoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_imagesize;

		#endregion
	}
}