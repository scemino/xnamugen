using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(ReadOnlyDictionary<,>.DebuggerProxy))]
	class ReadOnlyDictionary<T, U> : IDictionary<T, U>
	{
		class DebuggerProxy
		{
			public DebuggerProxy(ReadOnlyDictionary<T, U> dictionary)
			{
				if (dictionary == null) throw new ArgumentNullException("dictionary");

				m_dictionary = dictionary;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePair<T, U>[] Items
			{
				get
				{
					var array = new KeyValuePair<T, U>[m_dictionary.Count];

					Int32 index = 0;
					foreach (var kvp in m_dictionary)
					{
						array[index] = kvp;
						++index;
					}

					return array;
				}
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly ReadOnlyDictionary<T, U> m_dictionary;

			#endregion
		}

		[DebuggerStepThrough]
		public ReadOnlyDictionary(Dictionary<T, U> dictionary)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");

			m_dictionary = dictionary;
		}

		[DebuggerStepThrough]
		public ReadOnlyDictionary()
		{
			m_dictionary = new Dictionary<T, U>();
		}

		[DebuggerStepThrough]
		public Dictionary<T, U>.Enumerator GetEnumerator()
		{
			return m_dictionary.GetEnumerator();
		}

		#region IDictionary<T,U> Members

		[DebuggerStepThrough]
		public void Add(T key, U value)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public Boolean ContainsKey(T key)
		{
			return m_dictionary.ContainsKey(key);
		}

		public ICollection<T> Keys
		{
			get { return m_dictionary.Keys; }
		}

		[DebuggerStepThrough]
		public Boolean Remove(T key)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public Boolean TryGetValue(T key, out U value)
		{
			return m_dictionary.TryGetValue(key, out value);
		}

		public ICollection<U> Values
		{
			get { return m_dictionary.Values; }
		}

		public U this[T key]
		{
			get { return m_dictionary[key]; }

			set { throw new NotImplementedException(); }
		}

		#endregion

		#region ICollection<KeyValuePair<T,U>> Members

		[DebuggerStepThrough]
		public void Add(KeyValuePair<T, U> item)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public void Clear()
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public bool Contains(KeyValuePair<T, U> item)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		public void CopyTo(KeyValuePair<T, U>[] array, Int32 arrayIndex)
		{
			throw new NotImplementedException();
		}

		public Int32 Count
		{
			get { return m_dictionary.Count; }
		}

		public Boolean IsReadOnly
		{
			get { return true; }
		}

		[DebuggerStepThrough]
		public Boolean Remove(KeyValuePair<T, U> item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<T,U>> Members

		[DebuggerStepThrough]
		IEnumerator<KeyValuePair<T, U>> IEnumerable<KeyValuePair<T, U>>.GetEnumerator()
		{
			return m_dictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		[DebuggerStepThrough]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_dictionary.GetEnumerator();
		}

		#endregion

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<T, U> m_dictionary;

		#endregion
	}
}