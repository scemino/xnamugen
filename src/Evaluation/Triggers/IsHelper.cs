using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHelper")]
	static class IsHelper
	{
		public static Number Evaluate(Object state, Number helperid)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number(false);

			switch (helperid.NumberType)
			{
				case NumberType.Int:
				case NumberType.Float:
					return new Number(helper.Data.HelperId == helperid.IntValue);

				default:
					return new Number(true);
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
				parsestate.BaseNode.Children.Add(Node.EmptyNode);
				return parsestate.BaseNode;
			}
		}
	}
}
