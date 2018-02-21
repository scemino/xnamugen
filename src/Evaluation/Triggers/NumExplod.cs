using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumExplod")]
	internal static class NumExplod
	{
		public static int Evaluate(Character character, ref bool error, int explodId)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var count = 0;

			foreach (var explod in character.GetExplods(explodId)) ++count;

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
