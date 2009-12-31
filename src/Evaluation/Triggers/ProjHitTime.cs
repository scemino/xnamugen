using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjHitTime")]
	static class ProjHitTime
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 projectile_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;
			if (projinfo.Type == ProjectileDataType.Hit && (projectile_id <= 0 || projectile_id == projinfo.ProjectileId))
			{
				return projinfo.Time;
			}

			return -1;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
