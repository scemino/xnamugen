using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SysFVar")]
	class SysFVar : Function
	{
		public SysFVar(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}
		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Number r1 = Children[0].Evaluate(state);
			if (r1.NumberType == NumberType.None) return new Number();

			Single result;
			if (character.Variables.GetFloat(r1.IntValue, true, out result) == true)
			{
				return new Number(result);
			}
			else
			{
				return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
