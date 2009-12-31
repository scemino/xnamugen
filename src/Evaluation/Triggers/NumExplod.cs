using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumExplod")]
	static class NumExplod
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 explod_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 count = 0;

			foreach (Combat.Explod explod in character.GetExplods(explod_id)) ++count;

			return count;
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
