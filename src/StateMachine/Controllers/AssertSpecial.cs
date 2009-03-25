using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AssertSpecial")]
	class AssertSpecial : StateController
	{
		public AssertSpecial(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_assert1 = textsection.GetAttribute<Assertion>("flag", Assertion.None);
			m_assert2 = textsection.GetAttribute<Assertion>("flag2", Assertion.None);
			m_assert3 = textsection.GetAttribute<Assertion>("flag3", Assertion.None);
		}

		public override void Run(Combat.Character character)
		{
			if (HasAssert(Assertion.NoAutoturn) == true)
			{
				character.Assertions.NoAutoTurn = true;
			}

			if (HasAssert(Assertion.NoJuggleCheck) == true)
			{
				character.Assertions.NoJuggleCheck = true;
			}

			if (HasAssert(Assertion.NoKOSound) == true)
			{
				character.Engine.Assertions.NoKOSound = true;
			}

			if (HasAssert(Assertion.NoKOSlow) == true)
			{
				character.Engine.Assertions.NoKOSlow = true;
			}

			if (HasAssert(Assertion.NoShadow) == true)
			{
				character.Assertions.NoShadow = true;
			}

			if (HasAssert(Assertion.NoKOSlow) == true)
			{
				character.Engine.Assertions.NoGlobalShadow = true;
			}

			if (HasAssert(Assertion.NoMusic) == true)
			{
				character.Engine.Assertions.NoMusic = true;
			}

			if (HasAssert(Assertion.TimerFreeze) == true)
			{
				character.Engine.Assertions.TimerFreeze = true;
			}

			if (HasAssert(Assertion.Unguardable) == true)
			{
				character.Assertions.UnGuardable = true;
			}

			if (HasAssert(Assertion.Invisible) == true)
			{
				character.Assertions.Invisible = true;
			}

			if (HasAssert(Assertion.NoWalk) == true)
			{
				character.Assertions.NoWalk = true;
			}

			if (HasAssert(Assertion.NoStandGuard) == true)
			{
				character.Assertions.NoStandingGuard = true;
			}

			if (HasAssert(Assertion.NoCrouchGuard) == true)
			{
				character.Assertions.NoCrouchingGuard = true;
			}

			if (HasAssert(Assertion.NoAirGuard) == true)
			{
				character.Assertions.NoAirGuard = true;
			}

			if (HasAssert(Assertion.Intro) == true)
			{
				character.Engine.Assertions.Intro = true;
			}

			if (HasAssert(Assertion.NoBarDisplay) == true)
			{
				character.Engine.Assertions.NoBarDisplay = true;
			}

			if (HasAssert(Assertion.RoundNotOver) == true)
			{
				character.Engine.Assertions.WinPose = true;
			}

			if (HasAssert(Assertion.NoForeground) == true)
			{
				character.Engine.Assertions.NoFrontLayer = true;
			}

			if (HasAssert(Assertion.NoBackground) == true)
			{
				character.Engine.Assertions.NoBackLayer = true;
			}

			if (HasAssert(Assertion.NoKO) == true)
			{
				character.Assertions.NoKO = true;
			}
		}

		Boolean HasAssert(Assertion assert)
		{
			if (assert == Assertion.None) throw new ArgumentOutOfRangeException("assert");

			return Assert1 == assert || Assert2 == assert || Assert3 == assert;
		}

		public Assertion Assert1
		{
			get { return m_assert1; }
		}

		public Assertion Assert2
		{
			get { return m_assert2; }
		}

		public Assertion Assert3
		{
			get { return m_assert3; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Assertion m_assert1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Assertion m_assert2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Assertion m_assert3;

		#endregion
	}
}