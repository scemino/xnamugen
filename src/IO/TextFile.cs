using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.IO
{
	/// <summary>
	/// Represents the text data found in a file.
	/// </summary>
	internal class TextFile
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filepath">The path to the file this class represents.</param>
		/// <param name="sections">The collection of TextSection parsed from the represented file.</param>
		[DebuggerStepThrough]
		public TextFile(string filepath, List<TextSection> sections)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));
			if (sections == null) throw new ArgumentNullException(nameof(sections));

			m_filepath = filepath;
			m_sections = sections;
		}

		/// <summary>
		/// Returns the path of the file this object represents.
		/// </summary>
		/// <returns>The path of the file this object represents.</returns>
		[DebuggerStepThrough]
		public override string ToString()
		{
			return Filepath;
		}

		/// <summary>
		/// Returns a TextSection with a given title.
		/// </summary>
		/// <param name="title">The title of the requested TextSection.</param>
		/// <returns>The TextSection with the given title if found; null otherwise.</returns>
		[DebuggerStepThrough]
		public TextSection GetSection(string title)
		{
			if (title == null) throw new ArgumentNullException(nameof(title));

			var sc = StringComparer.OrdinalIgnoreCase;

			foreach (var s in this) if (sc.Equals(s.Title, title)) return s;

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
		public string Filepath => m_filepath;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		private readonly List<TextSection> m_sections;

		#endregion
	}
}
