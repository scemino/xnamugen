using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProj")]
	static class NumProj
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
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Projectile projectile = character.FilterEntityAsProjectile(entity, Int32.MinValue);
				if (projectile == null) continue;

				++count;
			}

			return count;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
	
