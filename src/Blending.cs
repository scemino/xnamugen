using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// Alpha blending information that is sent to the graphics shader.
	/// </summary>
	struct Blending : IEquatable<Blending>
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="type">The type of color blending to be used.</param>
		/// <param name="source">The weight of the source color used in color blending.</param>
		/// <param name="destination">The weight of the destination color used in color blending.</param>
		[DebuggerStepThrough]

		public Blending(BlendType type, Byte source, Byte destination)
		{
			m_type = type;
			m_sourcefactor = (type != BlendType.None) ? source : (Byte)0;
			m_destionationfactor = (type != BlendType.None) ? destination : (Byte)0; ;
		}
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="type">The type of color blending to be used.</param>
		/// <param name="source">The weight of the source color used in color blending.</param>
		/// <param name="destination">The weight of the destination color used in color blending.</param>
		[DebuggerStepThrough]
		public Blending(BlendType type, Int32 source, Int32 destination)
		{
			m_type = type;
			m_sourcefactor = (type != BlendType.None) ? (Byte)Misc.Clamp(source, 0, 255) : (Byte)0;
			m_destionationfactor = (type != BlendType.None) ? (Byte)Misc.Clamp(destination, 0, 255) : (Byte)0; ;
		}

		[DebuggerStepThrough]
		public Boolean Equals(Blending other)
		{
			return this == other;
		}

		[DebuggerStepThrough]
		public override Boolean Equals(Object obj)
		{
			if (obj == null || obj.GetType() != this.GetType()) return false;

			return this == (Blending)obj;
		}

		public static Boolean operator ==(Blending lhs, Blending rhs)
		{
			return lhs.BlendType == rhs.BlendType && lhs.SourceFactor == rhs.SourceFactor && lhs.DestinationFactor == rhs.DestinationFactor;
		}

		public static Boolean operator !=(Blending lhs, Blending rhs)
		{
			return lhs.BlendType != rhs.BlendType || lhs.SourceFactor != rhs.SourceFactor || lhs.DestinationFactor != rhs.DestinationFactor;
		}

		/// <summary>
		/// Generates a hash code based off the value of this instance.
		/// </summary>
		/// <returns>The hash code of this instance.</returns>
		[DebuggerStepThrough]
		public override Int32 GetHashCode()
		{
			if (BlendType == BlendType.None) return 0;

			return BlendType.GetHashCode() ^ SourceFactor ^ DestinationFactor;
		}

		/// <summary>
		/// The type of color blending to be used.
		/// </summary>
		/// <returns></returns>
		public BlendType BlendType
		{
			get { return m_type; }
		}

		/// <summary>
		/// The weight of the source color used in color blending.
		/// </summary>
		/// <returns>The weight of the source color.</returns>
		public Byte SourceFactor
		{
			get { return m_sourcefactor; }
		}

		/// <summary>
		/// The weight of the destination color used in color blending.
		/// </summary>
		/// <returns>The weight of the destination color.</returns>
		public Byte DestinationFactor
		{
			get { return m_destionationfactor; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly BlendType m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_sourcefactor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_destionationfactor;

		#endregion
	}
}