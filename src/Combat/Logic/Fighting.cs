using System;

namespace xnaMugen.Combat.Logic
{
	internal class Fighting : Base
	{
		public Fighting(FightEngine engine)
			: base(engine, RoundState.Fight)
		{
		}

		public override void Update()
		{
			base.Update();

			if (TickCount != 0 && TickCount % 60 == 0) Engine.Clock.Tick();
		}

		private void GivePlayerControl(Player player)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));

			player.PlayerControl = PlayerControl.InControl;
		}

		protected override void OnFirstTick()
		{
			base.OnFirstTick();

			Engine.Team1.DoAction(GivePlayerControl);
			Engine.Team2.DoAction(GivePlayerControl);
		}

		protected override Elements.Base GetElement()
		{
			return null;
		}

		public override bool IsFinished()
		{
			return Engine.Clock.Time == 0 || Engine.Team1.VictoryStatus.Lose || Engine.Team2.VictoryStatus.Lose;
		}
	}
}