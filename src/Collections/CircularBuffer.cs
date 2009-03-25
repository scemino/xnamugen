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
			Data = new T[Capacity];

			Clear();
		}

		public void Clear()
		{
			Size = 0;
			WritePosition = 0;
			ReadPosition = 0;
			WrapAround = false;
		}

		public void Add(T obj)
		{
			Data[WritePosition] = obj;
			++WritePosition;

			if (WrapAround == false)
			{
				++Size;
			}
			else
			{
				++ReadPosition;
				if (ReadPosition == Capacity) ReadPosition = 0;
			}

			if (WritePosition == Capacity)
			{
				WritePosition = 0;
				WrapAround = true;
			}
		}

		public T Get(Int32 index)
		{
			if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException();

			return Data[(index + ReadPosition) % Capacity];
		}

		public T ReverseGet(Int32 index)
		{
			if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException();

			index = Size - index - 1;
			return Data[(index + ReadPosition) % Capacity];
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (Int32 i = 0; i != Size; ++i)
			{
				yield return Get(i);
			}
		}

		public IEnumerator<T> ReverseOrder()
		{
			for (Int32 i = 0; i != Size; ++i)
			{
				yield return ReverseGet(i);
			}
		}

		public T[] GetCurrentBuffer()
		{
			T[] buffer = new T[Size];

			for (Int32 i = 0; i != Size; ++i) buffer[i] = Get(i);

			return buffer;
		}

		public T[] GetCurrentReversedBuffer()
		{
			T[] buffer = new T[Size];

			for (Int32 i = 0; i != Size; ++i) buffer[i] = ReverseGet(i);

			return buffer;
		}

		public Int32 Size { get; private set; }

		public Int32 Capacity
		{
			get { return m_capacity; }
		}

		Int32 WritePosition { get; set; }

		Int32 ReadPosition { get; set; }

		Boolean WrapAround { get; set; }

		T[] Data { get; set; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_capacity;

		#endregion
	}
}