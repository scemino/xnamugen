using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class CombatOver : Base
	{
		enum WinType { None, KO, DoubleKO, TimeOut, Draw }

		public CombatOver(FightEngine engine)
			: base(engine, RoundState.PreOver)
		{
			m_wintype = WinType.None;
		}

		void RemovePlayerControl(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

			player.PlayerControl = PlayerControl.NoControl;
		}

		protected override void OnFirstTick()
		{
			if (Engine.Team1.VictoryStatus.LoseTime == true || Engine.Team2.VictoryStatus.LoseTime == true)
			{
				m_wintype = WinType.TimeOut;
			}
			else if (Engine.Team1.VictoryStatus.Lose == true && Engine.Team2.VictoryStatus.Lose == true)
			{
				m_wintype = WinType.DoubleKO;
			}
			else if (Engine.Team1.VictoryStatus.Lose == true || Engine.Team2.VictoryStatus.Lose == true)
			{
				m_wintype = WinType.KO;
			}
			else
			{
				m_wintype = WinType.Draw;
			}

			base.OnFirstTick();
		}

		public override void Update()
		{
			base.Update();

			if (TickCount < Engine.RoundInformation.KOSlowTime && Engine.Assertions.NoKOSlow == false && (m_wintype == WinType.KO || m_wintype == WinType.DoubleKO))
			{
				Engine.Speed = GameSpeed.Slow;
			}
			else
			{
				Engine.Speed = GameSpeed.Normal;
			}

			if (TickCount == Engine.RoundInformation.OverWaitTime)
			{
				Engine.Team1.DoAction(RemovePlayerControl);
				Engine.Team2.DoAction(RemovePlayerControl);
			}
		}

		protected override Elements.Base GetElement()
		{
			switch (m_wintype)
			{
				case WinType.DoubleKO:
					return Engine.Elements.GetElement("DKO");

				case WinType.Draw:
					return null;

				case WinType.KO:
					return Engine.Elements.GetElement("KO");

				case WinType.TimeOut:
					return Engine.Elements.GetElement("TO");

				default:
					throw new ArgumentOutOfRangeException("m_wintype");
			}
		}

		public override Boolean IsFinished()
		{
			if (TickCount < Engine.RoundInformation.OverWaitTime) return false;

			switch (m_wintype)
			{
				case WinType.KO:
				case WinType.DoubleKO:
					if (Engine.Team1.VictoryStatus.LoseKO == true && Engine.Team1.GetDyingState() != DyingState.Dead) return false;
					if (Engine.Team2.VictoryStatus.LoseKO == true && Engine.Team2.GetDyingState() != DyingState.Dead) return false;
					return TickCount - Engine.RoundInformation.OverWaitTime > Engine.RoundInformation.WinTime;

				case WinType.TimeOut:
				case WinType.Draw:
					return TickCount - Engine.RoundInformation.OverWaitTime > Engine.RoundInformation.WinTime;

				default:
					throw new ArgumentOutOfRangeException("m_wintype");
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		WinType m_wintype;

		#endregion
	}
}