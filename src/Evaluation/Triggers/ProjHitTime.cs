using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjHitTime")]
	internal static class ProjHitTime
	{
		public static int Evaluate(Character character, ref bool error, int projectileId)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var projinfo = character.OffensiveInfo.ProjectileInfo;
			if (projinfo.Type == ProjectileDataType.Hit && (projectileId <= 0 || projectileId == projinfo.ProjectileId))
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
