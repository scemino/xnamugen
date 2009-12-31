using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2MoveType")]
	static class P2MoveType
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Operator @operator, xnaMugen.MoveType movetype)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null)
			{
				error = true;
				return false;
			}

			if (movetype == xnaMugen.MoveType.Unchanged || movetype == xnaMugen.MoveType.None)
			{
				error = true;
				return false;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return movetype == opponent.MoveType;

				case Operator.NotEquals:
					return movetype != opponent.MoveType;

				default:
					error = true;
					return false;
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
