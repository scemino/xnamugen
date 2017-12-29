namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SysFVar")]
	internal static class SysFVar
	{
		public static float Evaluate(object state, ref bool error, int value)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			float result;
			if (character.Variables.GetFloat(value, true, out result)) return result;

			error = true;
			return 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
