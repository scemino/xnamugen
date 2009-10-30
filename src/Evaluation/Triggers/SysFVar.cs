using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SysFVar")]
	static class SysFVar
	{
		public static Number Evaluate(Object state, Number r1)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (r1.NumberType == NumberType.None) return new Number();

			Single result;
			if (character.Variables.GetFloat(r1.IntValue, true, out result) == true)
			{
				return new Number(result);
			}
			else
			{
				return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
