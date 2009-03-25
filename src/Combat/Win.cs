using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	struct Win
	{
		public Win(Victory victory, Boolean perfect)
		{
			m_victory = victory;
			m_isperfect = perfect;
		}

		public Victory Victory
		{
			get { return m_victory; }
		}

		public Boolean IsPerfectVictory
		{
			get { return m_isperfect; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Victory m_victory;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_isperfect;

		#endregion
	}
}