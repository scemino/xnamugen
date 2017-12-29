namespace xnaMugen.Evaluation
{
	internal interface IExpression
	{
		Number[] Evaluate(object state);

		bool IsValid { get; }
	}
}