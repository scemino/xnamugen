using System;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// Base class to ease proper resource disposal.
	/// </summary>
	abstract class Resource: IDisposable
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected Resource()
		{
			m_disposed = false;
		}

		/// <summary>
		/// Finalizer to force disposal of unmanaged resources.
		/// </summary>
		~Resource()
		{
			Dispose(false);
		}

		/// <summary>
		/// Disposes managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes of resources managed by this class instance.
		/// </summary>
		/// <param name="disposing">Determined whether to dispose of managed resources.</param>
		protected virtual void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
			}

			m_disposed = true;
		}

		/// <summary>
		/// Get whether this object has disposed its resources.
		/// </summary>
		/// <returns>true if Dispose() has been called on this object; false otherwise.</returns>
		public Boolean IsDisposed
		{
			get { return m_disposed; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_disposed;

		#endregion
	}
}
