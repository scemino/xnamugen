using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class HitOverride
	{
		public HitOverride()
		{
			m_attr = HitAttribute.Default;
			m_statenumber = int.MinValue;
			m_time = 0;
			m_forceair = false;
			m_isactive = false;
		}

		public void Reset()
		{
			m_attr = HitAttribute.Default;
			m_statenumber = int.MinValue;
			m_time = 0;
			m_forceair = false;
			m_isactive = false;
		}

		public void Set(HitAttribute attribute, int statenumber, int time, bool forceair)
		{
			if (attribute == null) throw new ArgumentNullException(nameof(attribute));

			m_attr = attribute;
			m_statenumber = statenumber;
			m_time = time;
			m_forceair = forceair;
			m_isactive = true;
		}

		public void Update()
		{
			if (IsActive && (Time == -1 || Time > 0))
			{
				--m_time;
			}
			else
			{
				Reset();
			}
		}

		public bool IsActive => m_isactive;

		public HitAttribute Attribute => m_attr;

		public int StateNumber => m_statenumber;

		public int Time => m_time;

		public bool ForceAir => m_forceair;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private HitAttribute m_attr;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_forceair;

		#endregion
	}
}