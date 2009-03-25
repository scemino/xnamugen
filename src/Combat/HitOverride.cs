using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	class HitOverride
	{
		public HitOverride()
		{
			m_attr = HitAttribute.Default;
			m_statenumber = Int32.MinValue;
			m_time = 0;
			m_forceair = false;
			m_isactive = false;
		}

		public void Reset()
		{
			m_attr = HitAttribute.Default;
			m_statenumber = Int32.MinValue;
			m_time = 0;
			m_forceair = false;
			m_isactive = false;
		}

		public void Set(HitAttribute attribute, Int32 statenumber, Int32 time, Boolean forceair)
		{
			if (attribute == null) throw new ArgumentNullException("attribute");

			m_attr = attribute;
			m_statenumber = statenumber;
			m_time = time;
			m_forceair = forceair;
			m_isactive = true;
		}

		public void Update()
		{
			if (IsActive == true && (Time == -1 || Time > 0))
			{
				--m_time;
			}
			else
			{
				Reset();
			}
		}

		public Boolean IsActive
		{
			get { return m_isactive; }
		}

		public HitAttribute Attribute
		{
			get { return m_attr; }
		}

		public Int32 StateNumber
		{
			get { return m_statenumber; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		public Boolean ForceAir
		{
			get { return m_forceair; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		HitAttribute m_attr;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_forceair;

		#endregion
	}
}