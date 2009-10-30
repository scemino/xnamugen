using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SelfAnimExist")]
	static class SelfAnimExist
	{
		public static Number Evaluate(Object state, Number animationnumber)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (animationnumber.NumberType != NumberType.Int) return new Number();

			return new Number(character.AnimationManager.HasAnimation(animationnumber.IntValue));
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
