using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumHelper")]
	class NumHelper : Function
	{
		public NumHelper(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32? helper_id;

			if (Children.Count == 0)
			{
				helper_id = null;
			}
			else if (Children.Count == 1)
			{
				Number number = Children[0](state);
				if (number.NumberType != NumberType.Int) return new Number();

				helper_id = number.IntValue;
			}
			else
			{
				return new Number();
			}

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Helper helper = character.FilterEntityAsHelper(entity, helper_id);
				if (helper == null) continue;

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
