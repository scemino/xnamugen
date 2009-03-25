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
		public Expression(String expression, List<CallBack> functions)
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

			foreach (CallBack callback in m_functions)
			{
				if(callback.Target is Operations.Null) return false;
			}

			return true;
		}

		public Result Evaluate(Object state)
		{
			Result result = new Result();

			Evaluate(state, result);

			return result;
		}

		public void Evaluate(Object state, Result result)
		{
			if (result == null) throw new ArgumentNullException("result");

			result.Clear();

			foreach (CallBack func in m_functions)
			{
				result.Add(func(state));
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

		ListIterator<CallBack> CallBacks
		{
			get { return new ListIterator<CallBack>(m_functions); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_expression;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<CallBack> m_functions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_isvalid;

		#endregion
	}
}