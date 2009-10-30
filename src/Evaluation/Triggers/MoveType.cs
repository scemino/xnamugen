using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveType")]
	static class MoveType
	{
		public static Number Evaluate(Object state, Operator @operator, xnaMugen.MoveType movetype)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (movetype == xnaMugen.MoveType.Unchanged || movetype == xnaMugen.MoveType.None) return new Number();

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(movetype == character.MoveType);

				case Operator.NotEquals:
					return new Number(movetype != character.MoveType);

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
