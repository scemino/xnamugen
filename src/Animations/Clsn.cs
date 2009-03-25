using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Defines a collision box for an AnimationElement.
	/// </summary>
	struct Clsn
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="clsntype">The type of collision box.</param>
		/// <param name="rectangle">The Rectangle defining the dimensions of the collision box.</param>
		public Clsn(ClsnType clsntype, Rectangle rectangle)
		{
			m_clsntype = clsntype;
			m_rect = rectangle;
		}

		/// <summary>
		/// Calculates the location of the collision box.
		/// </summary>
		/// <param name="location">The base location of the box.</param>
		/// <param name="scale">The scaling factor of the box.</param>
		/// <param name="facing">The facing of the box.</param>
		/// <returns>The ingame dimension of the collision box.</returns>
		public Rectangle MakeRect(Point location, Vector2 scale, Facing facing)
		{
			Int32 x1 = location.X + (Int32)(Rectangle.Left * scale.X);
			Int32 x2 = location.X + (Int32)(Rectangle.Right * scale.X);

			if (facing == Facing.Left)
			{
				x1 = location.X - (Int32)(Rectangle.Left * scale.X);
				x2 = location.X - (Int32)(Rectangle.Right * scale.X);
			}

			if (x1 > x2)
			{
				Misc.Swap(ref x1, ref x2);
			}

			Int32 y1 = location.Y + (Int32)(Rectangle.Top * scale.Y);
			Int32 y2 = location.Y + (Int32)(Rectangle.Bottom * scale.Y);

			Rectangle rectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
			return rectangle;
		}

		/// <summary>
		/// Calculates the location of the collision box.
		/// </summary>
		/// <param name="location">The base location of the box.</param>
		/// <param name="scale">The scaling factor of the box.</param>
		/// <param name="facing">The facing of the box.</param>
		/// <returns>The ingame dimension of the collision box.</returns>
		public Rectangle MakeRect(Vector2 location, Vector2 scale, Facing facing)
		{
			return MakeRect((Point)location, scale, facing);
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override String ToString()
		{
			return ClsnType.ToString() + " - " + Rectangle.ToString();
		}

		/// <summary>
		/// Gets the type of collision box.
		/// </summary>
		/// <returns>The type of collision box.</returns>
		public ClsnType ClsnType
		{
			get { return m_clsntype; }
		}

		/// <summary>
		/// Gets the dimensions of the collision box.
		/// </summary>
		/// <returns>The dimensions of the collision box.</returns>
		public Rectangle Rectangle
		{
			get { return m_rect; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ClsnType m_clsntype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Rectangle m_rect;

		#endregion
	}
}