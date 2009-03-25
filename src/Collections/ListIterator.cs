using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(ListIterator<>.DebuggerProxy))]
	struct ListIterator<T> : IEnumerable<T>
	{
		class DebuggerProxy
		{
			public DebuggerProxy(ListIterator<T> collection)
			{
				m_collection = collection;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Items
			{
				get
				{
					List<T> list = new List<T>();
					foreach (var obj in m_collection) list.Add(obj);

					return list.ToArray();
				}
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly ListIterator<T> m_collection;

			#endregion
		}

		public ListIterator(List<T> list)
		{
			m_list = list;
		}

		public List<T>.Enumerator GetEnumerator()
		{
			if (m_list != null) return m_list.GetEnumerator();

			return new List<T>.Enumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T this[Int32 index]
		{
			get
			{
				if (m_list != null) return m_list[index];

				throw new ArgumentOutOfRangeException();
			}

			set
			{
				if (m_list != null)
				{
					m_list[index] = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public Int32 Count
		{
			get { return (m_list != null) ? m_list.Count : 0; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<T> m_list;

		#endregion
	}
}