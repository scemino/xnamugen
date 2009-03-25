using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	class CharacterAssertions
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

		public Boolean Invisible
		{
			get { return m_invisible; }

			set { m_invisible = value; }
		}

		public Boolean NoStandingGuard
		{
			get { return m_nostandingguard; }

			set { m_nostandingguard = value; }
		}

		public Boolean NoCrouchingGuard
		{
			get { return m_nocrouchingguard; }

			set { m_nocrouchingguard = value; }
		}

		public Boolean NoAirGuard
		{
			get { return m_noairguard; }

			set { m_noairguard = value; }
		}

		public Boolean NoWalk
		{
			get { return m_nowalk; }

			set { m_nowalk = value; }
		}

		public Boolean NoAutoTurn
		{
			get { return m_noautoturn; }

			set { m_noautoturn = value; }
		}

		public Boolean NoJuggleCheck
		{
			get { return m_nojugglecheck; }

			set { m_nojugglecheck = value; }
		}

		public Boolean NoShadow
		{
			get { return m_noshadow; }

			set { m_noshadow = value; }
		}

		public Boolean UnGuardable
		{
			get { return m_unguardable; }

			set { m_unguardable = value; }
		}

		public Boolean NoKO
		{
			get { return m_noko; }

			set { m_noko = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_invisible;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nostandingguard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nocrouchingguard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_noairguard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nowalk;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_noautoturn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nojugglecheck;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_noshadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_unguardable;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_noko;

		#endregion
	}
}