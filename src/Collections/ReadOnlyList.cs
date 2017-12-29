using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
	[DebuggerTypeProxy(typeof(ReadOnlyList<>.DebuggerProxy))]
	internal class ReadOnlyList<T> : IList<T>
	{
		private class DebuggerProxy
		{
			public DebuggerProxy(ReadOnlyList<T> collection)
			{
				if (collection == null) throw new ArgumentNullException(nameof(collection));

				m_collection = collection;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Items => m_collection.m_list.ToArray();

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly ReadOnlyList<T> m_collection;

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
			if (list == null) throw new ArgumentNullException(nameof(list));

			m_list = list;
		}

		[DebuggerStepThrough]
		public ReadOnlyList(IEnumerable<T> input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			m_list = new List<T>(input);
		}

		[DebuggerStepThrough]
		public List<T>.Enumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		#region IList<T> Members

		[DebuggerStepThrough]
		public int IndexOf(T item)
		{
			return m_list.IndexOf(item);
		}

		[DebuggerStepThrough]
		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public T this[int index]
		{
			get => m_list[index];

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
		public bool Contains(T item)
		{
			return m_list.Contains(item);
		}

		[DebuggerStepThrough]
		public void CopyTo(T[] array, int arrayIndex)
		{
			m_list.CopyTo(array, arrayIndex);
		}

		public int Count => m_list.Count;

		public bool IsReadOnly => true;

		[DebuggerStepThrough]
		public bool Remove(T item)
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
		private readonly List<T> m_list;

		#endregion
	}
}