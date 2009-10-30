using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjContactTime")]
	static class ProjContactTime
	{
		public static Number Evaluate(Object state, Number projectile_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (projectile_id.NumberType != NumberType.Int) return new Number();

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;
			if ((projinfo.Type == ProjectileDataType.Hit || projinfo.Type == ProjectileDataType.Guarded) && (projectile_id.IntValue <= 0 || projectile_id.IntValue == projinfo.ProjectileId))
			{
				return new Number(projinfo.Time);
			}

			return new Number(-1);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
