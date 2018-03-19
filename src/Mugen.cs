using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace xnaMugen
{
	/// <summary>
	/// Main class of MUGEN engine.
	/// </summary>
	internal class Mugen : Game
	{
		/// <summary>
		/// Initializes a new instance of this class. Sets game timing and default screen size.
		/// </summary>
		public Mugen(string[] args)
		{
			if (args == null) throw new ArgumentNullException(nameof(args));

			m_args = new List<string>(args);

			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
			IsMouseVisible = true;

			var graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = ScreenSize.X;
			graphics.PreferredBackBufferHeight = ScreenSize.Y;
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

			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.DebugDraw, ToggleDebugDraw);
			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.TakeScreenshot, TakeScreenshot);

			base.Initialize();
		}

		private Replay.Recording BuildRecording(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			var profiles = m_subsystems.GetSubSystem<ProfileLoader>();

			if (m_subsystems.GetSubSystem<IO.FileSystem>().DoesFileExist(filepath) == false) return null;

			var text = m_subsystems.GetSubSystem<IO.FileSystem>().OpenTextFile(filepath);
			var header = text.GetSection("xnaMugen Replay Header");
			var data = text.GetSection("xnaMugen Replay Data");

			if (header == null || data == null) return null;

			var version = header.GetAttribute("Version", 0);
			var mode = header.GetAttribute("Combat Mode", CombatMode.None);
            var p1name = header.GetAttribute<string>("Player 1 Name", null);
            var p1version = header.GetAttribute<string>("Player 1 Version", null);
			var p1palette = header.GetAttribute("Player 1 Palette", int.MinValue);
            var p2name = header.GetAttribute<string>("Player 2 Name", null);
            var p2version = header.GetAttribute<string>("Player 2 Version", null);
			var p2palette = header.GetAttribute("Player 2 Palette", int.MinValue);
            var stagepath = header.GetAttribute<string>("Stage Path", null);
			var seed = header.GetAttribute("Seed", int.MinValue);

			if (version != 1 || mode == CombatMode.None || stagepath == null || seed == int.MinValue) return null;
			if (p1name == null || p1version == null || p1palette == int.MinValue) return null;

			var p1profile = profiles.FindPlayerProfile(p1name, p1version);
			var p2profile = profiles.FindPlayerProfile(p2name, p2version);
			var stageprofile = profiles.FindStageProfile(stagepath);

			if (p1profile == null || p2profile == null || stageprofile == null) return null;

            // TODO: fix team modes and p2 profiles
            var initsettings = new Combat.EngineInitialization(mode, TeamMode.None, TeamMode.None, 
                                                               p1profile, p1palette, PlayerMode.Human,
                                                               p1profile, p1palette, PlayerMode.Human,
                                                               p2profile, p2palette, PlayerMode.Human,
                                                               p2profile, p2palette, PlayerMode.Human,
                                                               stageprofile, seed);

			var replaydata = new List<Replay.RecordingData>();

			var line_regex = new Regex(@"^(\d+),\s*(\d+),\s*(\d+),\s*(\d+),\s*(\d+)$", RegexOptions.IgnoreCase);
			var converter = profiles.GetSubSystem<StringConverter>();
			foreach (var dataline in data.Lines)
			{
				var match = line_regex.Match(dataline);
				if (match.Success == false) continue;

				var inputdata = new Replay.RecordingData(
					converter.Convert<int>(match.Groups[1].Value),
					converter.Convert<int>(match.Groups[2].Value),
					converter.Convert<int>(match.Groups[3].Value),
					converter.Convert<int>(match.Groups[4].Value),
					converter.Convert<int>(match.Groups[5].Value)
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

            var recordingpath = m_args.Count > 0 ? m_args[0] : string.Empty;

			var recording = BuildRecording(recordingpath);
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

            if (m_takescreeshot)
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
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_subsystems?.Dispose();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Input handler for toggling the drawing of debug information.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		private void ToggleDebugDraw(bool pressed)
		{
			if (pressed)
			{
				DebugDraw = !DebugDraw;
			}
		}

		/// <summary>
		/// Input handler for taking a screenshot.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		private void TakeScreenshot(bool pressed)
		{
			if (pressed)
			{
				m_takescreeshot = true;
			}
		}

		/// <summary>
		/// Logical screen size of game.
		/// </summary>
		/// <returns>The screen size used by game logic.</returns>
		public static Point ScreenSize => new Point(320, 240);

		/// <summary>
		/// Gets or sets whether debug information is drawn to screen.
		/// </summary>
		/// <returns>true if debug information is drawn; false otherwise.</returns>
		public bool DebugDraw
		{
			get => m_debugdraw;

			set => m_debugdraw = value;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<string> m_args;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SubSystems m_subsystems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_debugdraw;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_takescreeshot;

		#endregion
	}
}