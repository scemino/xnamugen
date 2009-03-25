using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(DictionaryIterator<,>.DebuggerProxy))]
	struct DictionaryIterator<T, U> : IEnumerable<KeyValuePair<T, U>>
	{
		class DebuggerProxy
		{
			public DebuggerProxy(DictionaryIterator<T, U> collection)
			{
				m_collection = collection;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePair<T, U>[] Items
			{
				get
				{
					List<KeyValuePair<T, U>> list = new List<KeyValuePair<T, U>>();
					foreach (var obj in m_collection) list.Add(obj);

					return list.ToArray();
				}
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly DictionaryIterator<T, U> m_collection;

			#endregion
		}

		public DictionaryIterator(Dictionary<T, U> list)
		{
			m_dictionary = list;
		}

		public Dictionary<T, U>.Enumerator GetEnumerator()
		{
			if (m_dictionary != null) return m_dictionary.GetEnumerator();

			return new Dictionary<T, U>.Enumerator();
		}

		IEnumerator<KeyValuePair<T, U>> IEnumerable<KeyValuePair<T, U>>.GetEnumerator()
		{
			return GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public U this[T key]
		{
			get
			{
				if (m_dictionary != null) return m_dictionary[key];

				throw new ArgumentOutOfRangeException();
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Int32 Count
		{
			get { return (m_dictionary != null) ? m_dictionary.Count : 0; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<T, U> m_dictionary;

		#endregion
	}
}