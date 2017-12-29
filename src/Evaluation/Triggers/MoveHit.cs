namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveHit")]
	internal static class MoveHit
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.MoveType == xnaMugen.MoveType.Attack ? character.OffensiveInfo.MoveHit : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
