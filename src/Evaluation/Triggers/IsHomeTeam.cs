namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHomeTeam")]
	internal static class IsHomeTeam
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.Side == xnaMugen.TeamSide.Left;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
