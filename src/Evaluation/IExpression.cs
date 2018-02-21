using xnaMugen.Combat;

namespace xnaMugen.Evaluation
{
	internal interface IExpression
	{
        Number[] Evaluate(Character character);

		bool IsValid { get; }
	}
}