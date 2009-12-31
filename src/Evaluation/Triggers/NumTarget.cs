using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumTarget")]
	static class NumTarget
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 target_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 count = 0;
			foreach (Combat.Character target in character.GetTargets(target_id)) ++count;

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
