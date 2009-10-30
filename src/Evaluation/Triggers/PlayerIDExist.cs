using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PlayerIDExist")]
	static class PlayerIDExist
	{
		public static Number Evaluate(Object state, Number id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character c = entity as Combat.Character;
				if (c == null) continue;

				if (c.Id == id.IntValue) return new Number(true);
			}

			return new Number(false);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
