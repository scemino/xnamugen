using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal struct Win
	{
		public Win(Victory victory, bool perfect)
		{
			m_victory = victory;
			m_isperfect = perfect;
		}

		public Victory Victory => m_victory;

		public bool IsPerfectVictory => m_isperfect;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Victory m_victory;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_isperfect;

		#endregion
	}
}