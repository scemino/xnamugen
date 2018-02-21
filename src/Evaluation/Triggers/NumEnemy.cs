using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumEnemy")]
	internal static class NumEnemy
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var count = 0;
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
