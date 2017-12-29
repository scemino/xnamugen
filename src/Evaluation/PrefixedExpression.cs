using System;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	internal class PrefixedExpression : IExpression
	{
		public PrefixedExpression(Expression expression, bool? common)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			m_expression = expression;
			m_common = common;
		}

		public bool IsCommon(bool failover)
		{
			return Common ?? failover;
		}

		public Number[] Evaluate(object state)
		{
			return m_expression.Evaluate(state);
		}

		public Expression Expression => m_expression;

		public bool? Common => m_common;

		public bool IsValid => Expression.IsValid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Expression m_expression;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool? m_common;

		#endregion
	}
}