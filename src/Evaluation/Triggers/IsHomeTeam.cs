using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHomeTeam")]
	internal static class IsHomeTeam
	{
		public static bool Evaluate(Character character, ref bool error)
		{
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
