using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	class RecordedCombatScreen : Screen
	{
		public RecordedCombatScreen(MenuSystem menusystem)
			: base(menusystem)
		{
			m_pause = PauseState.Unpaused;
			m_over = false;
		}

		public void SetReplay(Replay.Recording recording)
		{
			if (recording == null) throw new ArgumentNullException("recording");

			m_recording = recording;
		}

		void CancelCombat(Boolean pressed)
		{
			if (pressed == true)
			{
				Pause = PauseState.Unpaused;

				MenuSystem.PostEvent(new Events.FadeScreen(FadeDirection.Out));
			}
		}

		public override void FadingIn()
		{
			base.FadingIn();
		}

		public override void FadeOutComplete()
		{
			base.FadeOutComplete();

			MenuSystem.SubSystems.Game.Exit();
		}

		public override void Reset()
		{
			base.Reset();

			m_over = false;
			FightEngine.Reset();
		}

		public override void SetInput(Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			inputstate[0].Add(SystemButton.Quit, CancelCombat);
			inputstate[0].Add(SystemButton.Pause, TogglePause);
			inputstate[0].Add(SystemButton.PauseStep, TogglePauseStep);

			//FightEngine.SetInput(inputstate);
		}

		public override void Update(GameTime gametime)
		{
			base.Update(gametime);

			if (m_over == true) return;

			if (FightEngine.TickCount >= m_recording.Data.Count)
			{
				if (MenuSystem.GetSubSystem<InitializationSettings>().QuitAfterReplay == true)
				{
					MenuSystem.PostEvent(new Events.FadeScreen(FadeDirection.Out));
				}

				m_over = true;
				return;
			}

			InjectRecordingInput();

			if (Pause == PauseState.Unpaused || Pause == PauseState.PauseStep)
			{
				FightEngine.Update(gametime);
			}

			if (Pause == PauseState.PauseStep) m_pause = PauseState.Paused;
		}

		void InjectRecordingInput()
		{
			Replay.RecordingData data = m_recording.Data[FightEngine.TickCount];

			Combat.Player p1 = FightEngine.Team1.MainPlayer;
			Combat.Player p2 = FightEngine.Team2.MainPlayer;
			Combat.Player p3 = FightEngine.Team1.TeamMate;
			Combat.Player p4 = FightEngine.Team2.TeamMate;

			if (p1 != null)
			{
				p1.RecieveInput((PlayerButton)Int32.MaxValue, false);
				p1.RecieveInput(data.Player1Input, true);
			}

			if (p2 != null)
			{
				p2.RecieveInput((PlayerButton)Int32.MaxValue, false);
				p2.RecieveInput(data.Player2Input, true);
			}

			if (p3 != null)
			{
				p3.RecieveInput((PlayerButton)Int32.MaxValue, false);
				p3.RecieveInput(data.Player3Input, true);
			}

			if (p4 != null)
			{
				p4.RecieveInput((PlayerButton)Int32.MaxValue, false);
				p4.RecieveInput(data.Player4Input, true);
			}
		}

		public override void Draw(Boolean debugdraw)
		{
			base.Draw(debugdraw);

			FightEngine.Draw(debugdraw);
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
					MenuSystem.GetSubSystem<Audio.SoundSystem>().UnPauseSounds();
				}
				else
				{
					m_pause = PauseState.Paused;
					MenuSystem.GetSubSystem<Audio.SoundSystem>().PauseSounds();
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

		public Combat.FightEngine FightEngine
		{
			get { return MenuSystem.GetMainSystem<Combat.FightEngine>(); }
		}

		public override Int32 FadeInTime
		{
			get { return FightEngine.RoundInformation.IntroDelay; }
		}

		public override Int32 FadeOutTime
		{
			get { return 20; }
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

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Replay.Recording m_recording;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		PauseState m_pause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_over;

		#endregion;
	}
}