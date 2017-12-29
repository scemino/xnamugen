namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FVar")]
	internal static class FVar
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
			if (character.Variables.GetFloat(value, false, out result)) return result;

			error = true;
			return 0;
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
