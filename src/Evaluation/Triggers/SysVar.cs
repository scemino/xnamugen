using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SysVar")]
	internal static class SysVar
	{
		public static int Evaluate(Character character, ref bool error, int value)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			int result;
			if (character.Variables.GetInteger(value, true, out result)) return result;

			error = true;
			return 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
