using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("InGuardDist")]
	class InGuardDist : Function
	{
		public InGuardDist(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
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

		Boolean InGuardDistance(Combat.HitDefinition hitdef, Combat.Entity attacker, Combat.Character target)
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
