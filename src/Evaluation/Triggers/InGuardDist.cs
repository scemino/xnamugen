using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("InGuardDist")]
	internal static class InGuardDist
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			foreach (var entity in character.Engine.Entities)
			{
				var opp = character.FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (opp != null && opp.OffensiveInfo.ActiveHitDef && InGuardDistance(opp.OffensiveInfo.HitDef, opp, character))
				{
					return true;
				}

				var projectile = entity as Combat.Projectile;
				if (projectile != null && projectile.Team != character.Team && InGuardDistance(projectile.Data.HitDef, projectile, character))
				{
					return true;
				}
			}

			return false;
		}

		private static bool InGuardDistance(Combat.HitDefinition hitdef, Combat.Entity attacker, Combat.Character target)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			var distance = Math.Abs(attacker.CurrentLocation.X - target.CurrentLocation.X);

			return distance <= hitdef.GuardDistance;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
