using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumEnemy")]
	static class NumEnemy
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character enemy = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (enemy == null) continue;

				Combat.Player enemyplayer = enemy as Combat.Player;
				if (enemyplayer == null) continue;

				++count;
			}

			return new Number(count);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
