using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
	[DebuggerTypeProxy(typeof(ReadOnlyKeyedCollection<,>.DebuggerProxy))]
	internal class ReadOnlyKeyedCollection<T, U> : ICollection<U>
	{
		private class DebuggerProxy
		{
			[DebuggerStepThrough]
			public DebuggerProxy(ReadOnlyKeyedCollection<T, U> collection)
			{
				if (collection == null) throw new ArgumentNullException(nameof(collection));

				m_collection = collection;
			}

			[DebuggerStepThrough]
			public override string ToString()
			{
				return m_collection.ToString();
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public U[] Items
			{
				get
				{
					var array = new U[m_collection.Count];

					var index = 0;
					foreach (var kvp in m_collection)
					{
						array[index] = kvp;
						++index;
					}

					return array;
				}
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly ReadOnlyKeyedCollection<T, U> m_collection;

			#endregion
		}

		[DebuggerStepThrough]
		public ReadOnlyKeyedCollection(KeyedCollection<T, U> collection)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));

			m_collection = collection;
		}

		[DebuggerStepThrough]
		public bool Contains(T key)
		{
			return m_collection.Contains(key);
		}

		public U this[T key] => m_collection[key];

		public U GetItemByIndex(int index)
		{
			return m_collection[index];
		}

		#region ICollection<U> Members

		public void Add(U item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(U item)
		{
			return m_collection.Contains(item);
		}

		public void CopyTo(U[] array, int arrayIndex)
		{
			m_collection.CopyTo(array, arrayIndex);
		}

		public int Count => m_collection.Count;

		public bool IsReadOnly => true;

		public bool Remove(U item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<U> Members

		public IEnumerator<U> GetEnumerator()
		{
			return m_collection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_collection.GetEnumerator();
		}

		#endregion

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private KeyedCollection<T, U> m_collection;

		#endregion
	}
}