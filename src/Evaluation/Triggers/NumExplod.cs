using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumExplod")]
	class NumExplod : Function
	{
		public NumExplod(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32? explod_id;

			if (Children.Count == 0)
			{
				explod_id = null;
			}
			else if (Children.Count == 1)
			{
				Number number = Children[0].Evaluate(state);
				if (number.NumberType != NumberType.Int) return new Number();

				explod_id = number.IntValue;
			}
			else
			{
				return new Number();
			}

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Explod explod = character.FilterEntityAsExplod(entity, explod_id);
				if (explod == null) continue;

				++count;
			}

			return new Number(count);
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node != null)
			{
				return node;
			}
			else
			{
				return parsestate.BaseNode;
			}
		}
	}
}
