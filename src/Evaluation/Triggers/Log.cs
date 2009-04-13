using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Log")]
	class Log : Function
	{
		public Log(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number n1 = Children[0].Evaluate(state);
			Number n2 = Children[1].Evaluate(state);
			if (n1.NumberType == NumberType.None || n2.NumberType == NumberType.None) return new Number();

			return new Number(Math.Log(n1.FloatValue, n2.FloatValue));
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
