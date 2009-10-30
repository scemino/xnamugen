using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2StateType")]
	static class P2StateType
	{
		public static Number Evaluate(Object state, Operator @operator, xnaMugen.StateType statetype)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null) return new Number();

			if (statetype == xnaMugen.StateType.Unchanged || statetype == xnaMugen.StateType.None) return new Number();

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(statetype == opponent.StateType);

				case Operator.NotEquals:
					return new Number(statetype != opponent.StateType);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			++parsestate.TokenIndex;

			xnaMugen.StateType statetype = parsestate.ConvertCurrentToken<xnaMugen.StateType>();
			if (statetype == xnaMugen.StateType.Unchanged || statetype == xnaMugen.StateType.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(statetype);
			return parsestate.BaseNode;
		}
	}
}
