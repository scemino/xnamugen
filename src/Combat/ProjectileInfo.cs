using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	class ProjectileInfo
	{
		public ProjectileInfo()
		{
			m_projectileid = 0;
			m_datatype = ProjectileDataType.None;
			m_time = 0;
		}

		public void Reset()
		{
			m_projectileid = 0;
			m_datatype = ProjectileDataType.None;
			m_time = 0;
		}

		public void Set(Int32 proj_id, ProjectileDataType datatype)
		{
			m_projectileid = proj_id;
			m_datatype = datatype;
			m_time = 0;
		}

		public void Update()
		{
			if (Type != ProjectileDataType.None) ++m_time;
		}

		public Int32 ProjectileId
		{
			get { return m_projectileid; }
		}

		public ProjectileDataType Type
		{
			get { return m_datatype; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_projectileid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ProjectileDataType m_datatype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

		#endregion
	}
}