using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.IO
{
	/// <summary>
	/// Represents the text data found in a file.
	/// </summary>
	class TextFile
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filepath">The path to the file this class represents.</param>
		/// <param name="sections">The collection of TextSection parsed from the represented file.</param>
		[DebuggerStepThrough]
		public TextFile(String filepath, List<TextSection> sections)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");
			if (sections == null) throw new ArgumentNullException("sections");

			m_filepath = filepath;
			m_sections = sections;
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
		/// Returns a TextSection with a given title.
		/// </summary>
		/// <param name="title">The title of the requested TextSection.</param>
		/// <returns>The TextSection with the given title if found; null otherwise.</returns>
		[DebuggerStepThrough]
		public TextSection GetSection(String title)
		{
			if (title == null) throw new ArgumentNullException("title");

			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			foreach (TextSection s in this) if (sc.Equals(s.Title, title) == true) return s;

			return null;
		}

		/// <summary>
		/// Returns an enumerator for the TextSections of this object.
		/// </summary>
		/// <returns>An enumerator for the TextSections of this object.</returns>
		[DebuggerStepThrough]
		public List<TextSection>.Enumerator GetEnumerator()
		{
			return m_sections.GetEnumerator();
		}

		/// <summary>
		/// Returns the path of the file this object represents.
		/// </summary>
		/// <returns>The path of the file this object represents.</returns>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public String Filepath
		{
			get { return m_filepath; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<TextSection> m_sections;

		#endregion
	}
}
