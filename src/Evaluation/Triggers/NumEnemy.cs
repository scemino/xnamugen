using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumEnemy")]
	static class NumEnemy
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 count = 0;
			Action<Combat.Player> func = player =>
			{
				if (player != null && character.FilterEntityAsCharacter(player, AffectTeam.Enemy) != null) ++count;
			};

			func(character.Engine.Team1.MainPlayer);
			func(character.Engine.Team2.MainPlayer);
			func(character.Engine.Team1.TeamMate);
			func(character.Engine.Team2.TeamMate);

			return count;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
