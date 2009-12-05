using System;
using System.Diagnostics;

namespace xnaMugen.IO.FileHeaders
{
	class SpriteFileHeader
	{
		public SpriteFileHeader(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			Byte[] data = file.ReadBytes(33);
			if (data.Length != 33) throw new ArgumentException("File is not long enough", "file");

			m_signature = System.Text.Encoding.Default.GetString(data, 0, 11);
			m_version = new Drawing.SpriteFileVersion(data[12], data[13], data[14], data[15]);
			m_numberofgroups = BitConverter.ToInt32(data, 16);
			m_numberofimages = BitConverter.ToInt32(data, 20);
			m_subheaderoffset = BitConverter.ToInt32(data, 24);
			m_subheadersize = BitConverter.ToInt32(data, 28);
			m_sharedpalette = data[32] > 0;
		}

		public String Signature
		{
			get { return m_signature; }
		}

		public Drawing.SpriteFileVersion Version
		{
			get { return m_version; }
		}

		public Int32 NumberOfGroups
		{
			get { return m_numberofgroups; }
		}

		public Int32 NumberOfImages
		{
			get { return m_numberofimages; }
		}

		public Int32 SubheaderOffset
		{
			get { return m_subheaderoffset; }
		}

		public Int32 SubheaderSize
		{
			get { return m_subheadersize; }
		}

		public Boolean SharedPalette
		{
			get { return m_sharedpalette; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_signature;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteFileVersion m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_numberofgroups;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_numberofimages;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_subheaderoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_subheadersize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_sharedpalette;

		#endregion
	}
}