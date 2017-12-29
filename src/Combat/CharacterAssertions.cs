using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class CharacterAssertions
	{
		public CharacterAssertions()
		{
			Reset();
		}

		public void Reset()
		{
			m_invisible = false;
			m_nostandingguard = false;
			m_nocrouchingguard = false;
			m_noairguard = false;
			m_nowalk = false;
			m_noautoturn = false;
			m_nojugglecheck = false;
			m_noshadow = false;
			m_unguardable = false;
			m_noko = false;
		}

		public bool Invisible
		{
			get => m_invisible;

			set { m_invisible = value; }
		}

		public bool NoStandingGuard
		{
			get => m_nostandingguard;

			set { m_nostandingguard = value; }
		}

		public bool NoCrouchingGuard
		{
			get => m_nocrouchingguard;

			set { m_nocrouchingguard = value; }
		}

		public bool NoAirGuard
		{
			get => m_noairguard;

			set { m_noairguard = value; }
		}

		public bool NoWalk
		{
			get => m_nowalk;

			set { m_nowalk = value; }
		}

		public bool NoAutoTurn
		{
			get => m_noautoturn;

			set { m_noautoturn = value; }
		}

		public bool NoJuggleCheck
		{
			get => m_nojugglecheck;

			set { m_nojugglecheck = value; }
		}

		public bool NoShadow
		{
			get => m_noshadow;

			set { m_noshadow = value; }
		}

		public bool UnGuardable
		{
			get => m_unguardable;

			set { m_unguardable = value; }
		}

		public bool NoKO
		{
			get => m_noko;

			set { m_noko = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_invisible;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nostandingguard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nocrouchingguard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_noairguard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nowalk;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_noautoturn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nojugglecheck;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_noshadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_unguardable;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_noko;

		#endregion
	}
}