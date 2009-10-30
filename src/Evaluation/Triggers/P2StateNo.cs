using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2StateNo")]
	static class P2StateNo
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null) return new Number();

			return new Number(opponent.StateManager.CurrentState.Number);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
