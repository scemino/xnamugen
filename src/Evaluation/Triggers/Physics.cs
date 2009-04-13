using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Physics")]
	class Physics : Function
	{
		public Physics(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 2) return new Number();

			Operator @operator = (Operator)Arguments[0];
			xnaMugen.Physics physics = (xnaMugen.Physics)Arguments[1];

			if (physics == xnaMugen.Physics.Unchanged || physics == xnaMugen.Physics.None) return new Number();

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(physics == character.Physics);

				case Operator.NotEquals:
					return new Number(physics != character.Physics);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			++parsestate.TokenIndex;

			xnaMugen.Physics physics = parsestate.ConvertCurrentToken<xnaMugen.Physics>();
			if (physics == xnaMugen.Physics.Unchanged || physics == xnaMugen.Physics.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(physics);
			return parsestate.BaseNode;
		}
	}
}
