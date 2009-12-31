using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("StateNo")]
	static class StateNo
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			StateMachine.State currentstate = character.StateManager.CurrentState;
			return (currentstate != null) ? currentstate.Number : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
