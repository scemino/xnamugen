using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

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
		public Mugen()
		{
			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
			IsMouseVisible = true;

			GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
			graphics.MinimumVertexShaderProfile = ShaderProfile.VS_1_1;
			graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
			graphics.PreferredBackBufferWidth = Mugen.ScreenSize.X;
			graphics.PreferredBackBufferHeight = Mugen.ScreenSize.Y;
			graphics.ApplyChanges();

			m_pause = PauseState.Unpaused;
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

			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.Pause, this.TogglePause);
			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.PauseStep, this.TogglePauseStep);
			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.DebugDraw, this.ToggleDebugDraw);
			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.TakeScreenshot, this.TakeScreenshot);

			base.Initialize();
		}

		/// <summary>
		/// Setup code called before game runs.
		/// </summary>
		protected override void BeginRun()
		{
			base.BeginRun();

			m_subsystems.GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.SwitchScreen(ScreenType.Title));
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

			if (Pause == PauseState.Unpaused || Pause == PauseState.PauseStep)
			{
				m_subsystems.GetMainSystem<Menus.MenuSystem>().Update(gameTime);
			}

			if (Pause == PauseState.PauseStep) m_pause = PauseState.Paused;
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
		/// Input handler for toggling pause state.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		void TogglePause(Boolean pressed)
		{
			if (pressed == true)
			{
				if (Pause == PauseState.Paused || Pause == PauseState.PauseStep)
				{
					m_pause = PauseState.Unpaused;
					m_subsystems.GetSubSystem<Audio.SoundSystem>().UnPauseSounds();
				}
				else
				{
					m_pause = PauseState.Paused;
					m_subsystems.GetSubSystem<Audio.SoundSystem>().PauseSounds();
				}
			}
		}

		/// <summary>
		/// Input handler for toggling pause step state.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		void TogglePauseStep(Boolean pressed)
		{
			if (pressed == true)
			{
				if (Pause == PauseState.Paused) m_pause = PauseState.PauseStep;
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
		/// Gets or set whether and how the game is paused.
		/// </summary>
		/// <returns>Unpaused if the game is not paused; Paused if the game is paused; PauseStep if the game is paused but will still run for one tick.</returns>
		public PauseState Pause
		{
			get { return m_pause; }

			set { m_pause = value; }
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
		SubSystems m_subsystems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		PauseState m_pause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_debugdraw;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_takescreeshot;

		#endregion
	}
}