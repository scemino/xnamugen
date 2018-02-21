using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjContactTime")]
	internal static class ProjContactTime
	{
		public static int Evaluate(Character character, ref bool error, int projectileId)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var projinfo = character.OffensiveInfo.ProjectileInfo;
			if ((projinfo.Type == ProjectileDataType.Hit || projinfo.Type == ProjectileDataType.Guarded) && (projectileId <= 0 || projectileId == projinfo.ProjectileId))
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
