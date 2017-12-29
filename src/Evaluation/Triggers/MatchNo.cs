namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MatchNo")]
	internal static class MatchNo
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

#warning Not really implimented. Just a quick fix.
			return 1;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
