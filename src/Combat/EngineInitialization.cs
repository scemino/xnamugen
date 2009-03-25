using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	class EngineInitialization
	{
		public EngineInitialization(CombatMode mode, PlayerProfile p1, Int32 p1palette, PlayerProfile p2, Int32 p2palette, StageProfile stage)
		{
			if (mode == CombatMode.None) throw new ArgumentOutOfRangeException("mode");
			if (p1 == null) throw new ArgumentNullException("p1");
			if (p1palette < 0 || p1palette > 11) throw new ArgumentOutOfRangeException("p1palette");
			if (p2 == null) throw new ArgumentNullException("p2");
			if (p2palette < 0 || p2palette > 11) throw new ArgumentOutOfRangeException("p2palette");
			if (stage == null) throw new ArgumentNullException("stage");

			m_mode = mode;
			m_p1 = new PlayerCreation(p1, p1.GetValidPaletteIndex(p1palette));
			m_p2 = new PlayerCreation(p2, p2.GetValidPaletteIndex(p2palette));
			m_stage = stage;
		}

		public CombatMode Mode
		{
			get { return m_mode; }
		}

		public PlayerCreation P1
		{
			get { return m_p1; }
		}

		public PlayerCreation P2
		{
			get { return m_p2; }
		}

		public StageProfile Stage
		{
			get { return m_stage; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CombatMode m_mode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerCreation m_p1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerCreation m_p2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StageProfile m_stage;

		#endregion
	}
}