using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	class Result
	{
		[DebuggerStepThrough]
		public Result()
			: this(3)
		{
		}

		[DebuggerStepThrough]
		public Result(Int32 capacity)
		{
			m_data = new List<Number>(capacity);
		}

		[DebuggerStepThrough]
		public void Add(Number er)
		{
			m_data.Add(er);
		}

		[DebuggerStepThrough]
		public void Clear()
		{
			m_data.Clear();
		}

		[DebuggerStepThrough]
		public Boolean IsValid(Int32 index)
		{
			if (index < 0 || index >= m_data.Count || this[index].NumberType == NumberType.None) return false;
			return true;
		}

		public Number this[Int32 index]
		{
			get { return m_data[index]; }
		}

		public Int32 Count
		{
			get { return m_data.Count; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<Number> m_data;

		#endregion
	}
}