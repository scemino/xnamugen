using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class PreIntro : Base
	{
		public PreIntro(FightEngine engine)
			: base(engine, RoundState.PreIntro)
		{
		}

		void SetPlayer(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

			Engine.Entities.Add(player);

			player.StateManager.ChangeState(0);
			player.SetLocalAnimation(0, 0);
			player.PlayerControl = PlayerControl.NoControl;
			player.Life = player.Constants.MaximumLife;
			player.SoundManager.Stop();
			player.JugglePoints = player.Constants.AirJuggle;
			player.StateManager.ChangeState(StateMachine.StateNumber.Initialize);
            player.OffensiveInfo.Reset();
            player.DefensiveInfo.Reset();

			if (player.Team.Side == TeamSide.Left)
			{
				player.CurrentLocation = Engine.Stage.P1Start;
				player.CurrentFacing = Engine.Stage.P1Facing;
			}
			else
			{
				player.CurrentLocation = Engine.Stage.P2Start;
				player.CurrentFacing = Engine.Stage.P2Facing;
			}
		}

		protected override void OnFirstTick()
		{
			base.OnFirstTick();

			Engine.Clock.Time = Engine.GetSubSystem<InitializationSettings>().RoundLength;

			Engine.Camera.Location = new Point(0, 0);

			Engine.Entities.Clear();

			Engine.Team1.Display.ComboCounter.Reset();
			Engine.Team1.DoAction(SetPlayer);

			Engine.Team2.Display.ComboCounter.Reset(); 
			Engine.Team2.DoAction(SetPlayer);
		}

		protected override Elements.Base GetElement()
		{
			return null;
		}

		public override Boolean IsFinished()
		{
			return TickCount == Engine.RoundInformation.IntroDelay;
		}
	}
}