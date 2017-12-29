using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Collections
{
	[DebuggerDisplay("Size = {" + nameof(Size) + "}")]
	[DebuggerTypeProxy(typeof(CircularBuffer<>.DebuggerProxy))]
	internal class CircularBuffer<T>
	{
		private class DebuggerProxy
		{
			public DebuggerProxy(CircularBuffer<T> data)
			{
				m_data = data;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Items => m_data.GetCurrentBuffer();

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly CircularBuffer<T> m_data;
		}

		public CircularBuffer(int capacity)
		{
			if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero");

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

		public T Get(int index)
		{
			if (index < 0 || index >= m_size) throw new ArgumentOutOfRangeException();

			return m_data[(index + m_readposition) % m_capacity];
		}

		public T ReverseGet(int index)
		{
			if (index < 0 || index >= m_size) throw new ArgumentOutOfRangeException();

			return m_data[(m_size - index - 1 + m_readposition) % m_capacity];
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (var i = 0; i != m_size; ++i)
			{
				yield return Get(i);
			}
		}

		public IEnumerator<T> ReverseOrder()
		{
			for (var i = 0; i != m_size; ++i)
			{
				yield return ReverseGet(i);
			}
		}

		public T[] GetCurrentBuffer()
		{
			var buffer = new T[m_size];

			for (var i = 0; i != m_size; ++i) buffer[i] = Get(i);

			return buffer;
		}

		public T[] GetCurrentReversedBuffer()
		{
			var buffer = new T[m_size];

			for (var i = 0; i != m_size; ++i) buffer[i] = ReverseGet(i);

			return buffer;
		}

		public int Size => m_size;

		public int Capacity => m_capacity;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_capacity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private T[] m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_wraparound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_readposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_writeposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_size;

		#endregion
	}
}