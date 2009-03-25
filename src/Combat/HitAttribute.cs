using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	class HitAttribute
	{
		static HitAttribute()
		{
			s_default = new HitAttribute(AttackStateType.None, new ReadOnlyList<HitType>());
		}

		public HitAttribute(AttackStateType height, ReadOnlyList<HitType> attackdata)
		{
			if (attackdata == null) throw new ArgumentNullException("attackdata");

			m_attackheight = height;
			m_attackdata = attackdata;
		}

		public Boolean HasHeight(AttackStateType height)
		{
			if (height == AttackStateType.None) return false;

			return (AttackHeight & height) == height;
		}

		public Boolean HasData(HitType hittype)
		{
			if (hittype.Class == AttackClass.None || hittype.Power == AttackPower.None) return false;

			foreach (HitType type in AttackData)
			{
				if (HitType.Match(hittype, type) == true) return true;
			}

			return false;
		}

		public ReadOnlyList<HitType> AttackData
		{
			get { return m_attackdata; }
		}

		public AttackStateType AttackHeight
		{
			get { return m_attackheight; }
		}

		public static HitAttribute Default
		{
			get { return s_default; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static HitAttribute s_default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AttackStateType m_attackheight;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<HitType> m_attackdata;

		#endregion
	}
}