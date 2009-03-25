using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	class CombatScreen : Screen
	{
		public CombatScreen(MenuSystem menusystem)
			: base(menusystem)
		{
		}

		void CancelCombat(Boolean pressed)
		{
			if (pressed == true)
			{
				Mugen mugen = MenuSystem.SubSystems.Game as Mugen;
				mugen.Pause = PauseState.Unpaused;

				MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
			}
		}

        public override void FadeOutComplete()
        {
            base.FadeOutComplete();

            MenuSystem.GetSubSystem<Audio.SoundSystem>().StopAllSounds();
        }

		public override void Reset()
		{
			base.Reset();

			FightEngine.Reset();
		}

		public override void SetInput(xnaMugen.Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			inputstate[0].Add(SystemButton.Quit, CancelCombat);

			FightEngine.SetInput(inputstate);
		}

		public override void Update(GameTime gametime)
		{
			base.Update(gametime);

			FightEngine.Update(gametime);
		}

		public override void Draw(Boolean debugdraw)
		{
			base.Draw(debugdraw);

			FightEngine.Draw(debugdraw);
		}

		Combat.FightEngine FightEngine
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
	}
}