using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	class VictoryStatus
	{
		public VictoryStatus(Team team)
		{
			if (team == null) throw new ArgumentNullException("team");

			m_team = team;
		}

		public Boolean Win
		{
			get { return WinKO || WinTime; }
		}

		public Boolean WinKO
		{
			get { return LoseKO == false && m_team.OtherTeam.VictoryStatus.LoseKO == true; }
		}

		public Boolean WinTime
		{
			get
			{
				if (WinKO == true) return false;
				if (m_team.Engine.Clock.Time != 0) return false;

				Int32 mylife = m_team.MainPlayer.Life + (m_team.TeamMate != null ? m_team.TeamMate.Life : 0);
				Int32 otherlife = m_team.OtherTeam.MainPlayer.Life + (m_team.OtherTeam.TeamMate != null ? m_team.OtherTeam.TeamMate.Life : 0);

				return otherlife < mylife;
			}
		}

		public Boolean WinPerfect
		{
			get 
			{
				if (Win == false) return false;
				if (m_team.MainPlayer.Life != m_team.MainPlayer.Constants.MaximumLife) return false;
				if (m_team.TeamMate != null && m_team.TeamMate.Life != m_team.TeamMate.Constants.MaximumLife) return false;
				return true;
			}
		}

		public Boolean Lose
		{
			get { return LoseKO || LoseTime; }
		}

		public Boolean LoseKO
		{
			get { return m_team.MainPlayer.Life == 0 && (m_team.TeamMate == null || m_team.TeamMate.Life == 0); }
		}

		public Boolean LoseTime
		{
			get
			{
				if (LoseKO == true) return false;
				if (m_team.Engine.Clock.Time != 0) return false;

				Int32 mylife = m_team.MainPlayer.Life + (m_team.TeamMate != null ? m_team.TeamMate.Life : 0);
				Int32 otherlife = m_team.OtherTeam.MainPlayer.Life + (m_team.OtherTeam.TeamMate != null ? m_team.OtherTeam.TeamMate.Life : 0);

				return otherlife > mylife;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Team m_team;

		#endregion
	}
}