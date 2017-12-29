using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	internal class SoundFileHeader
	{
		public SoundFileHeader(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var data = file.ReadBytes(24);
			if (data.Length != 24) throw new ArgumentException("File is not long enough", nameof(file));

			m_signature = System.Text.Encoding.Default.GetString(data, 0, 11);
			m_version = BitConverter.ToInt32(data, 12);
			m_numberofsounds = BitConverter.ToInt32(data, 16);
			m_subheaderoffset = BitConverter.ToInt32(data, 20);
		}

		public string Signature => m_signature;

		public int Version => m_version;

		public int NumberOfSounds => m_numberofsounds;

		public int SubheaderOffset => m_subheaderoffset;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_signature;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_numberofsounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_subheaderoffset;

		#endregion
	}
}