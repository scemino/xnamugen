using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class Fighting : Base
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

		void GivePlayerControl(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

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

		public override Boolean IsFinished()
		{
			return Engine.Clock.Time == 0 || Engine.Team1.VictoryStatus.Lose == true || Engine.Team2.VictoryStatus.Lose == true;
		}
	}
}