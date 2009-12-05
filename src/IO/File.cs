using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace xnaMugen.IO
{
	/// <summary>
	/// Contains methods for read only access of a file.
	/// </summary>
	class File : Resource
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filepath">The path to the represented file.</param>
		/// <param name="stream">The stream to the file.</param>
		[DebuggerStepThrough]
		public File(String filepath, Stream stream)
		{
			m_filepath = filepath;
			m_stream = stream;
			m_breader = new BinaryReader(m_stream);
			m_sreader = new StreamReader(m_stream);
			m_filelength = m_stream.Length;
		}

		/// <summary>
		/// Changes the read position of the file.
		/// </summary>
		/// <param name="offset">The new read position, offset from the beginning of the file.</param>
		[DebuggerStepThrough]
		public void SeekFromBeginning(Int64 offset)
		{
			m_stream.Seek(offset, SeekOrigin.Begin);
		}

		/// <summary>
		/// Changes the read position of the file.
		/// </summary>
		/// <param name="offset">The new read position, offset from the end of the file.</param>
		[DebuggerStepThrough]
		public void SeekFromEnd(Int64 offset)
		{
			m_stream.Seek(offset, SeekOrigin.End);
		}

		/// <summary>
		/// Changes the read position of the file.
		/// </summary>
		/// <param name="offset">The new read position, offset from the current read position.</param>
		[DebuggerStepThrough]
		public void SeekFromCurrent(Int64 offset)
		{
			m_stream.Seek(offset, SeekOrigin.Current);
		}

		public Byte[] ReadBytes(Int32 count)
		{
			return m_breader.ReadBytes(count);
		}

		/// <summary>
		/// Reads the entire file, starting from the current read position into a StringBuilder.
		/// </summary>
		/// <param name="builder">The StringBuilder to read the file into.</param>
		[DebuggerStepThrough]
		public void ReadIntoStringBuilder(StringBuilder builder)
		{
			if (builder == null) throw new ArgumentNullException("builder");

			Int32 filelength = (Int32)(FileLength - ReadPosition);
			if (builder.Capacity < filelength) builder.Capacity = filelength;
			builder.Length = 0;

			var charLen = m_sreader.GetType().GetField("charLen", BindingFlags.NonPublic | BindingFlags.Instance);
			var charPos = m_sreader.GetType().GetField("charPos", BindingFlags.NonPublic | BindingFlags.Instance);
			var charBuffer = (Char[])m_sreader.GetType().GetField("charBuffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_sreader);
			var readBuffer = m_sreader.GetType().GetMethod("ReadBuffer", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

			do
			{
				Int32 cpos = (Int32)charPos.GetValue(m_sreader);
				Int32 clen = (Int32)charLen.GetValue(m_sreader);
				builder.Append(charBuffer, cpos, clen - cpos);
				charPos.SetValue(m_sreader, clen);

				readBuffer.Invoke(m_sreader, null);
			}
			while ((Int32)charLen.GetValue(m_sreader) > 0);
		}

		public String ReadToEnd()
		{
			return m_sreader.ReadToEnd();
		}

		public String ReadLine()
		{
			return m_sreader.ReadLine();
		}

		/// <summary>
		/// Returns the path of the file this object represents.
		/// </summary>
		/// <returns>The path of the file this object represents.</returns>
		[DebuggerStepThrough]
		public override String ToString()
		{
			return Filepath;
		}

		/// <summary>
		/// Disposes of resources managed by this class instance.
		/// </summary>
		/// <param name="disposing">Determined whether to dispose of managed resources.</param>
		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_stream != null)
				{
					m_stream.Dispose();
				}

				if (m_breader != null)
				{
					m_breader.Close();
				}

				if (m_sreader != null)
				{
					m_sreader.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Returns the path of the file this object represents.
		/// </summary>
		/// <returns>The path of the file this object represents.</returns>
		public String Filepath
		{
			get { return m_filepath; }
		}

		/// <summary>
		/// Returns of the length of the file.
		/// </summary>
		/// <returns>The length of the file.</returns>
		public Int64 FileLength
		{
			get { return m_filelength; }
		}

		/// <summary>
		/// Returns the current read position.
		/// </summary>
		/// <returns>The current read position.</returns>
		public Int64 ReadPosition
		{
			get { return m_stream.Position; }
		}

		/// <summary>
		/// Returns the underlying System.IO.FileStream.
		/// </summary>
		/// <returns>The underlying System.IO.FileStream.</returns>
		public Stream Stream
		{
			get { return m_stream; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Stream m_stream;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int64 m_filelength;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly BinaryReader m_breader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StreamReader m_sreader;

		#endregion
	}
}
