using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Floor")]
	static class Floor
	{
		public static Number Evaluate(Object state, Number value)
		{
			switch (value.NumberType)
			{
				case NumberType.Int:
					return new Number((Int32)Math.Floor(value.FloatValue));

				case NumberType.Float:
					return new Number(Math.Floor(value.FloatValue));

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
