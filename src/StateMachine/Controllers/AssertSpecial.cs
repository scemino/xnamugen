using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AssertSpecial")]
	internal class AssertSpecial : StateController
	{
		public AssertSpecial(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_assert1 = textsection.GetAttribute("flag", Assertion.None);
			m_assert2 = textsection.GetAttribute("flag2", Assertion.None);
			m_assert3 = textsection.GetAttribute("flag3", Assertion.None);
		}

		public override void Run(Combat.Character character)
		{
			if (HasAssert(Assertion.NoAutoturn))
			{
				character.Assertions.NoAutoTurn = true;
			}

			if (HasAssert(Assertion.NoJuggleCheck))
			{
				character.Assertions.NoJuggleCheck = true;
			}

			if (HasAssert(Assertion.NoKOSound))
			{
				character.Engine.Assertions.NoKOSound = true;
			}

			if (HasAssert(Assertion.NoKOSlow))
			{
				character.Engine.Assertions.NoKOSlow = true;
			}

			if (HasAssert(Assertion.NoShadow))
			{
				character.Assertions.NoShadow = true;
			}

			if (HasAssert(Assertion.NoKOSlow))
			{
				character.Engine.Assertions.NoGlobalShadow = true;
			}

			if (HasAssert(Assertion.NoMusic))
			{
				character.Engine.Assertions.NoMusic = true;
			}

			if (HasAssert(Assertion.TimerFreeze))
			{
				character.Engine.Assertions.TimerFreeze = true;
			}

			if (HasAssert(Assertion.Unguardable))
			{
				character.Assertions.UnGuardable = true;
			}

			if (HasAssert(Assertion.Invisible))
			{
				character.Assertions.Invisible = true;
			}

			if (HasAssert(Assertion.NoWalk))
			{
				character.Assertions.NoWalk = true;
			}

			if (HasAssert(Assertion.NoStandGuard))
			{
				character.Assertions.NoStandingGuard = true;
			}

			if (HasAssert(Assertion.NoCrouchGuard))
			{
				character.Assertions.NoCrouchingGuard = true;
			}

			if (HasAssert(Assertion.NoAirGuard))
			{
				character.Assertions.NoAirGuard = true;
			}

			if (HasAssert(Assertion.Intro))
			{
				character.Engine.Assertions.Intro = true;
			}

			if (HasAssert(Assertion.NoBarDisplay))
			{
				character.Engine.Assertions.NoBarDisplay = true;
			}

			if (HasAssert(Assertion.RoundNotOver))
			{
				character.Engine.Assertions.WinPose = true;
			}

			if (HasAssert(Assertion.NoForeground))
			{
				character.Engine.Assertions.NoFrontLayer = true;
			}

			if (HasAssert(Assertion.NoBackground))
			{
				character.Engine.Assertions.NoBackLayer = true;
			}

			if (HasAssert(Assertion.NoKO))
			{
				character.Assertions.NoKO = true;
			}
		}

		private bool HasAssert(Assertion assert)
		{
			if (assert == Assertion.None) throw new ArgumentOutOfRangeException(nameof(assert));

			return Assert1 == assert || Assert2 == assert || Assert3 == assert;
		}

		public Assertion Assert1 => m_assert1;

		public Assertion Assert2 => m_assert2;

		public Assertion Assert3 => m_assert3;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Assertion m_assert1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Assertion m_assert2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Assertion m_assert3;

		#endregion
	}
}