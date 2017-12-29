namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProjID")]
	internal static class NumProjID
	{
		public static int Evaluate(object state, ref bool error, int projectileId)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var count = 0;
			foreach (var entity in character.Engine.Entities)
			{
				var projectile = character.FilterEntityAsProjectile(entity, projectileId);
				if (projectile == null) continue;

				++count;
			}

			return count;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
