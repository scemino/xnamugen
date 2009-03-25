using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("{Class} {Power}")]
	struct HitType
	{
		public HitType(AttackClass aclass, AttackPower apower)
		{
			m_class = aclass;
			m_power = apower;
		}

		public static Boolean Match(HitType lhs, HitType rhs)
		{
			if (lhs.Class == AttackClass.None || lhs.Power == AttackPower.None) return false;
			if (rhs.Class == AttackClass.None || rhs.Power == AttackPower.None) return false;

			if (lhs.Class == rhs.Class && lhs.Power == rhs.Power) return true;
			if ((lhs.Class == AttackClass.All || rhs.Class == AttackClass.All) && lhs.Power == rhs.Power) return true;
			if (lhs.Class == rhs.Class && (lhs.Power == AttackPower.All || rhs.Power == AttackPower.All)) return true;

			return false;
		}

		public AttackClass Class
		{
			get { return m_class; }
		}

		public AttackPower Power
		{
			get { return m_power; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AttackClass m_class;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AttackPower m_power;

		#endregion
	}
}