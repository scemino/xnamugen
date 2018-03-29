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

                var mylife = GetLife(m_team);
                var otherlife = GetLife(m_team);
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

        public bool LoseKO => GetLife(m_team) == 0;

		public bool LoseTime
		{
			get
			{
				if (LoseKO) return false;
				if (m_team.Engine.Clock.Time != 0) return false;

                var mylife = GetLife(m_team);
                var otherlife = GetLife(m_team.OtherTeam);

				return otherlife > mylife;
			}
		}

        private static int GetLife(Team team)
        {
	        if (team.Mode != TeamMode.Turns) return team.MainPlayer.Life + (team.TeamMate?.Life ?? 0);
	        
	        if (team.OtherTeam.Wins.Count == 0)
	        {
		        return team.MainPlayer.Life;
	        }
	        return team.TeamMate.Life;
        }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Team m_team;

		#endregion
	}
}