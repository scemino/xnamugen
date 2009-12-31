using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2StateNo")]
	static class P2StateNo
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null)
			{
				error = true;
				return 0;
			}

			StateMachine.State currentstate = opponent.StateManager.CurrentState;
			return (currentstate != null) ? currentstate.Number : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
