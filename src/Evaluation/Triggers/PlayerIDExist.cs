using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PlayerIDExist")]
	static class PlayerIDExist
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character c = entity as Combat.Character;
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
