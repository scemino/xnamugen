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

            var mode = character.Team.Mode;
            bool result;
            switch (mode)
            {
                case xnaMugen.TeamMode.Simul:
                    result = string.Equals(text, "simul", StringComparison.OrdinalIgnoreCase);
                    break;
                case xnaMugen.TeamMode.Single:
                    result = string.Equals(text, "single", StringComparison.OrdinalIgnoreCase);
                    break;
                case xnaMugen.TeamMode.Turns:
                    result = string.Equals(text, "turns", StringComparison.OrdinalIgnoreCase);
                    break;
                default:
                    error = true;
                    return false;
            }

			switch (@operator)
			{
				case Operator.Equals:
                    return result;

				case Operator.NotEquals:
                    return !result;

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
