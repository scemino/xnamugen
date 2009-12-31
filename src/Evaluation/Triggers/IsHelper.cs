using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHelper")]
	static class IsHelper
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 helperid)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return false;

			return (helperid >= 0) ? helper.Data.HelperId == helperid : true;
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
