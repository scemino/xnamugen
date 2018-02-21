using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FVar")]
	internal static class FVar
	{
		public static float Evaluate(Character character, ref bool error, int value)
		{
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
