using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("StateNo")]
	static class StateNo
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			StateMachine.State prevstate = character.StateManager.CurrentState;
			return (prevstate != null) ? new Number(prevstate.Number) : new Number(0);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
