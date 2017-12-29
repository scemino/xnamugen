namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitCount")]
	internal static class HitCount
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.OffensiveInfo.HitCount;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
