using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Assertion")]
	static class Assertion
	{
		public static Number Evaluate(Object state, xnaMugen.Assertion assertion)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (assertion)
			{
				case xnaMugen.Assertion.Intro:
					return new Number(character.Engine.Assertions.Intro);

				case xnaMugen.Assertion.Invisible:
					return new Number(character.Assertions.Invisible);

				case xnaMugen.Assertion.RoundNotOver:
					return new Number(character.Engine.Assertions.WinPose);

				case xnaMugen.Assertion.NoBarDisplay:
					return new Number(character.Engine.Assertions.NoBarDisplay);

				case xnaMugen.Assertion.NoBackground:
					return new Number(character.Engine.Assertions.NoBackLayer);

				case xnaMugen.Assertion.NoForeground:
					return new Number(character.Engine.Assertions.NoFrontLayer);

				case xnaMugen.Assertion.NoStandGuard:
					return new Number(character.Assertions.NoStandingGuard);

				case xnaMugen.Assertion.NoAirGuard:
					return new Number(character.Assertions.NoAirGuard);

				case xnaMugen.Assertion.NoCrouchGuard:
					return new Number(character.Assertions.NoCrouchingGuard);

				case xnaMugen.Assertion.NoAutoturn:
					return new Number(character.Assertions.NoAutoTurn);

				case xnaMugen.Assertion.NoJuggleCheck:
					return new Number(character.Assertions.NoJuggleCheck);

				case xnaMugen.Assertion.NoKOSound:
					return new Number(character.Engine.Assertions.NoKOSound);

				case xnaMugen.Assertion.NoKOSlow:
					return new Number(character.Engine.Assertions.NoKOSlow);

				case xnaMugen.Assertion.NoShadow:
					return new Number(character.Assertions.NoShadow);

				case xnaMugen.Assertion.GlobalNoShadow:
					return new Number(character.Engine.Assertions.NoGlobalShadow);

				case xnaMugen.Assertion.NoMusic:
					return new Number(character.Engine.Assertions.NoMusic);

				case xnaMugen.Assertion.NoWalk:
					return new Number(character.Assertions.NoWalk);

				case xnaMugen.Assertion.TimerFreeze:
					return new Number(character.Engine.Assertions.TimerFreeze);

				case xnaMugen.Assertion.Unguardable:
					return new Number(character.Assertions.UnGuardable);

				case xnaMugen.Assertion.None:
				default:
					return new Number(0);
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