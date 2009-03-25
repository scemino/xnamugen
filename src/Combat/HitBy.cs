using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	class HitBy
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
			if (IsActive == true && Time > 0)
			{
				--m_time;
			}
			else
			{
				Reset();
			}
		}

		public void Set(HitAttribute attribute, Int32 time, Boolean negation)
		{
			if (attribute == null) throw new ArgumentNullException("attribute");

			m_attribute = attribute;
			m_time = time;
			m_negation = negation;
			m_isactive = true;
		}

		public Boolean CanHit(HitAttribute attr)
		{
			if (attr == null) throw new ArgumentNullException("attr");

			if (IsActive == false) return true;

			if (m_negation == false)
			{
				if (m_attribute.HasHeight(attr.AttackHeight) == false) return false;
				foreach (HitType hittype in attr.AttackData) if (m_attribute.HasData(hittype) == false) return false;

				return true;
			}
			else
			{
				if (m_attribute.HasHeight(attr.AttackHeight) == true) return false;
				foreach (HitType hittype in attr.AttackData) if (m_attribute.HasData(hittype) == true) return false;

				return true;
			}
		}

		public Boolean IsActive
		{
			get { return m_isactive; }
		}

		public HitAttribute Attribute
		{
			get { return m_attribute; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		public Boolean IsNegation
		{
			get { return m_negation; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		HitAttribute m_attribute;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_negation;

		#endregion
	}
}