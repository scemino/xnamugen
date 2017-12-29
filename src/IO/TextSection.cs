using System;
using System.Collections.Generic;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.IO
{
	/// <summary>
	/// Represents a section of xnaMugen.IO.TextFile.
	/// </summary>
	internal class TextSection
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="filesystem">The filesystem.</param>
		/// <param name="title">The title of this section.</param>
		/// <param name="lines">The lines of this section.</param>
		/// <param name="parsedlines">The lines of this section parsed into key/value pairs.</param>
		public TextSection(FileSystem filesystem, string title, List<string> lines, List<KeyValuePair<string, string>> parsedlines)
		{
			if (filesystem == null) throw new ArgumentNullException(nameof(filesystem));
			if (title == null) throw new ArgumentNullException(nameof(title));
			if (lines == null) throw new ArgumentNullException(nameof(lines));
			if (parsedlines == null) throw new ArgumentNullException(nameof(parsedlines));

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
		public bool HasAttribute(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return GetValue(key) != null;
		}

		/// <summary>
		/// Return a value associated with a given key converted into a given Type.
		/// </summary>
		/// <typeparam name="T">The type of value is to be converted into.</typeparam>
		/// <param name="key">The key identified a requested value.</param>
		/// <returns>The value assoicated with the given key, converted into the given Type.</returns>
		public T GetAttribute<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			var value = GetValue(key);
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
		public T GetAttribute<T>(string key, T failover)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			var value = GetValue(key);
			T returnvalue;

			if (value != null)
			{
				if (m_filesystem.GetSubSystem<StringConverter>().TryConvert(value, out returnvalue))
				{
					return returnvalue;
				}

				Log.Write(LogLevel.Warning, LogSystem.FileSystem, "Cannot convert '{1}' for key '{0}' to type: {2}", key, value, typeof(T).Name);
			}

			return failover;
		}

		/// <summary>
		/// Returns the title of this section.
		/// </summary>
		/// <returns>The title of this section.</returns>
		public override string ToString()
		{
			return Title;
		}

		/// <summary>
		/// Returns the value associated with the given key.
		/// </summary>
		/// <param name="key">The key associated with the requested value.</param>
		/// <returns>The value for the given key is it exists; null otherwise.</returns>
		private string GetValue(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			var sc = StringComparer.OrdinalIgnoreCase;
			foreach (var data in ParsedLines)
			{
				if (sc.Equals(key, data.Key))
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
		public string Title => m_title;

		/// <summary>
		/// Returns an iterator for read only access of the lines of this section.
		/// </summary>
		/// <returns>An iterator for read only access of the lines of this section.</returns>
		public ListIterator<string> Lines => new ListIterator<string>(m_lines);

		/// <summary>
		/// Returns an iterator for read only access of the parsed lines of this section.
		/// </summary>
		/// <returns>An iterator for read only access of the parsed lines of this section.</returns>
		public ListIterator<KeyValuePair<string, string>> ParsedLines => new ListIterator<KeyValuePair<string, string>>(m_parsedlines);

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly FileSystem m_filesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_title;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<string> m_lines;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<KeyValuePair<string, string>> m_parsedlines;

		#endregion
	}
}
