using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProjID")]
	static class NumProjID
	{
		public static Number Evaluate(Object state, Number r1)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32? projectile_id = r1.IntValue > 0 ? r1.IntValue : (Int32?)null;

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Projectile projectile = character.FilterEntityAsProjectile(entity, projectile_id);
				if (projectile == null) continue;

				++count;
			}

			return new Number(count);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
