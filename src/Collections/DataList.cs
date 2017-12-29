using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Data.Count}, Index = {Index}")]
	internal class DataList<T>
	{
		[DebuggerStepThrough]
		public DataList(IEnumerable<T> data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_data = new ReadOnlyList<T>(new List<T>(data));
			m_index = 0;
		}

		private ReadOnlyList<T> Data => m_data;

		public int Index
		{
			get => m_index;

			set { m_index = value; }
		}

		public T this[int index] => index >= 0 && index < Data.Count ? Data[index] : default(T);

		public T Current => Index < Data.Count ? Data[Index] : default(T);

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<T> m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_index;

		#endregion
	}
}