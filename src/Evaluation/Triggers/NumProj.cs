using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProj")]
	static class NumProj
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Projectile projectile = character.FilterEntityAsProjectile(entity, null);
				if (projectile == null) continue;

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
	
