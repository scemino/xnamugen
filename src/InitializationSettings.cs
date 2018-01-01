using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using xnaMugen.Collections;

namespace xnaMugen
{
	internal class InitializationSettings : SubSystem
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
			var inifile = LoadIniFile();

			var videosettings = inifile.GetSection("Video Settings");
			if (videosettings != null)
			{
				m_screensize = videosettings.GetAttribute("ScreenSize", m_screensize);
				m_useoldshader = videosettings.GetAttribute("UseOldShader", m_useoldshader);
				m_vsync = videosettings.GetAttribute("VSync", m_vsync);
				m_screenshotformat = videosettings.GetAttribute("ScreenShotFormat", m_screenshotformat);
			}

			var replaysettings = inifile.GetSection("Replay Settings");
			if (replaysettings != null)
			{
				m_recordreplay = replaysettings.GetAttribute("RecordReplay", m_recordreplay);
				m_quitafterreplay = replaysettings.GetAttribute("QuitAfterReplay", m_quitafterreplay);
			}

			var debugsettings = inifile.GetSection("Debug Settings");
			if (debugsettings != null)
			{
				m_diagonsticwindow = debugsettings.GetAttribute("ShowDiagnosticWindow", m_diagonsticwindow);
				m_keeplog = debugsettings.GetAttribute("Keep Log", m_keeplog);
			}

			var gamesettings = inifile.GetSection("Game Settings");
			if (gamesettings != null)
			{
				m_preloadsprites = gamesettings.GetAttribute("PreloadCharacterSprites", m_preloadsprites);
                m_roundlength = gamesettings.GetAttribute("RoundLength", m_roundlength);
                m_soundchannels = gamesettings.GetAttribute("SoundChannels", m_soundchannels);
            }

			var systemkeys = inifile.GetSection("System Keys");
			if (systemkeys != null)
			{
				foreach (SystemButton systembutton in Enum.GetValues(typeof(SystemButton)))
				{
					if (systembutton == SystemButton.None) continue;

					var key = systemkeys.GetAttribute(systembutton.ToString(), Keys.None);
					if (key == Keys.None) continue;

					m_systemkeys.Add(systembutton, key);
				}
			}

			var p1keys = inifile.GetSection("Player 1 Keys");
			if (p1keys != null)
			{
				foreach (PlayerButton playerbutton in Enum.GetValues(typeof(PlayerButton)))
				{
					if (playerbutton == PlayerButton.None) continue;

					var key = p1keys.GetAttribute(playerbutton.ToString(), Keys.None);
					if (key == Keys.None) continue;

					m_p1keys.Add(playerbutton, key);
				}
			}

			var p2keys = inifile.GetSection("Player 2 Keys");
			if (p2keys != null)
			{
				foreach (PlayerButton playerbutton in Enum.GetValues(typeof(PlayerButton)))
				{
					if (playerbutton == PlayerButton.None) continue;

					var key = p2keys.GetAttribute(playerbutton.ToString(), Keys.None);
					if (key == Keys.None) continue;

					m_p2keys.Add(playerbutton, key);
				}
			}

#if TEST
			m_screensize = Mugen.ScreenSize * 3;
#endif
		}

		private IO.TextFile LoadIniFile()
		{
#if FRANTZX
            var dir = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
			var inipath = System.IO.Path.Combine(dir, @"xnaMugen.ini");
			
			return SubSystems.GetSubSystem<IO.FileSystem>().OpenTextFile(inipath);
#else
			return GetSubSystem<IO.FileSystem>().OpenTextFile(@"xnaMugen.ini");
#endif
		}

		public Point ScreenSize => m_screensize;

		public bool UseOldShader => m_useoldshader;

		public ScreenShotFormat ScreenShotFormat => m_screenshotformat;

		public bool VSync => m_vsync;

		public bool ShowDiagnosticWindow => m_diagonsticwindow;

		public bool KeepLog => m_keeplog;

		public bool PreloadCharacterSprites => m_preloadsprites;

		public DictionaryIterator<SystemButton, Keys> SystemKeys => new DictionaryIterator<SystemButton, Keys>(m_systemkeys);

		public DictionaryIterator<PlayerButton, Keys> Player1Keys => new DictionaryIterator<PlayerButton, Keys>(m_p1keys);

		public DictionaryIterator<PlayerButton, Keys> Player2Keys => new DictionaryIterator<PlayerButton, Keys>(m_p2keys);

		public int RoundLength => m_roundlength;

		public int SoundChannels => m_soundchannels;

		public bool RecordReplay => m_recordreplay;

		public bool QuitAfterReplay => m_quitafterreplay;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Point m_screensize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_useoldshader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_vsync;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_diagonsticwindow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_keeplog;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_preloadsprites;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ScreenShotFormat m_screenshotformat;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Dictionary<SystemButton, Keys> m_systemkeys;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Dictionary<PlayerButton, Keys> m_p1keys;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Dictionary<PlayerButton, Keys> m_p2keys;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_roundlength;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_soundchannels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_recordreplay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_quitafterreplay;

		#endregion
	}
}