using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("StateType")]
	class StateType : Function
	{
		public StateType(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 2) return new Number();

			Operator @operator = (Operator)Arguments[0];
			xnaMugen.StateType statetype = (xnaMugen.StateType)Arguments[1];

			if (statetype == xnaMugen.StateType.Unchanged || statetype == xnaMugen.StateType.None) return new Number();

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(statetype == character.StateType);

				case Operator.NotEquals:
					return new Number(statetype != character.StateType);

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
