using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("{Power} {Type}")]
	internal class HitPriority: IEquatable<HitPriority>
	{
		static HitPriority()
		{
			s_default = new HitPriority(PriorityType.Hit, 4);
		}

		public HitPriority(PriorityType type, int power)
		{
			m_type = type;
			m_power = power;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null)) return false;
			if (obj.GetType() != GetType()) return false;

			return this == (HitPriority)obj;
		}

		public bool Equals(HitPriority other)
		{
			return this == other;
		}

		public override int GetHashCode()

		{
			return Power.GetHashCode() ^ Type.GetHashCode();
		}

		public static bool operator ==(HitPriority lhs, HitPriority rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			if (ReferenceEquals(rhs, null)) return false;

			return lhs.Type == rhs.Type && lhs.Power == rhs.Power;
		}

		public static bool operator !=(HitPriority lhs, HitPriority rhs)
		{
			return !(lhs == rhs);
		}

		public static HitPriority Default => s_default;

		public int Power => m_power;

		public PriorityType Type => m_type;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly HitPriority s_default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PriorityType m_type;

		#endregion
	}
}