using System;

namespace xnaMugen.Evaluation
{
	interface IExpression
	{
		Result Evaluate(Object state);

		void Evaluate(Object state, Result result);

		Boolean IsValid { get; }
	}
}