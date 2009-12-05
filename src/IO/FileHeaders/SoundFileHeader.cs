using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	class SoundFileHeader
	{
		public SoundFileHeader(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			Byte[] data = file.ReadBytes(24);
			if (data.Length != 24) throw new ArgumentException("File is not long enough", "file");

			m_signature = System.Text.Encoding.Default.GetString(data, 0, 11);
			m_version = BitConverter.ToInt32(data, 12);
			m_numberofsounds = BitConverter.ToInt32(data, 16);
			m_subheaderoffset = BitConverter.ToInt32(data, 20);
		}

		public String Signature
		{
			get { return m_signature; }
		}

		public Int32 Version
		{
			get { return m_version; }
		}

		public Int32 NumberOfSounds
		{
			get { return m_numberofsounds; }
		}

		public Int32 SubheaderOffset
		{
			get { return m_subheaderoffset; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_signature;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_numberofsounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_subheaderoffset;

		#endregion
	}
}