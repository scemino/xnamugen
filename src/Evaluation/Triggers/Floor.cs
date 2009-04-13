using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Floor")]
	class Floor : Function
	{
		public Floor(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0].Evaluate(state);

			switch (number.NumberType)
			{
				case NumberType.Int:
					return new Number((Int32)Math.Floor(number.FloatValue));

				case NumberType.Float:
					return new Number(Math.Floor(number.FloatValue));

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
