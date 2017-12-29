using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	internal class SpriteFileHeader
	{
		public SpriteFileHeader(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var data = file.ReadBytes(33);
			if (data.Length != 33) throw new ArgumentException("File is not long enough", nameof(file));

			m_signature = System.Text.Encoding.Default.GetString(data, 0, 11);
			m_version = new Drawing.SpriteFileVersion(data[12], data[13], data[14], data[15]);
			m_numberofgroups = BitConverter.ToInt32(data, 16);
			m_numberofimages = BitConverter.ToInt32(data, 20);
			m_subheaderoffset = BitConverter.ToInt32(data, 24);
			m_subheadersize = BitConverter.ToInt32(data, 28);
			m_sharedpalette = data[32] > 0;
		}

		public string Signature => m_signature;

		public Drawing.SpriteFileVersion Version => m_version;

		public int NumberOfGroups => m_numberofgroups;

		public int NumberOfImages => m_numberofimages;

		public int SubheaderOffset => m_subheaderoffset;

		public int SubheaderSize => m_subheadersize;

		public bool SharedPalette => m_sharedpalette;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_signature;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteFileVersion m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_numberofgroups;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_numberofimages;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_subheaderoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_subheadersize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_sharedpalette;

		#endregion
	}
}