namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("UniqHitCount")]
	internal static class UniqHitCount
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.OffensiveInfo.UniqueHitCount;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}

