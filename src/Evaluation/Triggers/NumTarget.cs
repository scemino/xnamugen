using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumTarget")]
	internal static class NumTarget
	{
		public static int Evaluate(Character character, ref bool error, int targetId)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var count = 0;
			foreach (var target in character.GetTargets(targetId)) ++count;

			return count;
		}

		public static Node Parse(ParseState parsestate)
		{
			var node = parsestate.BuildParenNumberNode(true);
			if (node != null)
			{
				return node;
			}

			parsestate.BaseNode.Children.Add(Node.NegativeOneNode);
			return parsestate.BaseNode;
		}
	}
}
