using System;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	class NumberReturn
	{
		public NumberReturn(Number number)
		{
			m_number = number;
		}

		public override String ToString()
		{
			return m_number.ToString();
		}

		[DebuggerHidden]
		public Number Evaluate(Object state)
		{
			return m_number;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Number m_number;

		#endregion
	}
}