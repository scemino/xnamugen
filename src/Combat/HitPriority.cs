using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("{Power} {Type}")]
	class HitPriority: IEquatable<HitPriority>
	{
		static HitPriority()
		{
			s_default = new HitPriority(PriorityType.Hit, 4);
		}

		public HitPriority(PriorityType type, Int32 power)
		{
			m_type = type;
			m_power = power;
		}

		public override Boolean Equals(Object obj)
		{
			if (Object.ReferenceEquals(obj, null) == true) return false;
			if (obj.GetType() != GetType()) return false;

			return this == (HitPriority)obj;
		}

		public Boolean Equals(HitPriority other)
		{
			return this == other;
		}

		public override Int32 GetHashCode()

		{
			return Power.GetHashCode() ^ Type.GetHashCode();
		}

		public static Boolean operator ==(HitPriority lhs, HitPriority rhs)
		{
			if (Object.ReferenceEquals(lhs, rhs) == true) return true;
			if (Object.ReferenceEquals(lhs, null) == true) return false;
			if (Object.ReferenceEquals(rhs, null) == true) return false;

			return lhs.Type == rhs.Type && lhs.Power == rhs.Power;
		}

		public static Boolean operator !=(HitPriority lhs, HitPriority rhs)
		{
			return !(lhs == rhs);
		}

		public static HitPriority Default
		{
			get { return s_default; }
		}

		public Int32 Power
		{
			get { return m_power; }
		}

		public PriorityType Type
		{
			get { return m_type; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static HitPriority s_default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PriorityType m_type;

		#endregion
	}
}