namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveReversed")]
	internal static class MoveReversed
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.MoveType == xnaMugen.MoveType.Attack ? character.OffensiveInfo.MoveReversed : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
