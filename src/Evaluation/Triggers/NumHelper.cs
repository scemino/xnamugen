using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumHelper")]
	static class NumHelper
	{
		public static Number Evaluate(Object state, Number number)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32? helper_id;

			switch (number.NumberType)
			{
				case NumberType.Int:
					helper_id = number.IntValue;
					break;

				default:
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
				parsestate.BaseNode.Children.Add(Node.NegativeOneNode);
				return parsestate.BaseNode;
			}
		}
	}
}
