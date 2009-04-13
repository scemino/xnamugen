using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("StateNo")]
	class StateNo : Function
	{
		public StateNo(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
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
