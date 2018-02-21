using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SysFVar")]
	internal static class SysFVar
	{
		public static float Evaluate(Character character, ref bool error, int value)
		{
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
