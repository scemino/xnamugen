using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TicksPerSecond")]
	internal static class TicksPerSecond
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

#warning Hack
			return 60;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}