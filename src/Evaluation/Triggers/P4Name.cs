using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P4Name")]
	class P4Name : Function
	{
		public P4Name(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 2) return new Number();

			Combat.Player p4 = character.Team.TeamMate;
			Operator @operator = (Operator)Arguments[0];
			String text = (String)Arguments[1];

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
