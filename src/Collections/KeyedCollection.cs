using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
	[DebuggerTypeProxy(typeof(KeyedCollection<,>.DebuggerProxy))]
	internal class KeyedCollection<TKey, TItem> : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>
	{
		private class DebuggerProxy
		{
			public DebuggerProxy(KeyedCollection<TKey, TItem> collection)
			{
				if (collection == null) throw new ArgumentNullException(nameof(collection));

				m_collection = collection;
			}

			public override string ToString()
			{
				return m_collection.ToString();
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePair<TKey, TItem>[] Items
			{
				get
				{
					var items = new KeyValuePair<TKey, TItem>[m_collection.Count];

					var index = 0;
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
			private readonly KeyedCollection<TKey, TItem> m_collection;

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
			if (indexer == null) throw new ArgumentNullException(nameof(indexer));

			m_getkey = indexer;
		}

		[DebuggerStepThrough]
		protected override TKey GetKeyForItem(TItem item)
		{
			return m_getkey(item);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Converter<TItem, TKey> m_getkey;

		#endregion
	}
}