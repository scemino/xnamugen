using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(ReadOnlyList<>.DebuggerProxy))]
	class ReadOnlyList<T> : IList<T>
	{
		class DebuggerProxy
		{
			public DebuggerProxy(ReadOnlyList<T> collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");

				m_collection = collection;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Items
			{
				get
				{
					return m_collection.m_list.ToArray();
				}
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly ReadOnlyList<T> m_collection;

			#endregion
		}

		[DebuggerStepThrough]
		public ReadOnlyList()
		{
			m_list = new List<T>();
		}

		[DebuggerStepThrough]
		public ReadOnlyList(List<T> list)
		{
			if (list == null) throw new ArgumentNullException("list");

			m_list = list;
		}

		[DebuggerStepThrough]
		public ReadOnlyList(IEnumerable<T> input)
		{
			if (input == null) throw new ArgumentNullException("input");

			m_list = new List<T>(input);
		}

		[DebuggerStepThrough]
		public List<T>.Enumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		#region IList<T> Members

		[DebuggerStepThrough]
		public Int32 IndexOf(T item)
		{
			return m_list.IndexOf(item);
		}

		[DebuggerStepThrough]
		public void Insert(Int32 index, T item)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public void RemoveAt(Int32 index)
		{
			throw new NotImplementedException();
		}

		public T this[Int32 index]
		{
			get { return m_list[index]; }

			set { throw new NotImplementedException(); }
		}

		#endregion

		#region ICollection<T> Members

		[DebuggerStepThrough]
		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public void Clear()
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public Boolean Contains(T item)
		{
			return m_list.Contains(item);
		}

		[DebuggerStepThrough]
		public void CopyTo(T[] array, Int32 arrayIndex)
		{
			m_list.CopyTo(array, arrayIndex);
		}

		public Int32 Count
		{
			get { return m_list.Count; }
		}

		public Boolean IsReadOnly
		{
			get { return true; }
		}

		[DebuggerStepThrough]
		public Boolean Remove(T item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<T> Members

		[DebuggerStepThrough]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		[DebuggerStepThrough]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		#endregion

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<T> m_list;

		#endregion
	}
}