using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TeamSide")]
	static class TeamSide
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (character.Team.Side)
			{
				case xnaMugen.TeamSide.Left:
					return new Number(1);

				case xnaMugen.TeamSide.Right:
					return new Number(2);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
