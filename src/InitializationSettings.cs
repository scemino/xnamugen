using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using xnaMugen.Collections;

namespace xnaMugen
{
	class InitializationSettings : SubSystem
	{
		public InitializationSettings(SubSystems subsystems)
			: base(subsystems)
		{
			m_screensize = Mugen.ScreenSize;
			m_useoldshader = false;
			m_vsync = true;
			m_diagonsticwindow = false;
			m_keeplog = false;
			m_preloadsprites = true;
			m_screenshotformat = ScreenShotFormat.Png;
			m_systemkeys = new Dictionary<SystemButton, Keys>();
			m_p1keys = new Dictionary<PlayerButton, Keys>();
			m_p2keys = new Dictionary<PlayerButton, Keys>();
			m_roundlength = 99;
            m_soundchannels = 10;
			m_recordreplay = false;
			m_quitafterreplay = true;
		}

		public override void Initialize()
		{
			IO.TextFile inifile = LoadIniFile();

			IO.TextSection videosettings = inifile.GetSection("Video Settings");
			if (videosettings != null)
			{
				m_screensize = videosettings.GetAttribute<Point>("ScreenSize", m_screensize);
				m_useoldshader = videosettings.GetAttribute<Boolean>("UseOldShader", m_useoldshader);
				m_vsync = videosettings.GetAttribute<Boolean>("VSync", m_vsync);
				m_screenshotformat = videosettings.GetAttribute<ScreenShotFormat>("ScreenShotFormat", m_screenshotformat);
			}

			IO.TextSection replaysettings = inifile.GetSection("Replay Settings");
			if (replaysettings != null)
			{
				m_recordreplay = replaysettings.GetAttribute<Boolean>("RecordReplay", m_recordreplay);
				m_quitafterreplay = replaysettings.GetAttribute<Boolean>("QuitAfterReplay", m_quitafterreplay);
			}

			IO.TextSection debugsettings = inifile.GetSection("Debug Settings");
			if (debugsettings != null)
			{
				m_diagonsticwindow = debugsettings.GetAttribute<Boolean>("ShowDiagnosticWindow", m_diagonsticwindow);
				m_keeplog = debugsettings.GetAttribute<Boolean>("Keep Log", m_keeplog);
			}

			IO.TextSection gamesettings = inifile.GetSection("Game Settings");
			if (gamesettings != null)
			{
				m_preloadsprites = gamesettings.GetAttribute<Boolean>("PreloadCharacterSprites", m_preloadsprites);
                m_roundlength = gamesettings.GetAttribute<Int32>("RoundLength", m_roundlength);
                m_soundchannels = gamesettings.GetAttribute<Int32>("SoundChannels", m_soundchannels);
            }

			IO.TextSection systemkeys = inifile.GetSection("System Keys");
			if (systemkeys != null)
			{
				foreach (SystemButton systembutton in Enum.GetValues(typeof(SystemButton)))
				{
					if (systembutton == SystemButton.None) continue;

					Keys key = systemkeys.GetAttribute<Keys>(systembutton.ToString(), Keys.None);
					if (key == Keys.None) continue;

					m_systemkeys.Add(systembutton, key);
				}
			}

			IO.TextSection p1keys = inifile.GetSection("Player 1 Keys");
			if (p1keys != null)
			{
				foreach (PlayerButton playerbutton in Enum.GetValues(typeof(PlayerButton)))
				{
					if (playerbutton == PlayerButton.None) continue;

					Keys key = p1keys.GetAttribute<Keys>(playerbutton.ToString(), Keys.None);
					if (key == Keys.None) continue;

					m_p1keys.Add(playerbutton, key);
				}
			}

			IO.TextSection p2keys = inifile.GetSection("Player 2 Keys");
			if (p2keys != null)
			{
				foreach (PlayerButton playerbutton in Enum.GetValues(typeof(PlayerButton)))
				{
					if (playerbutton == PlayerButton.None) continue;

					Keys key = p2keys.GetAttribute<Keys>(playerbutton.ToString(), Keys.None);
					if (key == Keys.None) continue;

					m_p2keys.Add(playerbutton, key);
				}
			}

#if TEST
			m_screensize = Mugen.ScreenSize * 3;
#endif
		}

		IO.TextFile LoadIniFile()
		{
#if FRANTZX
			String exepath = Environment.CommandLine.Substring(1, Environment.CommandLine.IndexOf("\"", 1) - 1);
			String dir = System.IO.Path.GetDirectoryName(exepath);
			String inipath = System.IO.Path.Combine(dir, @"xnaMugen.ini");
			
			return SubSystems.GetSubSystem<IO.FileSystem>().OpenTextFile(inipath);
#else
			return GetSubSystem<IO.FileSystem>().OpenTextFile(@"xnaMugen.ini");
#endif
		}

		public Point ScreenSize
		{
			get { return m_screensize; }
		}

		public Boolean UseOldShader
		{
			get { return m_useoldshader; }
		}

		public ScreenShotFormat ScreenShotFormat
		{
			get { return m_screenshotformat; }
		}

		public Boolean VSync
		{
			get { return m_vsync; }
		}

		public Boolean ShowDiagnosticWindow
		{
			get { return m_diagonsticwindow; }
		}

		public Boolean KeepLog
		{
			get { return m_keeplog; }
		}

		public Boolean PreloadCharacterSprites
		{
			get { return m_preloadsprites; }
		}

		public DictionaryIterator<SystemButton, Keys> SystemKeys
		{
			get { return new DictionaryIterator<SystemButton, Keys>(m_systemkeys); }
		}

		public DictionaryIterator<PlayerButton, Keys> Player1Keys
		{
			get { return new DictionaryIterator<PlayerButton, Keys>(m_p1keys); }
		}

		public DictionaryIterator<PlayerButton, Keys> Player2Keys
		{
			get { return new DictionaryIterator<PlayerButton, Keys>(m_p2keys); }
		}

		public Int32 RoundLength
		{
			get { return m_roundlength; }
		}

		public Int32 SoundChannels
		{
			get { return m_soundchannels; }
		}

		public Boolean RecordReplay
		{
			get { return m_recordreplay; }
		}

		public Boolean QuitAfterReplay
		{
			get { return m_quitafterreplay; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Point m_screensize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_useoldshader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_vsync;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_diagonsticwindow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_keeplog;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_preloadsprites;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ScreenShotFormat m_screenshotformat;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Dictionary<SystemButton, Keys> m_systemkeys;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Dictionary<PlayerButton, Keys> m_p1keys;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Dictionary<PlayerButton, Keys> m_p2keys;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_roundlength;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_soundchannels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_recordreplay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_quitafterreplay;

		#endregion
	}
}