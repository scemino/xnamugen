using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2Name")]
	internal static class P2Name
	{
		public static bool Evaluate(object state, ref bool error, Operator @operator, string text)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			var p2 = character.Team.OtherTeam.MainPlayer;

			switch (@operator)
			{
				case Operator.Equals:
					return p2 != null ? string.Equals(p2.Profile.PlayerName, text, StringComparison.OrdinalIgnoreCase) : false;

				case Operator.NotEquals:
					return p2 != null ? !string.Equals(p2.Profile.PlayerName, text, StringComparison.OrdinalIgnoreCase) : true;

				default:
					error = true;
					return false;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			var @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++parsestate.TokenIndex;

			var text = parsestate.CurrentText;
			if (text == null) return null;
			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(text);
			return parsestate.BaseNode;
		}
	}
}
