using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Var")]
	internal static class Var
	{
		public static int Evaluate(Character character, ref bool error, int value)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			int result;
			if (character.Variables.GetInteger(value, false, out result)) return result;

			error = true;
			return 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
