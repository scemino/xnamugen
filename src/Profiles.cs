using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using xnaMugen.Collections;
using System.IO;

namespace xnaMugen
{
	internal class ProfileLoader : SubSystem
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

		public PlayerProfile FindPlayerProfile(string name, string version)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (version == null) throw new ArgumentNullException(nameof(version));

			foreach (var select in PlayerProfiles)
			{
				if (select.SelectionType != PlayerSelectType.Profile) continue;

				var profile = select.Profile;
				if (profile.PlayerName == name && profile.Version == version) return profile;
			}

			return null;
		}

		public StageProfile FindStageProfile(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			foreach (var profile in StageProfiles)
			{
				if (profile.Filepath == filepath) return profile;
			}

			return null;
		}

		private void BuildStageProfiles()
		{
			m_stages.Clear();

			var textfile = GetSubSystem<FileSystem>().OpenTextFile("data/select.def");
			var textsection = textfile.GetSection("ExtraStages");

			foreach (var line in textsection.Lines)
			{
				var stagetextfile = GetSubSystem<FileSystem>().OpenTextFile(line);
				var stagename = stagetextfile.GetSection("Info").GetAttribute<string>("name");

				m_stages.Add(new StageProfile(line, stagename));
			}
		}

		private void BuildPlayerProfiles()
		{
			m_players.Clear();

			var textfile = GetSubSystem<FileSystem>().OpenTextFile("data/select.def");
			var textsection = textfile.GetSection("Characters");

			foreach (var line in textsection.Lines)
			{
				if (string.Equals(line, "random", StringComparison.OrdinalIgnoreCase))
				{
					m_players.Add(new PlayerSelect(PlayerSelectType.Random, null));
				}
				else
				{
					string playerpath;
					string stagepath;
					if (ParseProfileLine(line, out playerpath, out stagepath))
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

		private bool ParseProfileLine(string line, out string playerpath, out string stagepath)
		{
			if (line == null) throw new ArgumentNullException(nameof(line));

			if (line == string.Empty)
			{
				playerpath = null;
				stagepath = null;
				return false;
			}

			var commaIndex = line.IndexOf(',');
			if (commaIndex != -1)
			{
				var playername = line.Substring(0, commaIndex).Trim();
				playerpath = @"chars/" + playername + @"/" + playername + @".def";

				stagepath = line.Substring(commaIndex + 1).Trim();
			}
			else
			{
				playerpath = @"chars/" + line + @"/" + line + @".def";
				stagepath = null;
			}

			if (GetSubSystem<FileSystem>().DoesFileExist(playerpath) == false)
			{
				playerpath = null;
				stagepath = null;
				return false;
			}

			if (stagepath != null && GetSubSystem<FileSystem>().DoesFileExist(stagepath) == false)
			{
				stagepath = null;
			}

			return true;
		}

		public ListIterator<StageProfile> StageProfiles => new ListIterator<StageProfile>(m_stages);

		public ListIterator<PlayerSelect> PlayerProfiles => new ListIterator<PlayerSelect>(m_players);

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<StageProfile> m_stages;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<PlayerSelect> m_players;

		#endregion
	}

	internal class PlayerSelect
	{
		public PlayerSelect(PlayerSelectType type, PlayerProfile profile)
		{
			if (type == PlayerSelectType.Profile)
			{
				if (profile == null) throw new ArgumentNullException(nameof(profile), "profile cannot be null when type = Profile");

				m_type = PlayerSelectType.Profile;
				m_profile = profile;
			}
			else if (type == PlayerSelectType.Random)
			{
				if (profile != null) throw new ArgumentException("profile must be null when type = Random", nameof(profile));

				m_type = PlayerSelectType.Random;
				m_profile = null;
			}
		}

		public PlayerSelectType SelectionType => m_type;

		public PlayerProfile Profile => m_profile;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerSelectType m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerProfile m_profile;

		#endregion
	}

	[DebuggerDisplay("{" + nameof(PlayerName) + "}")]
	internal class PlayerProfile
	{
		public PlayerProfile(ProfileLoader loader, string playerpath, string stagepath)
		{
			if (playerpath == null) throw new ArgumentNullException(nameof(playerpath));
			if (loader == null) throw new ArgumentNullException(nameof(loader));

			m_loader = loader;

			//May be null
			m_playerstagepath = stagepath;

			var textfile = m_loader.SubSystems.GetSubSystem<FileSystem>().OpenTextFile(playerpath);

			var infosection = textfile.GetSection("info");
			if (infosection == null) throw new InvalidOperationException("No 'info' section in .def file");

			var filesection = textfile.GetSection("files");
			if (filesection == null) throw new InvalidOperationException("No 'files' section in .def file");

			m_defpath = playerpath;
			m_basepath = Path.GetDirectoryName(textfile.Filepath);
			m_playername = infosection.GetAttribute("name", string.Empty);
			m_displayname = infosection.GetAttribute("displayname", m_playername);
			m_author = infosection.GetAttribute("author", string.Empty);
			m_version = infosection.GetAttribute("versiondate", string.Empty);
			m_mugenversion = infosection.GetAttribute("mugenversion", string.Empty);
			m_paletteorder = BuildPaletteOrder(infosection.GetAttribute<string>("pal.defaults", null));
			m_commandfile = FilterPath(filesection.GetAttribute("cmd", string.Empty));
			m_constantsfile = FilterPath(filesection.GetAttribute("cns", string.Empty));
			m_commonstatefile = GetCommonStateFile(filesection.GetAttribute<string>("stcommon"));
			m_statesfiles = BuildStateFiles(filesection);
			m_spritesfile = FilterPath(filesection.GetAttribute("sprite", string.Empty));
			m_animationfile = FilterPath(filesection.GetAttribute("anim", string.Empty));
			m_soundfile = FilterPath(filesection.GetAttribute("sound", string.Empty));
			m_palettefiles = BuildPaletteFiles(filesection);

			m_spritemanager = m_loader.SubSystems.GetSubSystem<Drawing.SpriteSystem>().CreateManager(SpritePath);

			SpriteManager.GetSprite(SpriteId.LargePortrait);
			SpriteManager.GetSprite(SpriteId.SmallPortrait);
		}

		private ReadOnlyList<int> BuildPaletteOrder(string input)
		{
			if (input == null) return new ReadOnlyList<int>();

			var order = new List<int>();

			var palinfo = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var palnumber in palinfo)
			{
				if (int.TryParse(palnumber, out var palvalue)) order.Add(palvalue);
			}

			return new ReadOnlyList<int>(order);
		}

		private string GetCommonStateFile(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			if (string.Equals(input, "common1.cns", StringComparison.OrdinalIgnoreCase))
			{
				var trypath = FilterPath(input);

				if (m_loader.SubSystems.GetSubSystem<FileSystem>().DoesFileExist(trypath))
				{
					return trypath;
				}

				return "data/common1.cns";
			}

			return FilterPath(input);
		}

		private ReadOnlyList<string> BuildStateFiles(TextSection filesection)
		{
			if (filesection == null) throw new ArgumentNullException(nameof(filesection));

			var files = new SortedList<int, string>();
			files.Add(-2, CommonStateFile);
			files.Add(-1, CommandPath);

			foreach (var kvp in filesection.ParsedLines)
			{
				if (string.Compare(kvp.Key, 0, "st", 0, 2, StringComparison.OrdinalIgnoreCase) != 0) continue;

				if (string.Equals(kvp.Key, "st", StringComparison.OrdinalIgnoreCase))
				{
					var path = FilterPath(kvp.Value);
					if (path != string.Empty) files[0] = path;
				}
				else
				{
					if (int.TryParse(kvp.Key.Substring(2), out var index))
					{
						var path = FilterPath(kvp.Value);
						if (path != string.Empty) files[index + 1] = path;
					}
				}
			}

			return new ReadOnlyList<string>(files.Values);
		}

		private ReadOnlyList<string> BuildPaletteFiles(TextSection filesection)
		{
			if (filesection == null) throw new ArgumentNullException(nameof(filesection));

			var pals = new List<string>(12);
			for (var i = 0; i != 12; ++i) pals.Add(FilterPath(filesection.GetAttribute("pal" + (i + 1), string.Empty)));

			return new ReadOnlyList<string>(pals);
		}

		private string FilterPath(string filepath)
		{
			if (string.IsNullOrEmpty(filepath)) return string.Empty;

			return Path.Combine(BasePath, filepath);
		}

		public int GetValidPaletteIndex(int index)
		{
			if (index < 0 || index > 11) throw new ArgumentOutOfRangeException(nameof(index));

			if (PaletteFiles[index] != string.Empty) return index;

			if (index >= 6)
			{
				if (PaletteFiles[index - 6] != string.Empty) return index - 6;
			}

			return 0;
		}

		public string DefinitionPath => m_defpath;

		public string PlayerName => m_playername;

		public string DisplayName => m_displayname;

		public string Author => m_author;

		public string Version => m_version;

		public string MugenVersion => m_mugenversion;

		public ReadOnlyList<int> PaletteOrder => m_paletteorder;

		public string CommandPath => m_commandfile;

		public string ConstantsPath => m_constantsfile;

		public ReadOnlyList<string> StateFiles => m_statesfiles;

		public string CommonStateFile => m_commonstatefile;

		public string SpritePath => m_spritesfile;

		public string AnimationPath => m_animationfile;

		public string SoundPath => m_soundfile;

		public string StagePath => m_playerstagepath;

		public ReadOnlyList<string> PaletteFiles => m_palettefiles;

		public Drawing.SpriteManager SpriteManager => m_spritemanager;

        public string BasePath => m_basepath;
		
        public ProfileLoader ProfileLoader => m_loader  ;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ProfileLoader m_loader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_defpath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_playername;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_displayname;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_author;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_mugenversion;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<int> m_paletteorder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_commandfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_constantsfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<string> m_statesfiles;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_commonstatefile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_spritesfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_animationfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_soundfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<string> m_palettefiles;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_basepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_playerstagepath;

		#endregion
	}
}