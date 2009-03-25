using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

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

            if (IsReady(Engine.Team1) == false || IsReady(Engine.Team2) == false) return false;
            return TickCount - Engine.RoundInformation.OverWaitTime > Engine.RoundInformation.WinTime;
		}

        Boolean IsReady(Team team)
        {
            if (team == null) throw new ArgumentNullException("team");

            if (IsDone(team.MainPlayer) == false) return false;
            if (team.TeamMate != null && IsDone(team.TeamMate) == false) return false;

            return true;
        }

        Boolean IsDone(Player player)
        {
            if (player == null) throw new ArgumentNullException("player");

            if (player.Life > 0)
            {
                return player.StateManager.CurrentState.Number == StateMachine.StateNumber.Standing;
            }
            else
            {
                return player.StateManager.CurrentState.Number == StateMachine.StateNumber.HitLieDead && player.CurrentVelocity == Vector2.Zero;
            }
        }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		WinType m_wintype;

		#endregion
	}
}