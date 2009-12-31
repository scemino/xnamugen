using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;
using xnaMugen.Evaluation.Tokenizing;
using System.Text.RegularExpressions;

namespace xnaMugen.Evaluation
{
	class Expression : IExpression
	{
		public Expression(String expression, List<EvaluationCallback> functions)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			if (functions == null) throw new ArgumentNullException("functions");

			m_expression = expression;
			m_functions = functions;
			m_isvalid = ValidCheck();
		}

		Boolean ValidCheck()
		{
			if (m_functions.Count == 0) return false;

			/*
			foreach (EvaluationCallback EvaluationCallback in m_functions)
			{
				if(EvaluationCallback.Target is Operations.Null) return false;
			}
			*/

			return true;
		}

		public Number[] Evaluate(Object state)
		{
			Number[] result = new Number[m_functions.Count];

			for (Int32 i = 0; i != result.Length; ++i)
			{
				try
				{
					result[i] = m_functions[i](state);
				}
				catch
				{
					result[i] = new Number();
				}
			}

			return result;
		}

		public Number EvaluateFirst(Object state)
		{
			if (IsValid == false) return new Number();

			try
			{
				return m_functions[0](state);
			}
			catch
			{
				return new Number();
			}
		}

		public override String ToString()
		{
			return m_expression;
		}

		public Boolean IsValid
		{
			get { return m_isvalid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_expression;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<EvaluationCallback> m_functions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_isvalid;

		#endregion
	}
}