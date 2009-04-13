using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Life")]
	class Life : Function
	{
		public Life(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if(character == null) return new Number();

			return new Number(character.Life);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
