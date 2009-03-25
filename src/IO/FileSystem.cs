using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using xnaMugen.Collections;
using System.Reflection;

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
			m_sectiontitleregex = new Regex(@"^\s*\[(.+)\]\s*$", RegexOptions.IgnoreCase);
			m_textcache = new KeyedCollection<String, TextFile>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
			m_stringbuilder = new StringBuilder();
			m_textbuilder = new TextFileBuilder(this);
		}

		public override void Initialize()
		{
#if FRANTZX
			if (Environment.UserDomainName == "OmegaPlus") Directory.SetCurrentDirectory(@"D:\WinMugen");

            if (Environment.UserDomainName == "Omega7") Directory.SetCurrentDirectory(@"D:\WinMugen");
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

			TextFile textfile = m_textbuilder.Build(file);
			m_textcache.Add(textfile);
			return textfile;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_sectiontitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<String, TextFile> m_textcache;

		StringBuilder m_stringbuilder;

		readonly TextFileBuilder m_textbuilder;

		#endregion
	}
}
