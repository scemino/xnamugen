using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Evaluation.Operators
{
	class RangeCheck : IFunction
	{
		public EvaluationResult Evaluate(EvaluationState state, ReadOnlyList<IEvaluate> args, ReadOnlyList<Object> data)
		{
			EvaluationResult checkagainst = args[0].Evaluate(state);
			EvaluationResult first = args[1].Evaluate(state);
			EvaluationResult second = args[2].Evaluate(state);

			Operator compare = (Operator)data[0];
			Symbol pre_op = (Symbol)data[1];
			Symbol post_op = (Symbol)data[2];

			return BaseOperations.Range(checkagainst, first, second, compare, pre_op, post_op);
		}
	}
}