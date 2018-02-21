using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation
{
	internal class Expression : IExpression
	{
		public Expression(string expression, List<EvaluationCallback> functions)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));
			if (functions == null) throw new ArgumentNullException(nameof(functions));

			m_expression = expression;
			m_functions = functions;
			m_isvalid = ValidCheck();
		}

		private bool ValidCheck()
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

        public Number[] Evaluate(Character character)
		{
			var result = new Number[m_functions.Count];

			for (var i = 0; i != result.Length; ++i)
			{
				try
				{
                    result[i] = m_functions[i](character);
				}
				catch
				{
					result[i] = new Number();
				}
			}

			return result;
		}

        public Number EvaluateFirst(Character character)
		{
			if (IsValid == false) return new Number();

			try
			{
                return m_functions[0](character);
			}
			catch
			{
				return new Number();
			}
		}

		public override string ToString()
		{
			return m_expression;
		}

		public bool IsValid => m_isvalid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_expression;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<EvaluationCallback> m_functions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_isvalid;

		#endregion
	}
}