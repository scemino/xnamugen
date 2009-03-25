using System;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// Represents a memory buffer that can be reused without reallocating memory.
	/// </summary>
	class SharedBuffer : SubSystem
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		public SharedBuffer(SubSystems subsystems)
			: base(subsystems)
		{
			m_buffer = new Byte[1];
		}

		/// <summary>
		/// Ensures that the memory buffer meets or exceeds a minimum size.
		/// </summary>
		/// <param name="size">Size that the memory buffer is to meet or exceed.</param>
		public void EnsureSize(Int32 size)
		{
			if (m_buffer == null || m_buffer.Length < size) m_buffer = new Byte[size];
		}

		/// <summary>
		/// Returns as object that can be used to synchronize access to the memory buffer.
		/// </summary>
		/// <returns>An object that can be used to synchronize access to the memory buffer.</returns>
		public Object LockObject
		{
			get { return m_buffer.SyncRoot; }
		}

		/// <summary>
		/// Gets or sets the element of the memory buffer at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element of the memory buffer at the specified index.</returns>
		public Byte this[Int32 index]
		{
			get { return m_buffer[index]; }

			set { m_buffer[index] = value; }
		}

		/// <summary>
		/// Memory buffer managed by this instance.
		/// </summary>
		/// <returns>A System.Byte array.</returns>
		public Byte[] Buffer
		{
			get { return m_buffer; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Byte[] m_buffer;

		#endregion
	}
}