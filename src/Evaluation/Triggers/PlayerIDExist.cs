namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PlayerIDExist")]
	internal static class PlayerIDExist
	{
		public static bool Evaluate(object state, ref bool error, int id)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			foreach (var entity in character.Engine.Entities)
			{
				var c = entity as Combat.Character;
				if (c == null) continue;

				if (c.Id == id) return true;
			}

			return false;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
