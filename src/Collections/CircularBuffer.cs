using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Size = {Size}")]
	[DebuggerTypeProxy(typeof(CircularBuffer<>.DebuggerProxy))]
	class CircularBuffer<T>
	{
		class DebuggerProxy
		{
			public DebuggerProxy(CircularBuffer<T> data)
			{
				m_data = data;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Items
			{
				get
				{
					return m_data.GetCurrentBuffer();
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly CircularBuffer<T> m_data;
		}

		public CircularBuffer(Int32 capacity)
		{
			if (capacity <= 0) throw new ArgumentOutOfRangeException("Capacity must be greater than zero");

			m_capacity = capacity;
			m_data = new T[m_capacity];

			Clear();
		}

		public void Clear()
		{
			m_size = 0;
			m_writeposition = 0;
			m_readposition = 0;
			m_wraparound = false;
		}

		public void Add(T obj)
		{
			m_data[m_writeposition] = obj;
			++m_writeposition;

			if (m_wraparound == false)
			{
				++m_size;
			}
			else
			{
				++m_readposition;
				if (m_readposition == m_capacity) m_readposition = 0;
			}

			if (m_writeposition == m_capacity)
			{
				m_writeposition = 0;
				m_wraparound = true;
			}
		}

		public T Get(Int32 index)
		{
			if (index < 0 || index >= m_size) throw new ArgumentOutOfRangeException();

			return m_data[(index + m_readposition) % m_capacity];
		}

		public T ReverseGet(Int32 index)
		{
			if (index < 0 || index >= m_size) throw new ArgumentOutOfRangeException();

			return m_data[(m_size - index - 1 + m_readposition) % m_capacity];
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (Int32 i = 0; i != m_size; ++i)
			{
				yield return Get(i);
			}
		}

		public IEnumerator<T> ReverseOrder()
		{
			for (Int32 i = 0; i != m_size; ++i)
			{
				yield return ReverseGet(i);
			}
		}

		public T[] GetCurrentBuffer()
		{
			T[] buffer = new T[m_size];

			for (Int32 i = 0; i != m_size; ++i) buffer[i] = Get(i);

			return buffer;
		}

		public T[] GetCurrentReversedBuffer()
		{
			T[] buffer = new T[m_size];

			for (Int32 i = 0; i != m_size; ++i) buffer[i] = ReverseGet(i);

			return buffer;
		}

		public Int32 Size
		{
			get { return m_size; }
		}

		public Int32 Capacity
		{
			get { return m_capacity; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_capacity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		T[] m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_wraparound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_readposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_writeposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_size;

		#endregion
	}
}