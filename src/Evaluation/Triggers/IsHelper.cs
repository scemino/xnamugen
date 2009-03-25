using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IsHelper")]
	class IsHelper : Function
	{
		public IsHelper(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number(false);

			if (Children.Count == 0)
			{
				return new Number(true);
			}
			else if (Children.Count == 1)
			{
				Number helperid = Children[0](state);
				if (helperid.NumberType != NumberType.Int) return new Number();

				return new Number(helper.Data.HelperId == helperid.IntValue);
			}

			return new Number();
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
				return parsestate.BaseNode;
			}
		}
	}
}
