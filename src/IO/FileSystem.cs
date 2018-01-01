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
    internal class FileSystem : SubSystem
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="subsystems">Collection of subsystems to be used by this class.</param>
        public FileSystem(SubSystems subsystems)
            : base(subsystems)
        {
            m_textcache = new KeyedCollection<string, TextFile>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
            m_titleregex = new Regex(@"^\s*\[(.+?)\]\s*$", RegexOptions.IgnoreCase);
            m_parsedlineregex = new Regex(@"^\s*(.+?)\s*=\s*(.+?)\s*$", RegexOptions.IgnoreCase);
        }

        public override void Initialize()
        {
            //#if FRANTZX
            //            Directory.SetCurrentDirectory(@"D:\WinMugen");
            //#endif

            Log.Write(LogLevel.Normal, LogSystem.FileSystem, "Base Directory: {0}", Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="filepath">The file to look for.</param>
        /// <returns>true is the file exists; false otherwise.</returns>
        public bool DoesFileExist(string filepath)
        {
            if (filepath == null) throw new ArgumentNullException(nameof(filepath));

            return System.IO.File.Exists(filepath);
        }

        /// <summary>
        /// Combines two paths strings.
        /// </summary>
        /// <param name="lhs">The first path to combine.</param>
        /// <param name="rhs">The second path to combine.</param>
        /// <returns>The combined path string.</returns>
        public string CombinePaths(string lhs, string rhs)
        {
            if (lhs == null) throw new ArgumentNullException(nameof(lhs));
            if (rhs == null) throw new ArgumentNullException(nameof(rhs));

            return Path.Combine(lhs, rhs);
        }

        /// <summary>
        /// Returns the directory path of the supplied path string.
        /// </summary>
        /// <param name="filepath">The path of a file or directory.</param>
        /// <returns>A string containing the filepath to the containing directory.</returns>
        public string GetDirectory(string filepath)
        {
            if (filepath == null) throw new ArgumentNullException(nameof(filepath));

            return Path.GetDirectoryName(filepath);
        }

        /// <summary>
        /// Opens a file with the given path.
        /// </summary>
        /// <param name="filepath">The path to the file to be opened.</param>
        /// <returns>A xnaMugen.IO.File for the given path.</returns>
        public File OpenFile(string filepath)
        {
            try
            {
                if (filepath == null) throw new ArgumentNullException(nameof(filepath));

                if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    filepath = filepath.Replace('\\', '/');
                }
				Log.Write(LogLevel.Normal, LogSystem.FileSystem, "Opening file: {0}", filepath);

				if (string.Compare(filepath, 0, "xnaMugen.", 0, 9, StringComparison.Ordinal) == 0)
				{
					var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filepath);
					if (stream != null) return new File(filepath, stream);
				}

				return new File(filepath, new FileStream(filepath, FileMode.Open, FileAccess.Read));
			}
			catch (FileNotFoundException)
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
		public TextFile OpenTextFile(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			if (m_textcache.Contains(filepath)) return m_textcache[filepath];

			return BuildTextFile(OpenFile(filepath));
		}

		/// <summary>
		/// Parses a xnaMugen.IO.File as a text file.
		/// </summary>
		/// <param name="file">The file to be parsed. The read position must be set to the beginning of the text location.</param>
		/// <returns>The xnaMugen.IO.TextFile parsed out of the given file.</returns>
		public TextFile BuildTextFile(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			if (m_textcache.Contains(file.Filepath)) return m_textcache[file.Filepath];

			var textfile = Build(file);
			m_textcache.Add(textfile);

			return textfile;
		}

		private TextFile Build(File file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var sections = new List<TextSection>();

			string sectiontitle = null;
			List<string> sectionlines = null;
			List<KeyValuePair<string, string>> sectionparsedlines = null;

			for (var line = file.ReadLine(); line != null; line = file.ReadLine())
			{
				line = line.Trim();

				var commentindex = line.IndexOf(';');
				if (commentindex != -1) line = line.Substring(0, commentindex);

				if (line == string.Empty) continue;

				var titlematch = m_titleregex.Match(line);
				if (titlematch.Success)
				{
					if (sectiontitle != null) sections.Add(new TextSection(this, sectiontitle, sectionlines, sectionparsedlines));

					sectiontitle = titlematch.Groups[1].Value;
					sectionlines = new List<string>();
					sectionparsedlines = new List<KeyValuePair<string, string>>();
				}
				else if (sectiontitle != null)
				{
					sectionlines.Add(line);

					var parsedmatch = m_parsedlineregex.Match(line);
					if (parsedmatch.Success)
					{
						var key = parsedmatch.Groups[1].Value;
						var value = parsedmatch.Groups[2].Value;

						if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"') value = value.Substring(1, value.Length - 2);

						sectionparsedlines.Add(new KeyValuePair<string, string>(key, value));
					}
				}
			}

			if (sectiontitle != null) sections.Add(new TextSection(this, sectiontitle, sectionlines, sectionparsedlines));

			return new TextFile(file.Filepath, sections);
		}

#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly KeyedCollection<string, TextFile> m_textcache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_titleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_parsedlineregex;

#endregion
	}
}
