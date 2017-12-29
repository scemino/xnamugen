using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	internal class HitAttribute
	{
		static HitAttribute()
		{
			s_default = new HitAttribute(AttackStateType.None, new ReadOnlyList<HitType>());
		}

		public HitAttribute(AttackStateType height, ReadOnlyList<HitType> attackdata)
		{
			if (attackdata == null) throw new ArgumentNullException(nameof(attackdata));

			m_attackheight = height;
			m_attackdata = attackdata;
		}

		public bool HasHeight(AttackStateType height)
		{
			if (height == AttackStateType.None) return false;

			return (AttackHeight & height) == height;
		}

		public bool HasData(HitType hittype)
		{
			if (hittype.Class == AttackClass.None || hittype.Power == AttackPower.None) return false;

			foreach (var type in AttackData)
			{
				if (HitType.Match(hittype, type)) return true;
			}

			return false;
		}

		public ReadOnlyList<HitType> AttackData => m_attackdata;

		public AttackStateType AttackHeight => m_attackheight;

		public static HitAttribute Default => s_default;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly HitAttribute s_default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AttackStateType m_attackheight;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<HitType> m_attackdata;

		#endregion
	}
}