using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Evaluation.Operators
{
	class Literal : IFunction
	{
		public Literal(EvaluationResult result)
		{
			m_result = result;
		}

		public EvaluationResult Evaluate(EvaluationState state, ReadOnlyList<IEvaluate> args, ReadOnlyList<Object> data)
		{
			return m_result;
		}

		public override String ToString()
		{
			return m_result.ToString();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly EvaluationResult m_result;

		#endregion
	}
}