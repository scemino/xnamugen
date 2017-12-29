using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class HitBy
	{
		public HitBy()
		{
			m_attribute = HitAttribute.Default;
			m_time = 0;
			m_negation = false;
			m_isactive = false;
		}

		public void Reset()
		{
			m_attribute = HitAttribute.Default;
			m_time = 0;
			m_negation = false;
			m_isactive = false;
		}

		public void Update()
		{
			if (IsActive && Time > 0)
			{
				--m_time;
			}
			else
			{
				Reset();
			}
		}

		public void Set(HitAttribute attribute, int time, bool negation)
		{
			if (attribute == null) throw new ArgumentNullException(nameof(attribute));

			m_attribute = attribute;
			m_time = time;
			m_negation = negation;
			m_isactive = true;
		}

		public bool CanHit(HitAttribute attr)
		{
			if (attr == null) throw new ArgumentNullException(nameof(attr));

			if (IsActive == false) return true;

			if (m_negation == false)
			{
				if (m_attribute.HasHeight(attr.AttackHeight) == false) return false;
				foreach (var hittype in attr.AttackData) if (m_attribute.HasData(hittype) == false) return false;

				return true;
			}

			if (m_attribute.HasHeight(attr.AttackHeight)) return false;
			foreach (var hittype in attr.AttackData) if (m_attribute.HasData(hittype)) return false;

			return true;
		}

		public bool IsActive => m_isactive;

		public HitAttribute Attribute => m_attribute;

		public int Time => m_time;

		public bool IsNegation => m_negation;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private HitAttribute m_attribute;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_negation;

		#endregion
	}
}