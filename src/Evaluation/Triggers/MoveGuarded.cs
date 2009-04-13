using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveGuarded")]
	class MoveGuarded : Function
	{
		public MoveGuarded(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (character.MoveType == xnaMugen.MoveType.Attack)
			{
				return new Number(character.OffensiveInfo.MoveGuarded);
			}
			else
			{
				return new Number(0);
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
