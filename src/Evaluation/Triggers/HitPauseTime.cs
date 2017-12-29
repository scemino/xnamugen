namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitPauseTime")]
	internal static class HitPauseTime
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.OffensiveInfo.HitPauseTime;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
