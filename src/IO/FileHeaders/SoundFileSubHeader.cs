using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	internal class SoundFileSubHeader
	{
		public SoundFileSubHeader(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var data = file.ReadBytes(16);
			if (data.Length != 16) throw new ArgumentException("File is not long enough", nameof(file));

			m_nextoffset = BitConverter.ToInt32(data, 0);
			m_length = BitConverter.ToInt32(data, 4);
			m_id = new SoundId(BitConverter.ToInt32(data, 8), BitConverter.ToInt32(data, 12));
		}

		public int NextOffset => m_nextoffset;

		public int SoundLength => m_length;

		public SoundId Id => m_id;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_nextoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_length;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_id;

		#endregion
	}
}