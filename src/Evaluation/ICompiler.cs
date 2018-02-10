namespace xnaMugen.Evaluation
{
    internal interface ICompiler
    {
        EvaluationCallback Create(Node node);
    }
}