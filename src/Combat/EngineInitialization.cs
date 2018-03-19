using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class EngineInitialization
	{
        public EngineInitialization(CombatMode mode,
                                    PlayerProfile team1p1, int team1p1palette, PlayerMode team1p1mode,
                                    PlayerProfile team2p1, int team2p1palette, PlayerMode team2p1mode,
                                    StageProfile stage) :
        this(mode, TeamMode.None, TeamMode.None,
             team1p1, team1p1palette, team1p1mode, null, 0, PlayerMode.Human,
             team2p1, team2p1palette, team2p1mode, null, 0, PlayerMode.Human,
             stage)
        {
        }

        public EngineInitialization(CombatMode mode, TeamMode team1mode, TeamMode team2mode,
                                    PlayerProfile team1p1, int team1p1palette, PlayerMode team1p1mode,
                                    PlayerProfile team1p2, int team1p2palette, PlayerMode team1p2mode,
                                    PlayerProfile team2p1, int team2p1palette, PlayerMode team2p1mode,
                                    PlayerProfile team2p2, int team2p2palette, PlayerMode team2p2mode,
                                    StageProfile stage, int? seed = null)
		{
            seed = seed ?? Environment.TickCount;
			if (mode == CombatMode.None) throw new ArgumentOutOfRangeException(nameof(mode));
            if (team1p1 == null) throw new ArgumentNullException(nameof(team1p1));
            if (team1p1palette < 0 || team1p1palette > 11) throw new ArgumentOutOfRangeException(nameof(team1p1palette));
            if (team1p2palette < 0 || team1p2palette > 11) throw new ArgumentOutOfRangeException(nameof(team1p2palette));
            if (team2p1 == null) throw new ArgumentNullException(nameof(team2p1));
            if (team2p1palette < 0 || team2p1palette > 11) throw new ArgumentOutOfRangeException(nameof(team2p1palette));
            if (team2p2palette < 0 || team2p2palette > 11) throw new ArgumentOutOfRangeException(nameof(team2p2palette));
			if (stage == null) throw new ArgumentNullException(nameof(stage));

			m_mode = mode;
            Team1Mode = team1mode;
            Team2Mode = team2mode;
            m_team1p1 = new PlayerCreation(team1p1, team1p1.GetValidPaletteIndex(team1p1palette), team1p1mode);
            if (team1p2 != null)
            {
                m_team1p2 = new PlayerCreation(team1p2, team1p2.GetValidPaletteIndex(team1p2palette), team1p2mode);
            }
            m_team2p1 = new PlayerCreation(team2p1, team2p1.GetValidPaletteIndex(team2p1palette), team2p1mode);
            if (team2p2 != null)
            {
                m_team2p2 = new PlayerCreation(team2p2, team2p2.GetValidPaletteIndex(team2p2palette), team2p2mode);
            }
			m_stage = stage;
            m_seed = seed.Value;
		}

		public CombatMode Mode => m_mode;

        public TeamMode Team1Mode { get; }

        public PlayerCreation Team1P1 => m_team1p1;

        public PlayerCreation Team1P2 => m_team1p2;

        public TeamMode Team2Mode { get; }

        public PlayerCreation Team2P1 => m_team2p1;

        public PlayerCreation Team2P2 => m_team2p2;

		public StageProfile Stage => m_stage;

		public int Seed => m_seed;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CombatMode m_mode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerCreation m_team1p1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerCreation m_team2p1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PlayerCreation m_team1p2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PlayerCreation m_team2p2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StageProfile m_stage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_seed;

		#endregion
	}
}