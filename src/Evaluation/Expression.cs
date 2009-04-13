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
		public Expression(String expression, List<IFunction> functions)
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

			foreach (IFunction fucntion in m_functions)
			{
				if (fucntion is Operations.Null) return false;
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

			foreach (IFunction fucntion in m_functions)
			{
				result.Add(fucntion.Evaluate(state));
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

		ListIterator<IFunction> CallBacks
		{
			get { return new ListIterator<IFunction>(m_functions); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_expression;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<IFunction> m_functions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_isvalid;

		#endregion
	}
}