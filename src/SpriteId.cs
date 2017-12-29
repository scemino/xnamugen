using System;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// An immutable identifier of a xnaMugen.Drawing.Sprite.
	/// </summary>
	public struct SpriteId : IEquatable<SpriteId>
	{
		/// <summary>
		/// Initializes an instance of this class.
		/// </summary>
		/// <param name="group">The group number of the represented sprite.</param>
		/// <param name="image">The image number of the represented sprite.</param>
		[DebuggerStepThrough]
		public SpriteId(int group, int image)
		{
			Group = group;
			Image = image;
		}

		/// <summary>
		/// Generates a hash code based off the value of this instance.
		/// </summary>
		/// <returns>The hash code of this instance.</returns>
		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			return Group ^ Image;
		}

		/// <summary>
		/// Determines whether the supplied SpriteId identifies the same xnaMugen.Drawing.Sprite as this instance.
		/// </summary>
		/// <param name="obj">The SpriteId to be compared to the current instance.</param>
		/// <returns>true if the supplied SpriteId is equal to this instance; false otherwise.</returns>
		[DebuggerStepThrough]
		public bool Equals(SpriteId other)
		{
			return this == other;
		}

		/// <summary>
		/// Determines whether the supplied object is an instance of this class identifing the same xnaMugen.Drawing.Sprite.
		/// </summary>
		/// <param name="obj">The object to be compared.</param>
		/// <returns>true if the supplied object is equal to this instance; false otherwise.</returns>
		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType()) return false;

			return this == (SpriteId)obj;
		}

		/// <summary>
		/// Determines whether two SpriteIds identify the same xnaMugen.Drawing.Sprite.
		/// </summary>
		/// <param name="lhs">The first SpriteId to be compared.</param>
		/// <param name="rhs">The second SpriteId to be compared.</param>
		/// <returns>true if the two Points identify the same Sprite; false otherwise.</returns>
		[DebuggerStepThrough]
		public static bool operator ==(SpriteId lhs, SpriteId rhs)
		{
			return lhs.Group == rhs.Group && lhs.Image == rhs.Image;
		}

		/// <summary>
		/// Determines whether the two SpriteIds do not identify the same xnaMugen.Drawing.Sprite.
		/// </summary>
		/// <param name="lhs">The first SpriteId to be compared.</param>
		/// <param name="rhs">The second SpriteId to be compared.</param>
		/// <returns>true if the two Points do not identify the same Sprite; false otherwise.</returns>
		[DebuggerStepThrough]
		public static bool operator !=(SpriteId lhs, SpriteId rhs)
		{
			return lhs.Group != rhs.Group || lhs.Image != rhs.Image;
		}

		/// <summary>
		/// Generates a System.String whose value is an representation of this instance.
		/// </summary>
		/// <returns>A System.String representation of this instance.</returns>
		public override string ToString()
		{
			return this != Invalid ? Group + ", " + Image : "Invalid";
		}

		/// <summary>
		/// A SpriteId that does not identify a xnaMugen.Drawing.Sprite.
		/// </summary>
		/// <returns>A SpriteId that does not identify a Sprite.</returns>
		public static SpriteId Invalid => new SpriteId(int.MinValue, int.MinValue);

		/// <summary>
		/// A SpriteId that identifies the xnaMugen.Drawing.Sprite that is the Large Portrait.
		/// </summary>
		/// <returns>A SpriteId that identifies the Large Portrait Sprite.</returns>
		public static SpriteId LargePortrait => new SpriteId(9000, 1);

		/// <summary>
		/// A SpriteId that identifies the xnaMugen.Drawing.Sprite that is the Small Portrait.
		/// </summary>
		/// <returns>A SpriteId that identifies the Small Portrait Sprite.</returns>
		public static SpriteId SmallPortrait => new SpriteId(9000, 0);

		/// <summary>
		/// The Group number of this instance.
		/// </summary>
		/// <returns>The Group number.</returns>
		public readonly int Group;

		/// <summary>
		/// The Image number of this instance.
		/// </summary>
		/// <returns>The Image number.</returns>
		public readonly int Image;
	}
}
