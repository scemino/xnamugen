using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;
using xnaMugen.Evaluation.Tokenizing;
using System.Text.RegularExpressions;

namespace xnaMugen.Evaluation
{
	class PrefixedExpression : IExpression
	{
		public PrefixedExpression(Expression expression, Boolean? common)
		{
			if (expression == null) throw new ArgumentNullException("expression");

			m_expression = expression;
			m_common = common;
		}

		public Boolean IsCommon(Boolean failover)
		{
			return Common ?? failover;
		}

		public Number[] Evaluate(Object state)
		{
			return Expression.Evaluate(state);
		}

		public Expression Expression
		{
			get { return m_expression; }
		}

		public Boolean? Common
		{
			get { return m_common; }
		}

		public Boolean IsValid
		{
			get { return Expression.IsValid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Expression m_expression;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean? m_common;

		#endregion
	}
}