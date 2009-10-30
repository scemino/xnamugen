using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("InGuardDist")]
	static class InGuardDist
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character opp = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (opp != null && opp.OffensiveInfo.ActiveHitDef == true && InGuardDistance(opp.OffensiveInfo.HitDef, opp, character) == true)
				{
					return new Number(true);
				}

				Combat.Projectile projectile = entity as Combat.Projectile;
				if (projectile != null && projectile.Team != character.Team && InGuardDistance(projectile.Data.HitDef, projectile, character) == true)
				{
					return new Number(true);
				}
			}

			return new Number(false);
		}

		static Boolean InGuardDistance(Combat.HitDefinition hitdef, Combat.Entity attacker, Combat.Character target)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			Single distance = Math.Abs(attacker.CurrentLocation.X - target.CurrentLocation.X);

			return distance <= hitdef.GuardDistance;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
