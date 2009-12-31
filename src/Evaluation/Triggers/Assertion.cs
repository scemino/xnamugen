using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Assertion")]
	static class Assertion
	{
		public static Boolean Evaluate(Object state, ref Boolean error, xnaMugen.Assertion assertion)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			switch (assertion)
			{
				case xnaMugen.Assertion.Intro:
					return character.Engine.Assertions.Intro;

				case xnaMugen.Assertion.Invisible:
					return character.Assertions.Invisible;

				case xnaMugen.Assertion.RoundNotOver:
					return character.Engine.Assertions.WinPose;

				case xnaMugen.Assertion.NoBarDisplay:
					return character.Engine.Assertions.NoBarDisplay;

				case xnaMugen.Assertion.NoBackground:
					return character.Engine.Assertions.NoBackLayer;

				case xnaMugen.Assertion.NoForeground:
					return character.Engine.Assertions.NoFrontLayer;

				case xnaMugen.Assertion.NoStandGuard:
					return character.Assertions.NoStandingGuard;

				case xnaMugen.Assertion.NoAirGuard:
					return character.Assertions.NoAirGuard;

				case xnaMugen.Assertion.NoCrouchGuard:
					return character.Assertions.NoCrouchingGuard;

				case xnaMugen.Assertion.NoAutoturn:
					return character.Assertions.NoAutoTurn;

				case xnaMugen.Assertion.NoJuggleCheck:
					return character.Assertions.NoJuggleCheck;

				case xnaMugen.Assertion.NoKOSound:
					return character.Engine.Assertions.NoKOSound;

				case xnaMugen.Assertion.NoKOSlow:
					return character.Engine.Assertions.NoKOSlow;

				case xnaMugen.Assertion.NoShadow:
					return character.Assertions.NoShadow;

				case xnaMugen.Assertion.GlobalNoShadow:
					return character.Engine.Assertions.NoGlobalShadow;

				case xnaMugen.Assertion.NoMusic:
					return character.Engine.Assertions.NoMusic;

				case xnaMugen.Assertion.NoWalk:
					return character.Assertions.NoWalk;

				case xnaMugen.Assertion.TimerFreeze:
					return character.Engine.Assertions.TimerFreeze;

				case xnaMugen.Assertion.Unguardable:
					return character.Assertions.UnGuardable;

				case xnaMugen.Assertion.None:
				default:
					error = true;
					return false;
			}
		}

		public static Node Parse(ParseState state)
		{
			if (state.CurrentSymbol != Symbol.LeftParen) return null;
			++state.TokenIndex;

			if (state.CurrentUnknown == null) return null;
			xnaMugen.Assertion assert = state.ConvertCurrentToken<xnaMugen.Assertion>();

			state.BaseNode.Arguments.Add(assert);
			++state.TokenIndex;

			if (state.CurrentSymbol != Symbol.RightParen) return null;
			++state.TokenIndex;

			return state.BaseNode;
		}
	}
}