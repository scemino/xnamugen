using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Data.Count}, Index = {Index}")]
	class DataList<T>
	{
		[DebuggerStepThrough]
		public DataList(IEnumerable<T> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			m_data = new ReadOnlyList<T>(new List<T>(data));
			m_index = 0;
		}

		ReadOnlyList<T> Data
		{
			get { return m_data; }
		}

		public Int32 Index
		{
			get { return m_index; }

			set { m_index = value; }
		}

		public T this[Int32 index]
		{
			get { return (index >= 0 && index < Data.Count) ? Data[index] : default(T); }
		}

		public T Current
		{
			get { return (Index < Data.Count) ? Data[Index] : default(T); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<T> m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_index;

		#endregion
	}
}