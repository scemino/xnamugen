using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveHit")]
	class MoveHit : Function
	{
		public MoveHit(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (character.MoveType == xnaMugen.MoveType.Attack)
			{
				return new Number(character.OffensiveInfo.MoveHit);
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
