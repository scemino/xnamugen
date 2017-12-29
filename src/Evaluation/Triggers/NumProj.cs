namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProj")]
	internal static class NumProj
	{
		public static int Evaluate(object state, ref bool error)
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
				var projectile = character.FilterEntityAsProjectile(entity, int.MinValue);
				if (projectile == null) continue;

				++count;
			}

			return count;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
	
