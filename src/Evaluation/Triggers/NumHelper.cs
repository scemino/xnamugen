using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumHelper")]
	static class NumHelper
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 helper_id)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			if (helper_id >= 0)
			{
				List<Combat.Helper> helpers;
				if (character.BasePlayer.Helpers.TryGetValue(helper_id, out helpers) == true) return helpers.Count;

				return 0;
			}
			else
			{
				Int32 count = 0;

				foreach (var data in character.BasePlayer.Helpers) count += data.Value.Count;

				return count;
			}
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
