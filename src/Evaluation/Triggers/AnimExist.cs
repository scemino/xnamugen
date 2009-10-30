using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimExist")]
	static class AnimExist
	{
		public static Number Evaluate(Object state, Number value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (character.AnimationManager.IsForeignAnimation == true) return new Number();

			if (value.NumberType == NumberType.None) return new Number();

			return new Number(character.AnimationManager.HasAnimation(value.IntValue));
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
