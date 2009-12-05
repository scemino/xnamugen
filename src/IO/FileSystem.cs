using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using xnaMugen.Collections;

namespace xnaMugen.IO
{
	/// <summary>
	/// Interfaces between game code underlying OS filesystem.
	/// </summary>
	class FileSystem : SubSystem
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="subsystems">Collection of subsystems to be used by this class.</param>
		public FileSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_textcache = new KeyedCollection<String, TextFile>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
			m_titleregex = new Regex(@"^\s*\[(.+?)\]\s*$", RegexOptions.IgnoreCase);
			m_parsedlineregex = new Regex(@"^\s*(.+?)\s*=\s*(.+?)\s*$", RegexOptions.IgnoreCase);
		}

		public override void Initialize()
		{
#if FRANTZX
            Directory.SetCurrentDirectory(@"D:\WinMugen");
#endif

			Log.Write(LogLevel.Normal, LogSystem.FileSystem, "Base Directory: {0}", Directory.GetCurrentDirectory());
		}

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <param name="filepath">The file to look for.</param>
		/// <returns>true is the file exists; false otherwise.</returns>
		public Boolean DoesFileExist(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			return System.IO.File.Exists(filepath);
		}

		/// <summary>
		/// Combines two paths strings.
		/// </summary>
		/// <param name="lhs">The first path to combine.</param>
		/// <param name="rhs">The second path to combine.</param>
		/// <returns>The combined path string.</returns>
		public String CombinePaths(String lhs, String rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			return Path.Combine(lhs, rhs);
		}

		/// <summary>
		/// Returns the directory path of the supplied path string.
		/// </summary>
		/// <param name="filepath">The path of a file or directory.</param>
		/// <returns>A string containing the filepath to the containing directory.</returns>
		public String GetDirectory(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			return Path.GetDirectoryName(filepath);
		}

		/// <summary>
		/// Opens a file with the given path.
		/// </summary>
		/// <param name="filepath">The path to the file to be opened.</param>
		/// <returns>A xnaMugen.IO.File for the given path.</returns>
		public File OpenFile(String filepath)
		{
			try
			{
				if (filepath == null) throw new ArgumentNullException("filepath");

				Log.Write(LogLevel.Normal, LogSystem.FileSystem, "Opening file: {0}", filepath);

				if (String.Compare(filepath, 0, "xnaMugen.", 0, 9, StringComparison.Ordinal) == 0)
				{
					System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filepath);
					if (stream != null) return new File(filepath, stream);
				}

				return new File(filepath, new FileStream(filepath, FileMode.Open, FileAccess.Read));
			}
			catch (System.IO.FileNotFoundException)
			{
				Log.Write(LogLevel.Error, LogSystem.FileSystem, "File not found: {0}", filepath);
				throw;
			}
		}

		/// <summary>
		/// Opens a file with the given path and parses it as text.
		/// </summary>
		/// <param name="filepath">The path to the file to be opened.</param>
		/// <returns>A xnaMugen.IO.TextFile for the given path.</returns>
		public TextFile OpenTextFile(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			if (m_textcache.Contains(filepath) == true) return m_textcache[filepath];

			return BuildTextFile(OpenFile(filepath));
		}

		/// <summary>
		/// Parses a xnaMugen.IO.File as a text file.
		/// </summary>
		/// <param name="file">The file to be parsed. The read position must be set to the beginning of the text location.</param>
		/// <returns>The xnaMugen.IO.TextFile parsed out of the given file.</returns>
		public TextFile BuildTextFile(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			if (m_textcache.Contains(file.Filepath) == true) return m_textcache[file.Filepath];

			TextFile textfile = Build(file);
			m_textcache.Add(textfile);

			return textfile;
		}

		TextFile Build(File file)
		{
			if (file == null) throw new ArgumentNullException("file");

			List<TextSection> sections = new List<TextSection>();

			String sectiontitle = null;
			List<String> sectionlines = null;
			List<KeyValuePair<String, String>> sectionparsedlines = null;

			for (String line = file.ReadLine(); line != null; line = file.ReadLine())
			{
				line = line.Trim();

				Int32 commentindex = line.IndexOf(';');
				if (commentindex != -1) line = line.Substring(0, commentindex);

				if (line == String.Empty) continue;

				Match titlematch = m_titleregex.Match(line);
				if (titlematch.Success == true)
				{
					if (sectiontitle != null) sections.Add(new TextSection(this, sectiontitle, sectionlines, sectionparsedlines));

					sectiontitle = titlematch.Groups[1].Value;
					sectionlines = new List<String>();
					sectionparsedlines = new List<KeyValuePair<String, String>>();
				}
				else if (sectiontitle != null)
				{
					sectionlines.Add(line);

					Match parsedmatch = m_parsedlineregex.Match(line);
					if (parsedmatch.Success == true)
					{
						String key = parsedmatch.Groups[1].Value;
						String value = parsedmatch.Groups[2].Value;

						if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"') value = value.Substring(1, value.Length - 2);

						sectionparsedlines.Add(new KeyValuePair<String, String>(key, value));
					}
				}
			}

			if (sectiontitle != null) sections.Add(new TextSection(this, sectiontitle, sectionlines, sectionparsedlines));

			return new TextFile(file.Filepath, sections);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<String, TextFile> m_textcache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_titleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_parsedlineregex;

		#endregion
	}
}
