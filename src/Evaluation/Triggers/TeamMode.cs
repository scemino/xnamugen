using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TeamMode")]
	static class TeamMode
	{
		public static Number Evaluate(Object state, Operator @operator, String text)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

#warning Hack
			Boolean match = String.Equals(text, "versus", StringComparison.OrdinalIgnoreCase);

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(match);

				case Operator.NotEquals:
					return new Number(!match);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++parsestate.TokenIndex;

			String text = parsestate.CurrentUnknown;
			if (text == null) return null;
			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(text);
			return parsestate.BaseNode;
		}
	}
}
