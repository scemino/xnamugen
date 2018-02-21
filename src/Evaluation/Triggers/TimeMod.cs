using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TimeMod")]
	internal static class TimeMod
	{
		public static bool Evaluate(Character character, ref bool error, int r1, int r2)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			var statetimeRemander = character.StateManager.StateTime % r1;

			return statetimeRemander == r2;
		}

		public static Node Parse(ParseState parsestate)
		{
			var @operator = parsestate.CurrentOperator;
			switch (@operator)
			{
				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.Greater:
				case Operator.GreaterEquals:
				case Operator.Lesser:
				case Operator.LesserEquals:
					++parsestate.TokenIndex;
					break;

				default:
					return null;
			}

			var child1 = parsestate.BuildNode(true);
			if (child1 == null) return null;

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var child2 = parsestate.BuildNode(true);
			if (child2 == null) return null;

			parsestate.BaseNode.Children.Add(child1);
			parsestate.BaseNode.Children.Add(child2);

			return parsestate.BaseNode;
		}
	}
}
