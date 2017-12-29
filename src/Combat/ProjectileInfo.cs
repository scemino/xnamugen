using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class ProjectileInfo
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

		public void Set(int projId, ProjectileDataType datatype)
		{
			m_projectileid = projId;
			m_datatype = datatype;
			m_time = 0;
		}

		public void Update()
		{
			if (Type != ProjectileDataType.None) ++m_time;
		}

		public int ProjectileId => m_projectileid;

		public ProjectileDataType Type => m_datatype;

		public int Time => m_time;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_projectileid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ProjectileDataType m_datatype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		#endregion
	}
}