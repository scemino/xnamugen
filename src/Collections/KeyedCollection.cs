using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(KeyedCollection<,>.DebuggerProxy))]
	class KeyedCollection<TKey, TItem> : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>
	{
		class DebuggerProxy
		{
			public DebuggerProxy(KeyedCollection<TKey, TItem> collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");

				m_collection = collection;
			}

			public override String ToString()
			{
				return m_collection.ToString();
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePair<TKey, TItem>[] Items
			{
				get
				{
					KeyValuePair<TKey, TItem>[] items = new KeyValuePair<TKey, TItem>[m_collection.Count];

					Int32 index = 0;
					foreach (var kvp in m_collection)
					{
						items[index] = new KeyValuePair<TKey, TItem>(m_collection.GetKeyForItem(kvp), kvp);
						++index;
					}

					return items;
				}
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly KeyedCollection<TKey, TItem> m_collection;

			#endregion
		}

		[DebuggerStepThrough]
		public KeyedCollection(Converter<TItem, TKey> indexer) :
			this(indexer, null)
		{
		}

		[DebuggerStepThrough]
		public KeyedCollection(Converter<TItem, TKey> indexer, IEqualityComparer<TKey> comparer) :
			base(comparer)
		{
			if (indexer == null) throw new ArgumentNullException("indexer");

			m_getkey = indexer;
		}

		[DebuggerStepThrough]
		protected override TKey GetKeyForItem(TItem item)
		{
			return m_getkey(item);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Converter<TItem, TKey> m_getkey;

		#endregion
	}
}