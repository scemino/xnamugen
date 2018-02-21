using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PlayerIDExist")]
	internal static class PlayerIDExist
	{
		public static bool Evaluate(Character character, ref bool error, int id)
		{
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
