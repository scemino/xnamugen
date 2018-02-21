using System;
using System.Diagnostics;
using xnaMugen.Combat;

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

        public Number[] Evaluate(Character character)
		{
            return m_expression.Evaluate(character);
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