using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FVar")]
	static class FVar
	{
		public static Single Evaluate(Object state, ref Boolean error, Int32 value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Single result;
			if (character.Variables.GetFloat(value, false, out result) == true) return result;

			error = true;
			return 0;
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
