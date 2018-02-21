using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProjID")]
	internal static class NumProjID
	{
		public static int Evaluate(Character character, ref bool error, int projectileId)
		{
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
