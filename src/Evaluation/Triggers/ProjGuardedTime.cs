namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjGuardedTime")]
	internal static class ProjGuardedTime
	{
		public static int Evaluate(object state, ref bool error, int projectileId)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var projinfo = character.OffensiveInfo.ProjectileInfo;
			if (projinfo.Type == ProjectileDataType.Guarded && (projectileId <= 0 || projectileId == projinfo.ProjectileId))
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
