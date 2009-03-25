using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	class HitFlag
	{
		static HitFlag()
		{
			s_default = new HitFlag(HitFlagCombo.DontCare, true, true, true, true, false);
		}

		public HitFlag(HitFlagCombo combo, Boolean high, Boolean low, Boolean air, Boolean falling, Boolean down)
		{
			m_high = high;
			m_low = low;
			m_air = air;
			m_falling = falling;
			m_down = down;
			m_combo = combo;
		}

		public static HitFlag Default
		{
			get { return s_default; }
		}

		public Boolean HitHigh
		{
			get { return m_high; }
		}

		public Boolean HitLow
		{
			get { return m_low; }
		}

		public Boolean HitFalling
		{
			get { return m_falling; }
		}

		public Boolean HitAir
		{
			get { return m_air; }
		}

		public Boolean HitDown
		{
			get { return m_down; }
		}

		public HitFlagCombo ComboFlag
		{
			get { return m_combo; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static HitFlag s_default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_high;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_low;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_air;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_falling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_down;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitFlagCombo m_combo;

		#endregion
	}
}