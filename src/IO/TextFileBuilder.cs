using System;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.IO
{
	/// <summary>
	/// Builds xnaMugen.IO.TextFiles out of given xnaMugen.IO.Files.
	/// </summary>
	class TextFileBuilder
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filesystem">Reference to creating FileSystem.</param>
		public TextFileBuilder(FileSystem filesystem)
		{
			if (filesystem == null) throw new ArgumentNullException("filesystem");

			m_system = filesystem;
			m_builder = new StringBuilder();
			m_linecache = new List<StringBuilderSubString>(100);
			m_lock = new Object();
		}

		/// <summary>
		/// Parses a xnaMugen.IO.File as a text file.
		/// </summary>
		/// <param name="file">The file to be parsed. The read position must be set to the beginning of the text location.</param>
		/// <returns>The xnaMugen.IO.TextFile parsed out of the given file.</returns>
		public TextFile Build(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			lock (m_lock)
			{
				file.ReadIntoStringBuilder(m_builder);

				ReadLines();

				Int32 section_count = GetNumberOfSections();

				List<TextSection> sections = new List<TextSection>(section_count);

				for (Int32 titleindex = FindTitleIndex(0); titleindex != -1; titleindex = FindTitleIndex(titleindex + 1))
				{
					String title = GetTitle(titleindex);
					Int32 linecount = GetLineCountForSection(titleindex);

					List<String> lines = new List<String>(linecount);
					List<KeyValuePair<String, String>> parsedlines = new List<KeyValuePair<String, String>>(linecount);

					for (Int32 i = 0; i != linecount; ++i)
					{
						StringBuilderSubString line_str = m_linecache[titleindex + i + 1];
						String line = line_str.ToString();

						lines.Add(line);

						if (m_system.GetSubSystem<Animations.AnimationSystem>().IsClsnLine(line) == false)
						{
							StringBuilderSubString key, value;
							if (line_str.Parse(out key, out value) == true) parsedlines.Add(new KeyValuePair<String, String>(key.ToString(), value.ToString()));
						}
					}

					TextSection section = new TextSection(m_system, title, lines, parsedlines);
					sections.Add(section);
				}

				return new TextFile(file.Filepath, sections);
			}
		}

		/// <summary>
		/// Returns the number of sections in the text cache.
		/// </summary>
		/// <returns>The number of sections in the text cache.</returns>
		Int32 GetNumberOfSections()
		{
			Int32 count = 0;
			for (Int32 titleindex = FindTitleIndex(0); titleindex != -1; titleindex = FindTitleIndex(titleindex + 1))
			{
				++count;
			}

			return count;
		}

		/// <summary>
		/// Returns the number of lines in a section identified by the index of its title.
		/// </summary>
		/// <param name="titleindex">The index of the title of a given section in the text cache.</param>
		/// <returns>The number of lines in the identified section.</returns>
		Int32 GetLineCountForSection(Int32 titleindex)
		{
			if (titleindex < 0 || titleindex >= m_linecache.Count) throw new ArgumentOutOfRangeException("startindex");

			if (IsTitleLine(titleindex) == false) throw new ArgumentException("titleindex");

			Int32 linecount = 0;

			for (Int32 i = titleindex + 1; i < m_linecache.Count; ++i)
			{
				if (IsTitleLine(i) == true) break;
				++linecount;
			}

			return linecount;
		}

		/// <summary>
		/// Determines whether a line in the text cache contains a section title.
		/// </summary>
		/// <param name="index">The index of the line in the text cache to check.</param>
		/// <returns>true is the indentified line is a section title; false otherwise.</returns>
		Boolean IsTitleLine(Int32 index)
		{
			if (index < 0 || index >= m_linecache.Count) throw new ArgumentOutOfRangeException("index");

			StringBuilderSubString substring = m_linecache[index];
			return substring.Length > 2 && substring[0] == '[' && substring[substring.Length - 1] == ']';
		}

		/// <summary>
		/// Returns the section title found in the given line.
		/// </summary>
		/// <param name="index">The index of the line in the text cache containing a section title.</param>
		/// <returns>The section title.</returns>
		String GetTitle(Int32 index)
		{
			if (index < 0 || index >= m_linecache.Count) throw new ArgumentOutOfRangeException("index");

			if (IsTitleLine(index) == false) throw new ArgumentException("index");

			StringBuilderSubString substring = m_linecache[index];
			++substring.StartIndex;
			--substring.EndIndex;

			return substring.ToString();
		}

		/// <summary>
		/// Searchs the text cache for a line that represents a section title.
		/// </summary>
		/// <param name="startindex">The line index to start the search.</param>
		/// <returns>The next index in the text cache that represents a section title. -1 if no line can be found.</returns>
		Int32 FindTitleIndex(Int32 startindex)
		{
			if (startindex < 0 || startindex > m_linecache.Count) throw new ArgumentOutOfRangeException("startindex");

			for (Int32 index = startindex; index < m_linecache.Count; ++index)
			{
				if (IsTitleLine(index) == true) return index;
			}

			return -1;
		}

		/// <summary>
		/// Builds the internal text cache.
		/// </summary>
		void ReadLines()
		{
			m_linecache.Clear();

			Int32 totalindex = 0;
			while (totalindex < m_builder.Length)
			{
				StringBuilderSubString substring = StringBuilderSubString.BuildLine(m_builder, totalindex);
				if (substring.StartIndex >= m_builder.Length) break;
				totalindex = substring.EndIndex + 1;

				substring.Clean();
				if (substring.Length == 0) continue;

				m_linecache.Add(substring);
			}
		}

		#region Fields

		FileSystem m_system;

		Object m_lock;

		StringBuilder m_builder;

		List<StringBuilderSubString> m_linecache;

		#endregion;
	}
}