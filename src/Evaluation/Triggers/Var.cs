using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Var")]
	static class Var
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 result;
			if (character.Variables.GetInteger(value, false, out result) == true) return result;

			error = true;
			return 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
