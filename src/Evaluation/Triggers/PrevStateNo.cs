using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PrevStateNo")]
	static class PrevStateNo
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			StateMachine.State prevstate = character.StateManager.PreviousState;
			return (prevstate != null) ? prevstate.Number : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
