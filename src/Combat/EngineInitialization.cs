using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class EngineInitialization
	{
		public EngineInitialization(CombatMode mode, PlayerProfile p1, int p1palette, PlayerProfile p2, int p2palette, StageProfile stage):
			this(mode, p1, p1palette, p2, p2palette, stage, Environment.TickCount)
		{
		}

		public EngineInitialization(CombatMode mode, PlayerProfile p1, int p1palette, PlayerProfile p2, int p2palette, StageProfile stage, int seed)
		{
			if (mode == CombatMode.None) throw new ArgumentOutOfRangeException(nameof(mode));
			if (p1 == null) throw new ArgumentNullException(nameof(p1));
			if (p1palette < 0 || p1palette > 11) throw new ArgumentOutOfRangeException(nameof(p1palette));
			if (p2 == null) throw new ArgumentNullException(nameof(p2));
			if (p2palette < 0 || p2palette > 11) throw new ArgumentOutOfRangeException(nameof(p2palette));
			if (stage == null) throw new ArgumentNullException(nameof(stage));

			m_mode = mode;
			m_p1 = new PlayerCreation(p1, p1.GetValidPaletteIndex(p1palette));
			m_p2 = new PlayerCreation(p2, p2.GetValidPaletteIndex(p2palette));
			m_stage = stage;
			m_seed = seed;
		}

		public CombatMode Mode => m_mode;

		public PlayerCreation P1 => m_p1;

		public PlayerCreation P2 => m_p2;

		public StageProfile Stage => m_stage;

		public int Seed => m_seed;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CombatMode m_mode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerCreation m_p1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerCreation m_p2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StageProfile m_stage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_seed;

		#endregion
	}
}