using System;
using System.Collections.Generic;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.IO
{
	/// <summary>
	/// Represents a section of xnaMugen.IO.TextFile.
	/// </summary>
	class TextSection
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filesystem">The filesystem.</param>
		/// <param name="title">The title of this section.</param>
		/// <param name="lines">The lines of this section.</param>
		/// <param name="parsedlines">The lines of this section parsed into key/value pairs.</param>
		public TextSection(FileSystem filesystem, String title, List<String> lines, List<KeyValuePair<String, String>> parsedlines)
		{
			if (filesystem == null) throw new ArgumentNullException("filesystem");
			if (title == null) throw new ArgumentNullException("title");
			if (lines == null) throw new ArgumentNullException("lines");
			if (parsedlines == null) throw new ArgumentNullException("parsedlines");

			m_filesystem = filesystem;
			m_title = title;
			m_lines = lines;
			m_parsedlines = parsedlines;
		}

		/// <summary>
		/// Determines whether this section contain a parsed lines with a given key.
		/// </summary>
		/// <param name="key">The key that is looked for.</param>
		/// <returns>true is the parsed line with the given key exists; false otherwise.</returns>
		public Boolean HasAttribute(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return GetValue(key) != null;
		}

		/// <summary>
		/// Return a value associated with a given key converted into a given Type.
		/// </summary>
		/// <typeparam name="T">The type of value is to be converted into.</typeparam>
		/// <param name="key">The key identified a requested value.</param>
		/// <returns>The value assoicated with the given key, converted into the given Type.</returns>
		public T GetAttribute<T>(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			String value = GetValue(key);
			if (value == "")
			{
				Log.Write(LogLevel.Error, LogSystem.FileSystem, "Section {0} is missing key '{1}'", Title, key);
				throw new ArgumentException("key");
			}

			return m_filesystem.GetSubSystem<StringConverter>().Convert<T>(value);
		}

		/// <summary>
		/// Return a value associated with a given key converted into a given Type.
		/// </summary>
		/// <typeparam name="T">The type of value is to be converted into.</typeparam>
		/// <param name="key">The key identified a requested value.</param>
		/// <param name="failover">A value of return of the key is not found.</param>
		/// <returns>The value assoicated with the given key, converted into the given Type. If the key is not found, then failover is returned.</returns>
		public T GetAttribute<T>(String key, T failover)
		{
			if (key == null) throw new ArgumentNullException("key");

			String value = GetValue(key);
			T returnvalue;

			if (value != null)
			{
				if (m_filesystem.GetSubSystem<StringConverter>().TryConvert<T>(value, out returnvalue) == true)
				{
					return returnvalue;
				}
				else
				{
					Log.Write(LogLevel.Warning, LogSystem.FileSystem, "Cannot convert '{1}' for key '{0}' to type: {2}", key, value, typeof(T).Name);
				}
			}

			return failover;
		}

		/// <summary>
		/// Returns the title of this section.
		/// </summary>
		/// <returns>The title of this section.</returns>
		public override String ToString()
		{
			return Title;
		}

		/// <summary>
		/// Returns the value associated with the given key.
		/// </summary>
		/// <param name="key">The key associated with the requested value.</param>
		/// <returns>The value for the given key is it exists; null otherwise.</returns>
		String GetValue(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			StringComparer sc = StringComparer.OrdinalIgnoreCase;
			foreach (KeyValuePair<String, String> data in ParsedLines)
			{
				if (sc.Equals(key, data.Key) == true)
				{
					return data.Value;
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the title of this section.
		/// </summary>
		/// <returns>The title of this section.</returns>
		public String Title
		{
			get { return m_title; }
		}

		/// <summary>
		/// Returns an iterator for read only access of the lines of this section.
		/// </summary>
		/// <returns>An iterator for read only access of the lines of this section.</returns>
		public ListIterator<String> Lines
		{
			get { return new ListIterator<String>(m_lines); }
		}

		/// <summary>
		/// Returns an iterator for read only access of the parsed lines of this section.
		/// </summary>
		/// <returns>An iterator for read only access of the parsed lines of this section.</returns>
		public ListIterator<KeyValuePair<String, String>> ParsedLines
		{
			get { return new ListIterator<KeyValuePair<String, String>>(m_parsedlines); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly FileSystem m_filesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_title;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<String> m_lines;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<KeyValuePair<String, String>> m_parsedlines;

		#endregion
	}
}
