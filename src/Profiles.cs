using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using xnaMugen.Collections;
using System.IO;

namespace xnaMugen
{
	class ProfileLoader : SubSystem
	{
		public ProfileLoader(SubSystems subsystems)
			: base(subsystems)
		{
			m_stages = new List<StageProfile>();
			m_players = new List<PlayerSelect>();
		}

		public override void Initialize()
		{
			BuildStageProfiles();
			BuildPlayerProfiles();
		}

		void BuildStageProfiles()
		{
			m_stages.Clear();

			TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile("data/select.def");
			TextSection textsection = textfile.GetSection("ExtraStages");

			foreach (String line in textsection.Lines)
			{
				TextFile stagetextfile = GetSubSystem<IO.FileSystem>().OpenTextFile(line);
				String stagename = stagetextfile.GetSection("Info").GetAttribute<String>("name");

				m_stages.Add(new StageProfile(line, stagename));
			}
		}

		void BuildPlayerProfiles()
		{
			m_players.Clear();

			TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile("data/select.def");
			TextSection textsection = textfile.GetSection("Characters");

			foreach (String line in textsection.Lines)
			{
				if (String.Equals(line, "random", StringComparison.OrdinalIgnoreCase) == true)
				{
					m_players.Add(new PlayerSelect(PlayerSelectType.Random, null));
				}
				else
				{
					String playerpath;
					String stagepath;
					if (ParseProfileLine(line, out playerpath, out stagepath) == true)
					{
						m_players.Add(new PlayerSelect(PlayerSelectType.Profile, new PlayerProfile(this, playerpath, stagepath)));
					}
					else
					{
						m_players.Add(null);
					}
				}
			}
		}

		Boolean ParseProfileLine(String line, out String playerpath, out String stagepath)
		{
			if (line == null) throw new ArgumentNullException("line");

			if (line == String.Empty)
			{
				playerpath = null;
				stagepath = null;
				return false;
			}

			Int32 comma_index = line.IndexOf(',');
			if (comma_index != -1)
			{
				StringSubString player_substring = new StringSubString(line, 0, comma_index);
				player_substring.TrimWhitespace();

				String playername = player_substring.ToString();
				playerpath = @"chars/" + playername + @"/" + playername + @".def";

				StringSubString stage_substring = new StringSubString(line, comma_index + 1, line.Length);
				stage_substring.TrimWhitespace();

				stagepath = stage_substring.ToString();
			}
			else
			{
				playerpath = @"chars/" + line + @"/" + line + @".def";
				stagepath = null;
			}

			if (GetSubSystem<IO.FileSystem>().DoesFileExist(playerpath) == false)
			{
				playerpath = null;
				stagepath = null;
				return false;
			}

			if (stagepath != null && GetSubSystem<IO.FileSystem>().DoesFileExist(stagepath) == false)
			{
				stagepath = null;
			}

			return true;
		}

		public ListIterator<StageProfile> StageProfiles
		{
			get { return new ListIterator<StageProfile>(m_stages); }
		}

		public ListIterator<PlayerSelect> PlayerProfiles
		{
			get { return new ListIterator<PlayerSelect>( m_players); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<StageProfile> m_stages;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<PlayerSelect> m_players;

		#endregion
	}

	class PlayerSelect
	{
		public PlayerSelect(PlayerSelectType type, PlayerProfile profile)
		{
			if (type == PlayerSelectType.Profile)
			{
				if (profile == null) throw new ArgumentNullException("profile", "profile cannot be null when type = Profile");

				m_type = PlayerSelectType.Profile;
				m_profile = profile;
			}
			else if (type == PlayerSelectType.Random)
			{
				if (profile != null) throw new ArgumentException("profile must be null when type = Random", "profile");

				m_type = PlayerSelectType.Random;
				m_profile = null;
			}
		}

		public PlayerSelectType SelectionType
		{
			get { return m_type; }
		}

		public PlayerProfile Profile
		{
			get { return m_profile; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerSelectType m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerProfile m_profile;

		#endregion
	}

	[DebuggerDisplay("{PlayerName}")]
	class PlayerProfile
	{
		public PlayerProfile(ProfileLoader loader, String playerpath, String stagepath)
		{
			if (playerpath == null) throw new ArgumentNullException("playerpath");
			if (loader == null) throw new ArgumentNullException("loader");

			m_loader = loader;

			//May be null
			m_playerstagepath = stagepath;

			IO.TextFile textfile = m_loader.SubSystems.GetSubSystem<IO.FileSystem>().OpenTextFile(playerpath);

			TextSection infosection = textfile.GetSection("info");
			if (infosection == null) throw new InvalidOperationException("No 'info' section in .def file");

			TextSection filesection = textfile.GetSection("files");
			if (filesection == null) throw new InvalidOperationException("No 'files' section in .def file");

			m_defpath = playerpath;
			m_basepath = Path.GetDirectoryName(textfile.Filepath);
			m_playername = infosection.GetAttribute<String>("name", String.Empty);
			m_displayname = infosection.GetAttribute<String>("displayname", m_playername);
			m_author = infosection.GetAttribute<String>("author", String.Empty);
			m_version = infosection.GetAttribute<String>("versiondate", String.Empty);
			m_mugenversion = infosection.GetAttribute<String>("mugenversion", String.Empty);
			m_paletteorder = BuildPaletteOrder(infosection.GetAttribute<String>("pal.defaults", null));
			m_commandfile = FilterPath(filesection.GetAttribute<String>("cmd", String.Empty));
			m_constantsfile = FilterPath(filesection.GetAttribute<String>("cns", String.Empty));
			m_commonstatefile = GetCommonStateFile(filesection.GetAttribute<String>("stcommon"));
			m_statesfiles = BuildStateFiles(filesection);
			m_spritesfile = FilterPath(filesection.GetAttribute<String>("sprite", String.Empty));
			m_animationfile = FilterPath(filesection.GetAttribute<String>("anim", String.Empty));
			m_soundfile = FilterPath(filesection.GetAttribute<String>("sound", String.Empty));
			m_palettefiles = BuildPaletteFiles(filesection);

			m_spritemanager = m_loader.SubSystems.GetSubSystem<Drawing.SpriteSystem>().CreateManager(SpritePath, false);

			SpriteManager.GetSprite(SpriteId.LargePortrait);
			SpriteManager.GetSprite(SpriteId.SmallPortrait);
		}

		ReadOnlyList<Int32> BuildPaletteOrder(String input)
		{
			if (input == null) return new ReadOnlyList<Int32>();

			List<Int32> order = new List<Int32>();

			String[] palinfo = input.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (String palnumber in palinfo)
			{
				Int32 palvalue = 0;
				if (Int32.TryParse(palnumber, out palvalue) == true) order.Add(palvalue);
			}

			return new ReadOnlyList<Int32>(order);
		}

		String GetCommonStateFile(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			if (String.Equals(input, "common1.cns", StringComparison.OrdinalIgnoreCase) == true)
			{
				String trypath = FilterPath(input);

				if (m_loader.SubSystems.GetSubSystem<IO.FileSystem>().DoesFileExist(trypath) == true)
				{
					return trypath;
				}
				else
				{
					return "data/common1.cns";
				}
			}
			else
			{
				return FilterPath(input);
			}
		}

		ReadOnlyList<String> BuildStateFiles(TextSection filesection)
		{
			if (filesection == null) throw new ArgumentNullException("filesection");

			SortedList<Int32, String> files = new SortedList<Int32, String>();
			files.Add(-2, CommonStateFile);
			files.Add(-1, CommandPath);

			foreach (var kvp in filesection.ParsedLines)
			{
				if (String.Compare(kvp.Key, 0, "st", 0, 2, StringComparison.OrdinalIgnoreCase) != 0) continue;

				if (String.Equals(kvp.Key, "st", StringComparison.OrdinalIgnoreCase) == true)
				{
					String path = FilterPath(kvp.Value);
					if (path != String.Empty) files[0] = path;
				}
				else
				{
					Int32 index = 0;
					if (Int32.TryParse(kvp.Key.Substring(2), out index) == true)
					{
						String path = FilterPath(kvp.Value);
						if (path != String.Empty) files[index + 1] = path;
					}
				}
			}

			return new ReadOnlyList<String>(files.Values);
		}

		ReadOnlyList<String> BuildPaletteFiles(TextSection filesection)
		{
			if (filesection == null) throw new ArgumentNullException("filesection");

			List<String> pals = new List<String>(12);
			for (Int32 i = 0; i != 12; ++i) pals.Add(FilterPath(filesection.GetAttribute<String>("pal" + (i + 1), String.Empty)));

			return new ReadOnlyList<String>(pals);
		}

		String FilterPath(String filepath)
		{
			if (String.IsNullOrEmpty(filepath) == true) return String.Empty;

			return Path.Combine(BasePath, filepath);
		}

		public Int32 GetValidPaletteIndex(Int32 index)
		{
			if (index < 0 || index > 11) throw new ArgumentOutOfRangeException("index");

			if (PaletteFiles[index] != String.Empty) return index;

			if (index >= 6)
			{
				if (PaletteFiles[index - 6] != String.Empty) return index - 6;
			}

			return 0;
		}

		public String DefinitionPath
		{
			get { return m_defpath; }
		}

		public String PlayerName
		{
			get { return m_playername; }
		}

		public String DisplayName
		{
			get { return m_displayname; }
		}

		public String Author
		{
			get { return m_author; }
		}

		public String Version
		{
			get { return m_version; }
		}

		public String MugenVersion
		{
			get { return m_mugenversion; }
		}

		public ReadOnlyList<Int32> PaletteOrder
		{
			get { return m_paletteorder; }
		}

		public String CommandPath
		{
			get { return m_commandfile; }
		}

		public String ConstantsPath
		{
			get { return m_constantsfile; }
		}

		public ReadOnlyList<String> StateFiles
		{
			get { return m_statesfiles; }
		}

		public String CommonStateFile
		{
			get { return m_commonstatefile; }
		}

		public String SpritePath
		{
			get { return m_spritesfile; }
		}

		public String AnimationPath
		{
			get { return m_animationfile; }
		}

		public String SoundPath
		{
			get { return m_soundfile; }
		}

		public String StagePath
		{
			get { return m_playerstagepath; }
		}

		public ReadOnlyList<String> PaletteFiles
		{
			get { return m_palettefiles; }
		}

		public Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public String BasePath
		{
			get { return m_basepath; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ProfileLoader m_loader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_defpath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_playername;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_displayname;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_author;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_mugenversion;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<Int32> m_paletteorder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_commandfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_constantsfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<String> m_statesfiles;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_commonstatefile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_spritesfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_animationfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_soundfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<String> m_palettefiles;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_basepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_playerstagepath;

		#endregion
	}
}