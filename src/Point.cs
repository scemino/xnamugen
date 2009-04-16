using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace xnaMugen
{
	/// <summary>
	/// Defines a point in two dimensional space with whole numbers.
	/// </summary>
	public struct Point : IEquatable<Point>
	{
		/// <summary>
		/// Initialize an instance of this class using the supplied coordinates.
		/// </summary>
		/// <param name="x">X coordinate of point.</param>
		/// <param name="y">Y coordinate of point.</param>
		[DebuggerStepThrough]
		public Point(Int32 x, Int32 y)
			: this()
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Creates an instance of Microsoft.Xna.Framework.Vector2 using the coordinates of the supplied Point instance.
		/// </summary>
		/// <param name="point">The Point used to initialize the Vector2 instance.</param>
		/// <returns>A new Vector2 instance where its X and Y coordinates are equal to the coordinates of the supplied Point instance.</returns>
		[DebuggerStepThrough]
		public static explicit operator Vector2(Point point)
		{
			return new Vector2(point.X, point.Y);
		}

		/// <summary>
		/// Creates an instance of this class using the coordinates of the supplied Microsoft.Xna.Framework.Vector2.
		/// </summary>
		/// <param name="vector">The Vector2 used to initialize the Point instance.</param>
		/// <returns>A new instance of this class where the X and Y coordinates are rounded integers of the coordinates of the supplied Vector2 instance.</returns>
		[DebuggerStepThrough]
		public static explicit operator Point(Vector2 vector)
		{
			return new Point((Int32)vector.X, (Int32)vector.Y);
		}

		/// <summary>
		/// Determines whether the supplied object is an instance of this class representing the same location in two dimensional space.
		/// </summary>
		/// <param name="obj">The object to be compared.</param>
		/// <returns>true if the supplied object is equal to this instance; false otherwise.</returns>
		[DebuggerStepThrough]
		public override Boolean Equals(Object obj)
		{
			if (obj == null || obj.GetType() != this.GetType()) return false;

			return this == (Point)obj;
		}

		[DebuggerStepThrough]
		public Boolean Equals(Point other)
		{
			return this == other;
		}

		/// <summary>
		/// Determines whether two Points represent the same location in two dimensional space.
		/// </summary>
		/// <param name="lhs">The first Point to be compared.</param>
		/// <param name="rhs">The second Point to be compared.</param>
		/// <returns>true if the two Points represent the same location; false otherwise.</returns>
		[DebuggerStepThrough]
		public static Boolean operator ==(Point lhs, Point rhs)
		{
			return lhs.X == rhs.X && lhs.Y == rhs.Y;
		}

		/// <summary>
		/// Determines whether two Points do not represent the same location in two dimensional space.
		/// </summary>
		/// <param name="lhs">The first Point to be compared.</param>
		/// <param name="rhs">The second Point to be compared.</param>
		/// <returns>true if the two Points do not represent the same location; false otherwise.</returns>
		[DebuggerStepThrough]
		public static Boolean operator !=(Point lhs, Point rhs)
		{
			return lhs.X != rhs.X || lhs.Y != rhs.Y;
		}

		/// <summary>
		/// Adds two instances of this class together.
		/// </summary>
		/// <param name="lhs">The first Point to be added together.</param>
		/// <param name="rhs">The second Point to be added together.</param>
		/// <returns>A new instance of this class where each coordinate is the sum of the same coordinate of the supplied instances.</returns>
		[DebuggerStepThrough]
		public static Point operator +(Point lhs, Point rhs)
		{
			return new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		/// <summary>
		/// Subtracts an instance of this class from another instance.
		/// </summary>
		/// <param name="lhs">The Point on the left side of the minus sign.</param>
		/// <param name="rhs">The Point on the right side of the minus sign.</param>
		/// <returns>A new instance of this class where each coordinate is the difference of the same coordinate of the supplied instances.</returns>
		[DebuggerStepThrough]
		public static Point operator -(Point lhs, Point rhs)
		{
			return new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		/// <summary>
		/// Scales an instance of this class by a scalar value.
		/// </summary>
		/// <param name="lhs">The Point instance to be scaled.</param>
		/// <param name="scalar">The scalar value.</param>
		/// <returns>A new instance of this class with each coordinate equal to the same coordinate of the supplied Point mulitplied by the scalar value.</returns>
		[DebuggerStepThrough]
		public static Point operator *(Point lhs, Int32 scalar)
		{
			return new Point(lhs.X * scalar, lhs.Y * scalar);
		}

		/// <summary>
		/// Scales an instance of this class by a scalar value.
		/// </summary>
		/// <param name="lhs">The Point instance to be scaled.</param>
		/// <param name="scalar">The scalar value.</param>
		/// <returns>A new instance of this class with each coordinate equal to the same coordinate of the supplied Point divided by the scalar value.</returns>
		[DebuggerStepThrough]
		public static Point operator /(Point lhs, Int32 scalar)
		{
			return new Point(lhs.X / scalar, lhs.Y / scalar);
		}

		/// <summary>
		/// Generates a hash code based off the value of this instance.
		/// </summary>
		/// <returns>The hash code of this instance.</returns>
		[DebuggerStepThrough]
		public override Int32 GetHashCode()
		{
			return X ^ Y;
		}

		/// <summary>
		/// Generates a System.String whose value is an representation of this instance.
		/// </summary>
		/// <returns>A System.String representation of this instance.</returns>
		[DebuggerStepThrough]
		public override String ToString()
		{
			return X + ", " + Y;
		}

		/// <summary>
		/// The X coordinate of the location this instance represents in space.
		/// </summary>
		/// <returns>The X coordinate.</returns>
		public Int32 X
		{
			get { return m_x; }

			set { m_x = value; }
		}

		/// <summary>
		/// The Y coordinate of the location this instance represents in space.
		/// </summary>
		/// <returns>The Y coordinate.</returns>
		public Int32 Y
		{
			get { return m_y; }

			set { m_y = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_y;

		#endregion
	}
}
