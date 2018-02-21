using System.Collections.Generic;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumHelper")]
	internal static class NumHelper
	{
		public static int Evaluate(Character character, ref bool error, int helperId)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			if (helperId >= 0)
			{
				List<Combat.Helper> helpers;
				if (character.BasePlayer.Helpers.TryGetValue(helperId, out helpers)) return helpers.Count;

				return 0;
			}

			var count = 0;

			foreach (var data in character.BasePlayer.Helpers) count += data.Value.Count;

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
