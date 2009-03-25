using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace xnaMugen
{
	static class Misc
	{
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		public static Facing FlipFacing(Facing input)
		{
			if (input == Facing.Left) return Facing.Right;

			if (input == Facing.Right) return Facing.Left;

			throw new ArgumentException("Not valid Facing", "input");
		}

		public static Color BlendColors(Color a, Color b)
		{
			return new Color(a.ToVector4() * b.ToVector4());
		}

		public static Int32 NextPowerOfTwo(Int32 input)
		{
			if (input < 0) throw new ArgumentOutOfRangeException("input");

			Int32 output = 1;
			while (output < input) output *= 2;

			return output;
		}

		public static Int32 Clamp(Int32 value, Int32 min, Int32 max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}

		public static Single Clamp(Single value, Single min, Single max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}

		public static Vector2 GetOffset(Vector2 location, Facing facing, Vector2 offset)
		{
			Vector2 output = location + new Vector2(0, offset.Y);

			if (facing == Facing.Right)
			{
				output.X += offset.X;
				return output;
			}

			if (facing == Facing.Left)
			{
				output.X -= offset.X;
				return output;
			}

			throw new ArgumentOutOfRangeException("facing");
		}

		public static Int32 FaceScalar(Facing facing, Int32 value)
		{
			switch(facing)
			{
				case Facing.Left:
					return -value;

				case Facing.Right:
					return value;

				default:
					throw new ArgumentOutOfRangeException("facing");
			}
		}

		public static Single FaceScalar(Facing facing, Single value)
		{
			switch (facing)
			{
				case Facing.Left:
					return -value;

				case Facing.Right:
					return value;

				default:
					throw new ArgumentOutOfRangeException("facing");
			}
		}

        public static String GetPrefix(TeamSide side)
        {
            switch (side)
            {
                case TeamSide.Left:
                    return "p1";

                case TeamSide.Right:
                    return "p2";

                default:
                    throw new ArgumentOutOfRangeException("side");
            }
        }
	}
}