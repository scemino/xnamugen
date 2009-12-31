using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProjID")]
	static class NumProjID
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 projectile_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Projectile projectile = character.FilterEntityAsProjectile(entity, projectile_id);
				if (projectile == null) continue;

				++count;
			}

			return count;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
