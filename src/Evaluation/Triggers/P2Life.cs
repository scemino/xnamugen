using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2Life")]
	static class P2Life
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null) return new Number();

			return new Number(opponent.Life);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
