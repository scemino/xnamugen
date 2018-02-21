using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TeamMode")]
	internal static class TeamMode
	{
		public static bool Evaluate(Character character, ref bool error, Operator @operator, string text)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

#warning Hack
			var match = string.Equals(text, "versus", StringComparison.OrdinalIgnoreCase);

			switch (@operator)
			{
				case Operator.Equals:
					return match;

				case Operator.NotEquals:
					return !match;

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

			var text = parsestate.CurrentUnknown;
			if (text == null) return null;
			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(text);
			return parsestate.BaseNode;
		}
	}
}
