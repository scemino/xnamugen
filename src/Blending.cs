using System;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// Alpha blending information that is sent to the graphics shader.
	/// </summary>
	internal struct Blending : IEquatable<Blending>
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="type">The type of color blending to be used.</param>
		/// <param name="source">The weight of the source color used in color blending.</param>
		/// <param name="destination">The weight of the destination color used in color blending.</param>
		[DebuggerStepThrough]

		public Blending(BlendType type, byte source, byte destination)
		{
			m_type = type;
			m_sourcefactor = type != BlendType.None ? source : (byte)0;
			m_destionationfactor = type != BlendType.None ? destination : (byte)0;
		}
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="type">The type of color blending to be used.</param>
		/// <param name="source">The weight of the source color used in color blending.</param>
		/// <param name="destination">The weight of the destination color used in color blending.</param>
		[DebuggerStepThrough]
		public Blending(BlendType type, int source, int destination)
		{
			m_type = type;
			m_sourcefactor = type != BlendType.None ? (byte)Misc.Clamp(source, 0, 255) : (byte)0;
			m_destionationfactor = type != BlendType.None ? (byte)Misc.Clamp(destination, 0, 255) : (byte)0;
		}

		[DebuggerStepThrough]
		public bool Equals(Blending other)
		{
			return this == other;
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType()) return false;

			return this == (Blending)obj;
		}

		public static bool operator ==(Blending lhs, Blending rhs)
		{
			return lhs.BlendType == rhs.BlendType && lhs.SourceFactor == rhs.SourceFactor && lhs.DestinationFactor == rhs.DestinationFactor;
		}

		public static bool operator !=(Blending lhs, Blending rhs)
		{
			return lhs.BlendType != rhs.BlendType || lhs.SourceFactor != rhs.SourceFactor || lhs.DestinationFactor != rhs.DestinationFactor;
		}

		/// <summary>
		/// Generates a hash code based off the value of this instance.
		/// </summary>
		/// <returns>The hash code of this instance.</returns>
		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			if (BlendType == BlendType.None) return 0;

			return BlendType.GetHashCode() ^ SourceFactor ^ DestinationFactor;
		}

		/// <summary>
		/// The type of color blending to be used.
		/// </summary>
		/// <returns></returns>
		public BlendType BlendType => m_type;

		/// <summary>
		/// The weight of the source color used in color blending.
		/// </summary>
		/// <returns>The weight of the source color.</returns>
		public byte SourceFactor => m_sourcefactor;

		/// <summary>
		/// The weight of the destination color used in color blending.
		/// </summary>
		/// <returns>The weight of the destination color.</returns>
		public byte DestinationFactor => m_destionationfactor;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly BlendType m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_sourcefactor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_destionationfactor;

		#endregion
	}
}