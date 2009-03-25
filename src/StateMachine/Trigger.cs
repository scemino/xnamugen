using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.StateMachine
{
	[DebuggerDisplay("{Index}, Count = {Expressions}")]
	class Trigger
	{
		public Trigger(Int32 index, List<Evaluation.Expression> expressions)
		{
			if (index < 0) throw new ArgumentNullException("index");
			if (expressions == null) throw new ArgumentNullException("expressions");

			m_index = index;
			m_expressions = expressions;
		}

		public Boolean Check(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			foreach (Evaluation.Expression trigger in m_expressions)
			{
				if (EvaluationHelper.AsBoolean(character, trigger, false) == false) return false;
			}

			return true;
		}

		public Int32 Index
		{
			get { return m_index; }
		}

		public ListIterator<Evaluation.Expression> Expressions
		{
			get { return new ListIterator<Evaluation.Expression>(m_expressions); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_index;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Evaluation.Expression> m_expressions;

		#endregion
	}
}