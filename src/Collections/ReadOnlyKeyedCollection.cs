using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(ReadOnlyKeyedCollection<,>.DebuggerProxy))]
	class ReadOnlyKeyedCollection<T, U> : ICollection<U>
	{
		class DebuggerProxy
		{
			[DebuggerStepThrough]
			public DebuggerProxy(ReadOnlyKeyedCollection<T, U> collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");

				m_collection = collection;
			}

			[DebuggerStepThrough]
			public override String ToString()
			{
				return m_collection.ToString();
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public U[] Items
			{
				get
				{
					U[] array = new U[m_collection.Count];

					Int32 index = 0;
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
			readonly ReadOnlyKeyedCollection<T, U> m_collection;

			#endregion
		}

		[DebuggerStepThrough]
		public ReadOnlyKeyedCollection(KeyedCollection<T, U> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");

			m_collection = collection;
		}

		[DebuggerStepThrough]
		public Boolean Contains(T key)
		{
			return m_collection.Contains(key);
		}

		public U this[T key]
		{
			get { return m_collection[key]; }
		}

		public U GetItemByIndex(Int32 index)
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

		public Boolean Contains(U item)
		{
			return m_collection.Contains(item);
		}

		public void CopyTo(U[] array, int arrayIndex)
		{
			m_collection.CopyTo(array, arrayIndex);
		}

		public Int32 Count
		{
			get { return m_collection.Count; }
		}

		public Boolean IsReadOnly
		{
			get { return true; }
		}

		public Boolean Remove(U item)
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
		KeyedCollection<T, U> m_collection;

		#endregion
	}
}