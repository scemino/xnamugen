using System.Diagnostics;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("{Class} {Power}")]
	internal struct HitType
	{
		public HitType(AttackClass aclass, AttackPower apower)
		{
			m_class = aclass;
			m_power = apower;
		}

		public static bool Match(HitType lhs, HitType rhs)
		{
			if (lhs.Class == AttackClass.None || lhs.Power == AttackPower.None) return false;
			if (rhs.Class == AttackClass.None || rhs.Power == AttackPower.None) return false;

			if (lhs.Class == rhs.Class && lhs.Power == rhs.Power) return true;
			if ((lhs.Class == AttackClass.All || rhs.Class == AttackClass.All) && lhs.Power == rhs.Power) return true;
			if (lhs.Class == rhs.Class && (lhs.Power == AttackPower.All || rhs.Power == AttackPower.All)) return true;

			return false;
		}

		public AttackClass Class => m_class;

		public AttackPower Power => m_power;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AttackClass m_class;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AttackPower m_power;

		#endregion
	}
}