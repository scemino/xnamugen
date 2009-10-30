using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Log")]
	static class Log
	{
		public static Number Evaluate(Object state, Number lhs, Number rhs)
		{
			if (lhs.NumberType == NumberType.None || rhs.NumberType == NumberType.None) return new Number();

			return new Number(Math.Log(lhs.FloatValue, rhs.FloatValue));
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
