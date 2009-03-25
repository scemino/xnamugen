using System;
using System.Diagnostics;

namespace xnaMugen.Audio
{
	/// <summary>
	/// Identifies which SoundManager is using a Channel to play a sound.
	/// </summary>
	struct ChannelId : IEquatable<ChannelId>
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="manager">The SoundManager to be identified.</param>
		/// <param name="number">The channel number used for playing a sound.</param>
		public ChannelId(SoundManager manager, Int32 number)
		{
			if (number < -1) throw new ArgumentOutOfRangeException("number");

			m_manager = manager;
			m_number = number;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type;
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public Boolean Equals(ChannelId other)
		{
			return this == other;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>true if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override Boolean Equals(Object obj)
		{
			if (obj == null || obj.GetType() != this.GetType()) return false;

			return this == (ChannelId)obj;
		}

		/// <summary>
		/// Indicates whether two instances are equal.
		/// </summary>
		/// <param name="lhs">The first instance to be compared.</param>
		/// <param name="rhs">The second instance to be compated.</param>
		/// <returns>true is the two instance are equal; false otherwise.</returns>
		public static Boolean operator ==(ChannelId lhs, ChannelId rhs)
		{
			return Object.ReferenceEquals(lhs.Manager, rhs.Manager) && lhs.Number == rhs.Number;
		}

		/// <summary>
		/// Indicates whether two instances are not equal.
		/// </summary>
		/// <param name="lhs">The first instance to be compared.</param>
		/// <param name="rhs">The second instance to be compated.</param>
		/// <returns>true is the two instance are not equal; false otherwise.</returns>
		public static Boolean operator !=(ChannelId lhs, ChannelId rhs)
		{
			return (lhs == rhs) == false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override Int32 GetHashCode()
		{
			return Number.GetHashCode();
		}

		/// <summary>
		/// Get the SoundManager that is currently using the Channel.
		/// </summary>
		/// <returns>The SoundManager currently using the Channel.</returns>
		public SoundManager Manager
		{
			get { return m_manager; }
		}

		/// <summary>
		/// Gets the channel number the SoundManager used to play this sound.
		/// </summary>
		/// <returns>The channel number.</returns>
		public Int32 Number
		{
			get { return m_number; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundManager m_manager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_number;

		#endregion
	}
}