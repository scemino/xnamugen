using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHomeTeam")]
	static class IsHomeTeam
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
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
