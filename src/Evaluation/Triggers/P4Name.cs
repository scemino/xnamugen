using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P4Name")]
	static class P4Name
	{
		public static Number Evaluate(Object state, Operator @operator, String text)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Player p4 = character.Team.TeamMate;

			switch (@operator)
			{
				case Operator.Equals:
					return new Number((p4 != null) ? String.Equals(p4.Profile.PlayerName, text, StringComparison.OrdinalIgnoreCase) : false);

				case Operator.NotEquals:
					return new Number((p4 != null) ? !String.Equals(p4.Profile.PlayerName, text, StringComparison.OrdinalIgnoreCase) : true);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++parsestate.TokenIndex;

			String text = parsestate.CurrentText;
			if (text == null) return null;
			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(text);
			return parsestate.BaseNode;
		}
	}
}
