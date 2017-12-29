namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumPartner")]
	internal static class NumPartner
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
				var partner = character.BasePlayer.FilterEntityAsPartner(entity);
				if (partner == null) continue;

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
