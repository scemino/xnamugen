using System;

namespace xnaMugen.Evaluation
{
	interface IExpression
	{
		Number[] Evaluate(Object state);

		Boolean IsValid { get; }
	}
}