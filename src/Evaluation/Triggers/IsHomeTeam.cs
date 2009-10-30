using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHomeTeam")]
	static class IsHomeTeam
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Team.Side == xnaMugen.TeamSide.Left);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
