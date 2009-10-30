using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FVar")]
	static class FVar
	{
		public static Number Evaluate(Object state, Number value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Single result;
			if (character.Variables.GetFloat(value.IntValue, false, out result) == true)
			{
				return new Number(result);
			}
			else
			{
				return new Number();
			}
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
