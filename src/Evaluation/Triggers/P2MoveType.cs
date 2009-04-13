using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2MoveType")]
	class P2MoveType : Function
	{
		public P2MoveType(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 2) return new Number();

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null) return new Number();

			Operator @operator = (Operator)Arguments[0];
			xnaMugen.MoveType movetype = (xnaMugen.MoveType)Arguments[1];

			if (movetype == xnaMugen.MoveType.Unchanged || movetype == xnaMugen.MoveType.None) return new Number();

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(movetype == opponent.MoveType);

				case Operator.NotEquals:
					return new Number(movetype != opponent.MoveType);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			++parsestate.TokenIndex;

			xnaMugen.MoveType movetype = parsestate.ConvertCurrentToken<xnaMugen.MoveType>();
			if (movetype == xnaMugen.MoveType.Unchanged || movetype == xnaMugen.MoveType.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(movetype);
			return parsestate.BaseNode;
		}
	}
}
