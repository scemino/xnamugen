using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Defines a collision box for an AnimationElement.
	/// </summary>
	internal struct Clsn
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
			var x1 = location.X + (int)(Rectangle.Left * scale.X);
			var x2 = location.X + (int)(Rectangle.Right * scale.X);

			if (facing == Facing.Left)
			{
				x1 = location.X - (int)(Rectangle.Left * scale.X);
				x2 = location.X - (int)(Rectangle.Right * scale.X);
			}

			if (x1 > x2)
			{
				Misc.Swap(ref x1, ref x2);
			}

			var y1 = location.Y + (int)(Rectangle.Top * scale.Y);
			var y2 = location.Y + (int)(Rectangle.Bottom * scale.Y);

			var rectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
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
		public override string ToString()
		{
			return ClsnType.ToString() + " - " + Rectangle.ToString();
		}

		/// <summary>
		/// Gets the type of collision box.
		/// </summary>
		/// <returns>The type of collision box.</returns>
		public ClsnType ClsnType => m_clsntype;

		/// <summary>
		/// Gets the dimensions of the collision box.
		/// </summary>
		/// <returns>The dimensions of the collision box.</returns>
		public Rectangle Rectangle => m_rect;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ClsnType m_clsntype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Rectangle m_rect;

		#endregion
	}
}