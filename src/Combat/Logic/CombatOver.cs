using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat.Logic
{
	internal class CombatOver : Base
	{
		private enum WinType { None, KO, DoubleKO, TimeOut, Draw }

		public CombatOver(FightEngine engine)
			: base(engine, RoundState.PreOver)
		{
			m_wintype = WinType.None;
		}

		private void RemovePlayerControl(Player player)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));

			player.PlayerControl = PlayerControl.NoControl;
		}

		protected override void OnFirstTick()
		{
			if (Engine.Team1.VictoryStatus.LoseTime || Engine.Team2.VictoryStatus.LoseTime)
			{
				m_wintype = WinType.TimeOut;
			}
			else if (Engine.Team1.VictoryStatus.Lose && Engine.Team2.VictoryStatus.Lose)
			{
				m_wintype = WinType.DoubleKO;
			}
			else if (Engine.Team1.VictoryStatus.Lose || Engine.Team2.VictoryStatus.Lose)
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

		public override bool IsFinished()
		{
			if (TickCount < Engine.RoundInformation.OverWaitTime) return false;

            if (IsReady(Engine.Team1) == false || IsReady(Engine.Team2) == false) return false;
            return TickCount - Engine.RoundInformation.OverWaitTime > Engine.RoundInformation.WinTime;
		}

		private bool IsReady(Team team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));

            if (IsDone(team.MainPlayer) == false) return false;
            if (team.TeamMate != null && IsDone(team.TeamMate) == false) return false;

            return true;
        }

		private bool IsDone(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (player.Life > 0)
            {
                return player.StateManager.CurrentState.Number == StateMachine.StateNumber.Standing;
            }

	        return player.StateManager.CurrentState.Number == StateMachine.StateNumber.HitLieDead && player.CurrentVelocity == Vector2.Zero;
        }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private WinType m_wintype;

		#endregion
	}
}