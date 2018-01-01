using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	internal class CombatScreen : Screen
	{
		public CombatScreen(MenuSystem menusystem)
			: base(menusystem)
		{
			m_pause = PauseState.Unpaused;
			m_recorder = new Replay.Recorder(this);
		}

		private void CancelCombat(bool pressed)
		{
			if (pressed)
			{
				Pause = PauseState.Unpaused;

				MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
			}
		}

		public override void FadingIn()
		{
			if (MenuSystem.GetSubSystem<InitializationSettings>().RecordReplay)
			{
				Recorder.StartRecording();
			}

			base.FadingIn();
		}

		public override void FadeOutComplete()
		{
			base.FadeOutComplete();

			Recorder.EndRecording();

			MenuSystem.GetSubSystem<Audio.SoundSystem>().StopAllSounds();
		}

		public override void Reset()
		{
			base.Reset();

			FightEngine.Reset();
			Recorder.Reset();
		}

		public override void SetInput(Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			if (Recorder.IsRecording) Recorder.SetInput(inputstate);

			inputstate[0].Add(SystemButton.Quit, CancelCombat);
			inputstate[0].Add(SystemButton.Pause, TogglePause);
			inputstate[0].Add(SystemButton.PauseStep, TogglePauseStep);

			FightEngine.SetInput(inputstate);
		}

		public override void Update(GameTime gametime)
		{
			base.Update(gametime);

			Recorder.Update();

			if (Pause == PauseState.Unpaused || Pause == PauseState.PauseStep)
			{
				FightEngine.Update(gametime);
			}

			if (Pause == PauseState.PauseStep) m_pause = PauseState.Paused;
		}

		public override void Draw(bool debugdraw)
		{
			base.Draw(debugdraw);

			FightEngine.Draw(debugdraw);
		}

		/// <summary>
		/// Input handler for toggling pause state.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed or released.</param>
		private void TogglePause(bool pressed)
		{
			if (pressed)
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
		private void TogglePauseStep(bool pressed)
		{
			if (pressed)
			{
				if (Pause == PauseState.Paused) m_pause = PauseState.PauseStep;
			}
		}

		public Combat.FightEngine FightEngine => MenuSystem.GetMainSystem<Combat.FightEngine>();

		public override int FadeInTime => FightEngine.RoundInformation.IntroDelay;

		public override int FadeOutTime => 20;

		public Replay.Recorder Recorder => m_recorder;

		/// <summary>
		/// Gets or set whether and how the game is paused.
		/// </summary>
		/// <returns>Unpaused if the game is not paused; Paused if the game is paused; PauseStep if the game is paused but will still run for one tick.</returns>
		public PauseState Pause
		{
			get => m_pause;

			set { m_pause = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Replay.Recorder m_recorder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PauseState m_pause;

		#endregion;
	}
}