namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Var")]
	internal static class Var
	{
		public static int Evaluate(object state, ref bool error, int value)
		{
			var character = state as Combat.Character;
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
