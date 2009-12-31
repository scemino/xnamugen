using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TimeMod")]
	static class TimeMod
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 r1, Int32 r2)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Int32 statetime_remander = character.StateManager.StateTime % r1;

			return statetime_remander == r2;
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
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

			Node child1 = parsestate.BuildNode(true);
			if (child1 == null) return null;

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node child2 = parsestate.BuildNode(true);
			if (child2 == null) return null;

			parsestate.BaseNode.Children.Add(child1);
			parsestate.BaseNode.Children.Add(child2);

			return parsestate.BaseNode;
		}
	}
}
