using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PrevStateNo")]
	static class PrevStateNo
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			StateMachine.State prevstate = character.StateManager.PreviousState;
			return (prevstate != null) ? new Number(prevstate.Number) : new Number(0);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
