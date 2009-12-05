using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumExplod")]
	static class NumExplod
	{
		public static Number Evaluate(Object state, Number number)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (number.NumberType != NumberType.Int) return new Number();

			Int32 explod_id = number.IntValue;
			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Explod explod = character.FilterEntityAsExplod(entity, explod_id);
				if (explod != null) ++count;
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
