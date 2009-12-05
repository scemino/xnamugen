using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace xnaMugen
{
	/// <summary>
	/// Main class of MUGEN engine.
	/// </summary>
	class Mugen : Game
	{
		/// <summary>
		/// Initializes a new instance of this class. Sets game timing and default screen size.
		/// </summary>
		public Mugen(String[] args)
		{
			if (args == null) throw new ArgumentNullException("args");

			m_args = new List<String>(args);

			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
			IsMouseVisible = true;

			GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
			graphics.MinimumVertexShaderProfile = ShaderProfile.VS_1_1;
			graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
			graphics.PreferredBackBufferWidth = Mugen.ScreenSize.X;
			graphics.PreferredBackBufferHeight = Mugen.ScreenSize.Y;
			graphics.ApplyChanges();

			m_debugdraw = false;
			m_takescreeshot = false;
		}

		/// <summary>
		/// Initializes engine subsystems, including drawing, sound and input.
		/// </summary>
		protected override void Initialize()
		{
			m_subsystems = new SubSystems(this);
			m_subsystems.LoadAllSubSystems();

			m_subsystems.GetSubSystem<IO.FileSystem>().Initialize();
			m_subsystems.GetSubSystem<InitializationSettings>().Initialize();
			m_subsystems.GetSubSystem<ProfileLoader>().Initialize();
			m_subsystems.GetSubSystem<Input.InputSystem>().Initialize();
			m_subsystems.GetSubSystem<Video.VideoSystem>().Initialize();
			m_subsystems.GetSubSystem<Audio.SoundSystem>().Initialize();
			m_subsystems.GetSubSystem<Diagnostics.DiagnosticSystem>().Initialize();

			m_subsystems.LoadAllMainSystems();

			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.DebugDraw, this.ToggleDebugDraw);
			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.TakeScreenshot, this.TakeScreenshot);

			base.Initialize();
		}

		Replay.Recording BuildRecording(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			ProfileLoader profiles = m_subsystems.GetSubSystem<ProfileLoader>();

			if (m_subsystems.GetSubSystem<IO.FileSystem>().DoesFileExist(filepath) == false) return null;

			IO.TextFile text = m_subsystems.GetSubSystem<IO.FileSystem>().OpenTextFile(filepath);
			IO.TextSection header = text.GetSection("xnaMugen Replay Header");
			IO.TextSection data = text.GetSection("xnaMugen Replay Data");

			if (header == null || data == null) return null;

			Int32 version = header.GetAttribute<Int32>("Version", 0);
			CombatMode mode = header.GetAttribute<CombatMode>("Combat Mode", CombatMode.None);
			String p1name = header.GetAttribute<String>("Player 1 Name", null);
			String p1version = header.GetAttribute<String>("Player 1 Version", null);
			Int32 p1palette = header.GetAttribute<Int32>("Player 1 Palette", Int32.MinValue);
			String p2name = header.GetAttribute<String>("Player 2 Name", null);
			String p2version = header.GetAttribute<String>("Player 2 Version", null);
			Int32 p2palette = header.GetAttribute<Int32>("Player 2 Palette", Int32.MinValue);
			String stagepath = header.GetAttribute<String>("Stage Path", null);
			Int32 seed = header.GetAttribute<Int32>("Seed", Int32.MinValue);

			if (version != 1 || mode == CombatMode.None || stagepath == null || seed == Int32.MinValue) return null;
			if (p1name == null || p1version == null || p1palette == Int32.MinValue) return null;

			PlayerProfile p1profile = profiles.FindPlayerProfile(p1name, p1version);
			PlayerProfile p2profile = profiles.FindPlayerProfile(p2name, p2version);
			StageProfile stageprofile = profiles.FindStageProfile(stagepath);

			if (p1profile == null || p2profile == null || stageprofile == null) return null;

			Combat.EngineInitialization initsettings = new Combat.EngineInitialization(mode, p1profile, p1palette, p2profile, p2palette, stageprofile, seed);

			List<Replay.RecordingData> replaydata = new List<Replay.RecordingData>();

			Regex line_regex = new Regex(@"^(\d+),\s*(\d+),\s*(\d+),\s*(\d+),\s*(\d+)$", RegexOptions.IgnoreCase);
			StringConverter converter = profiles.GetSubSystem<StringConverter>();
			foreach (String dataline in data.Lines)
			{
				Match match = line_regex.Match(dataline);
				if (match.Success == false) continue;

				Replay.RecordingData inputdata = new Replay.RecordingData(
					converter.Convert<Int32>(match.Groups[1].Value),
					converter.Convert<Int32>(match.Groups[2].Value),
					converter.Convert<Int32>(match.Groups[3].Value),
					converter.Convert<Int32>(match.Groups[4].Value),
					converter.Convert<Int32>(match.Groups[5].Value)
					);

				replaydata.Add(inputdata);
			}

			return new Replay.Recording(initsettings, replaydata);
		}

		/// <summary>
		/// Setup code called before game runs.
		/// </summary>
		protected override void BeginRun()
		{
			base.BeginRun();

			String recordingpath = (m_args.Count > 0) ? m_args[0] : String.Empty;

			Replay.Recording recording = BuildRecording(recordingpath);
			if (recording != null)
			{
				m_subsystems.GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.LoadReplay(recording));
				m_subsystems.GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.SwitchScreen(ScreenType.Replay));
			}
			else
			{
				m_subsystems.GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.SwitchScreen(ScreenType.Title));
			}
		}

		protected override void EndRun()
		{
			base.EndRun();

			if (m_subsystems.GetSubSystem<InitializationSettings>().KeepLog == false)
			{
				Log.KillLog();
			}
		}

		/// <summary>
		/// Runs primary game logic. Handles pauses.
		/// </summary>
		/// <param name="gameTime">Time since last update.</param>
		protected override void Update(GameTime gameTime)
		{
			m_subsystems.GetSubSystem<Input.InputSystem>().Update();
			m_subsystems.GetMainSystem<Menus.MenuSystem>().Update(gameTime);
		}

		/// <summary>
		/// Draws graphics onto screen.
		/// </summary>
		/// <param name="gameTime">Time since last time Draw was called.</param>
		protected override void Draw(GameTime gameTime)
		{
			m_subsystems.GetSubSystem<Video.VideoSystem>().ClearScreen(Color.Black);

			m_subsystems.GetMainSystem<Menus.MenuSystem>().Draw(DebugDraw);

			if (m_takescreeshot == true)
			{
				m_takescreeshot = false;
				m_subsystems.GetSubSystem<Video.VideoSystem>().TakeScreenshot();
			}

			base.Draw(gameTime);
		}

		/// <summary>
		/// Releases resources used by this object.
		/// </summary>
		/// <param name="disposing">Determines if managed resources are to be released.</param>
		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if(m_subsystems != null) m_subsystems.Dispose();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Input handler for toggling the drawing of debug information.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		void ToggleDebugDraw(Boolean pressed)
		{
			if (pressed == true)
			{
				DebugDraw = !DebugDraw;
			}
		}

		/// <summary>
		/// Input handler for taking a screenshot.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		void TakeScreenshot(Boolean pressed)
		{
			if (pressed == true)
			{
				m_takescreeshot = true;
			}
		}

		/// <summary>
		/// Logical screen size of game.
		/// </summary>
		/// <returns>The screen size used by game logic.</returns>
		public static Point ScreenSize
		{
			get { return new Point(320, 240); }
		}

		/// <summary>
		/// Gets or sets whether debug information is drawn to screen.
		/// </summary>
		/// <returns>true if debug information is drawn; false otherwise.</returns>
		public Boolean DebugDraw
		{
			get { return m_debugdraw; }

			set { m_debugdraw = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<String> m_args;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		SubSystems m_subsystems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_debugdraw;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_takescreeshot;

		#endregion
	}
}