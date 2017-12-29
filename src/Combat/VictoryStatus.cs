using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class VictoryStatus
	{
		public VictoryStatus(Team team)
		{
			if (team == null) throw new ArgumentNullException(nameof(team));

			m_team = team;
		}

		public bool Win => WinKO || WinTime;

		public bool WinKO => LoseKO == false && m_team.OtherTeam.VictoryStatus.LoseKO;

		public bool WinTime
		{
			get
			{
				if (WinKO) return false;
				if (m_team.Engine.Clock.Time != 0) return false;

				var mylife = m_team.MainPlayer.Life + (m_team.TeamMate != null ? m_team.TeamMate.Life : 0);
				var otherlife = m_team.OtherTeam.MainPlayer.Life + (m_team.OtherTeam.TeamMate != null ? m_team.OtherTeam.TeamMate.Life : 0);

				return otherlife < mylife;
			}
		}

		public bool WinPerfect
		{
			get 
			{
				if (Win == false) return false;
				if (m_team.MainPlayer.Life != m_team.MainPlayer.Constants.MaximumLife) return false;
				if (m_team.TeamMate != null && m_team.TeamMate.Life != m_team.TeamMate.Constants.MaximumLife) return false;
				return true;
			}
		}

		public bool Lose => LoseKO || LoseTime;

		public bool LoseKO => m_team.MainPlayer.Life == 0 && (m_team.TeamMate == null || m_team.TeamMate.Life == 0);

		public bool LoseTime
		{
			get
			{
				if (LoseKO) return false;
				if (m_team.Engine.Clock.Time != 0) return false;

				var mylife = m_team.MainPlayer.Life + (m_team.TeamMate != null ? m_team.TeamMate.Life : 0);
				var otherlife = m_team.OtherTeam.MainPlayer.Life + (m_team.OtherTeam.TeamMate != null ? m_team.OtherTeam.TeamMate.Life : 0);

				return otherlife > mylife;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Team m_team;

		#endregion
	}
}