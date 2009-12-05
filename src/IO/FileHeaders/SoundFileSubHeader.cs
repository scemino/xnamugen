using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	class SoundFileSubHeader
	{
		public SoundFileSubHeader(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			Byte[] data = file.ReadBytes(16);
			if (data.Length != 16) throw new ArgumentException("File is not long enough", "file");

			m_nextoffset = BitConverter.ToInt32(data, 0);
			m_length = BitConverter.ToInt32(data, 4);
			m_id = new SoundId(BitConverter.ToInt32(data, 8), BitConverter.ToInt32(data, 12));
		}

		public Int32 NextOffset
		{
			get { return m_nextoffset; }
		}

		public Int32 SoundLength
		{
			get { return m_length; }
		}

		public SoundId Id
		{
			get { return m_id; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_nextoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_length;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundId m_id;

		#endregion
	}
}