using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
	class MenuSystem : MainSystem
	{
		public MenuSystem(SubSystems subsystems)
			: base(subsystems)
		{
			TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/system.def");
			TextSection info = textfile.GetSection("info");
			TextSection files = textfile.GetSection("files");

			m_motifname = info.GetAttribute<String>("name", String.Empty);
			m_motifauthor = info.GetAttribute<String>("author", String.Empty);

			Dictionary<Int32, Font> fontmap = new Dictionary<Int32, Font>();

			Drawing.SpriteSystem spritesystem = GetSubSystem<Drawing.SpriteSystem>();

			String fontpath1 = files.GetAttribute<String>("font1", null);
			if (fontpath1 != null) fontmap[1] = spritesystem.LoadFont(fontpath1);

			String fontpath2 = files.GetAttribute<String>("font2", null);
			if (fontpath2 != null) fontmap[2] = spritesystem.LoadFont(fontpath2);

			String fontpath3 = files.GetAttribute<String>("font3", null);
			if (fontpath3 != null) fontmap[3] = spritesystem.LoadFont(fontpath3);

			m_fontmap = new Drawing.FontMap(fontmap);

			String soundpath = @"data/" + files.GetAttribute<String>("snd");
			String spritepath = @"data/" + files.GetAttribute<String>("spr");
			String animpath = textfile.Filepath;

			m_titlescreen = new TitleScreen(this, textfile.GetSection("Title Info"), spritepath, animpath, soundpath);
			m_titlescreen.LoadBackgrounds("Title", textfile);

			m_versusscreen = new VersusScreen(this, textfile.GetSection("VS Screen"), spritepath, animpath, soundpath);
			m_versusscreen.LoadBackgrounds("Versus", textfile);

			m_selectscreen = new SelectScreen(this, textfile.GetSection("Select Info"), spritepath, animpath, soundpath);
			m_selectscreen.LoadBackgrounds("Select", textfile);

			m_combatscreen = new CombatScreen(this);

			m_currentscreen = null;
			m_newscreen = null;
			m_fade = 0;
			m_fadespeed = 0;
			m_eventqueue = new Queue<Events.Base>();
		}

		public void PostEvent(Events.Base theevent)
		{
			if (theevent == null) throw new ArgumentNullException("theevent");

			m_eventqueue.Enqueue(theevent);
		}

		void SetScreen(Screen screen)
		{
			if (screen == null) throw new ArgumentNullException("screen");

			if (m_currentscreen != null)
			{
				m_newscreen = screen;

				PostEvent(new Events.FadeScreen(FadeDirection.Out));
			}
			else
			{
				m_currentscreen = screen;

				PostEvent(new Events.FadeScreen(FadeDirection.In));
			}
		}

		void FadeInScreen(Screen screen)
		{
			if (screen == null) throw new ArgumentNullException("screen");

			screen.Reset();
			screen.FadingIn();

			GetSubSystem<Input.InputSystem>().SaveInputState();
			screen.SetInput(GetSubSystem<Input.InputSystem>().CurrentInput);

			m_fade = 0;
			m_fadespeed = screen.FadeInTime;
		}

		void FadedInScreen(Screen screen)
		{
			if (screen == null) throw new ArgumentNullException("screen");

			screen.FadeInComplete();
		}

		void FadeOutScreen(Screen screen)
		{
			if (screen == null) throw new ArgumentNullException("screen");

			screen.FadingOut();

			GetSubSystem<Input.InputSystem>().LoadInputState();

			m_fade = 1;
			m_fadespeed = -screen.FadeOutTime;
		}

		void FadedOutScreen(Screen screen)
		{
			if (screen == null) throw new ArgumentNullException("screen");

			screen.FadeOutComplete();
		}

		public void Update(GameTime gametime)
		{
			RunEvents();

			DoFading();
			CurrentScreen.Update(gametime);
		}

		public void Draw(Boolean debugdraw)
		{
			m_currentscreen.Draw(debugdraw);
		}

		void RunEvents()
		{
			while (m_eventqueue.Count > 0)
			{
				var e = m_eventqueue.Dequeue();

				if (e is Events.SwitchScreen)
				{
					var ee = e as Events.SwitchScreen;

					switch (ee.Screen)
					{
						case ScreenType.Select:
							SetScreen(SelectScreen);
							break;

						case ScreenType.Title:
							SetScreen(TitleScreen);
							break;

						case ScreenType.Versus:
							SetScreen(VersusScreen);
							break;

						case ScreenType.Combat:
							SetScreen(CombatScreen);
							break;
					}
				}

				if (e is Events.FadeScreen)
				{
					var ee = e as Events.FadeScreen;

					switch (ee.Direction)
					{
						case FadeDirection.In:
							FadeInScreen(CurrentScreen);
							break;

						case FadeDirection.Out:
							FadeOutScreen(CurrentScreen);
							break;
					}
				}

				if (e is Events.SetupCombat)
				{
					var ee = e as Events.SetupCombat;

					GetMainSystem<Combat.FightEngine>().Set(ee.Initialization);
					VersusScreen.P1.Profile = ee.Initialization.P1.Profile;
					VersusScreen.P2.Profile = ee.Initialization.P2.Profile;
				}
			}
		}

		void DoFading()
		{
			if (m_fadespeed == 0)
			{
				GetSubSystem<Video.VideoSystem>().Tint = Color.White;
				return;
			}

			m_fade += 1.0f / m_fadespeed;

			if (m_fadespeed > 0)
			{
				m_fade = Math.Min(1, m_fade);

				if (m_fade == 1)
				{
					m_fadespeed = 0;

					FadedInScreen(CurrentScreen);
				}
			}

			if (m_fadespeed < 0)
			{
				m_fade = Math.Max(0, m_fade);

				if (m_fade == 0)
				{
					m_fadespeed = 0;

					FadedOutScreen(CurrentScreen);

					if (m_newscreen != null)
					{
						m_currentscreen = m_newscreen;
						m_newscreen = null;

						FadeInScreen(CurrentScreen);
					}
				}
			}

			GetSubSystem<Video.VideoSystem>().Tint = new Color(new Vector4(m_fade, m_fade, m_fade, m_fade));
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_titlescreen != null) m_titlescreen.Dispose();

				if (m_versusscreen != null) m_versusscreen.Dispose();

				if (m_selectscreen != null) m_selectscreen.Dispose();

				if (m_fontmap != null) m_fontmap.Dispose();
			}

			base.Dispose(disposing);
		}

		public String MotifName
		{
			get { return m_motifname; }
		}

		public String MotifAuthor
		{
			get { return m_motifauthor; }
		}

		public Drawing.FontMap FontMap
		{
			get { return m_fontmap; }
		}

		public TitleScreen TitleScreen
		{
			get { return m_titlescreen; }
		}

		public SelectScreen SelectScreen
		{
			get { return m_selectscreen; }
		}

		public VersusScreen VersusScreen
		{
			get { return m_versusscreen; }
		}

		public CombatScreen CombatScreen
		{
			get { return m_combatscreen; }
		}

		public Screen CurrentScreen
		{
			get { return m_currentscreen; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_motifname;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_motifauthor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.FontMap m_fontmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TitleScreen m_titlescreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SelectScreen m_selectscreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly VersusScreen m_versusscreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CombatScreen m_combatscreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Screen m_currentscreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Screen m_newscreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_fade;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_fadespeed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Queue<Events.Base> m_eventqueue;

		#endregion
	}
}