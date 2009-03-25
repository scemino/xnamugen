using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TimeMod")]
	class TimeMod : Function
	{
		public TimeMod(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 2) return new Number();

			Number r1 = Children[0](state);
			Number r2 = Children[1](state);
			if (r1.NumberType != NumberType.Int || r2.NumberType != NumberType.Int) return new Number();

			Int32 statetime_remander = character.StateManager.StateTime % r1.IntValue;

			return new Number(statetime_remander == r2.IntValue);
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
