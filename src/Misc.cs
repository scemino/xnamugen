using System;
using Microsoft.Xna.Framework;

namespace xnaMugen
{
	internal static class Misc
	{
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			var temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		public static Facing FlipFacing(Facing input)
		{
			if (input == Facing.Left) return Facing.Right;

			if (input == Facing.Right) return Facing.Left;

			throw new ArgumentException("Not valid Facing", nameof(input));
		}

		public static Color BlendColors(Color a, Color b)
		{
			return new Color(a.ToVector4() * b.ToVector4());
		}

		public static int NextPowerOfTwo(int input)
		{
			if (input < 0) throw new ArgumentOutOfRangeException(nameof(input));

			var output = 1;
			while (output < input) output *= 2;

			return output;
		}

		public static int Clamp(int value, int min, int max)
		{
			value = value > max ? max : value;
			value = value < min ? min : value;
			return value;
		}

		public static float Clamp(float value, float min, float max)
		{
			value = value > max ? max : value;
			value = value < min ? min : value;
			return value;
		}

		public static Vector2 GetOffset(Vector2 location, Facing facing, Vector2 offset)
		{
			var output = location + new Vector2(0, offset.Y);

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

			throw new ArgumentOutOfRangeException(nameof(facing));
		}

		public static int FaceScalar(Facing facing, int value)
		{
			switch(facing)
			{
				case Facing.Left:
					return -value;

				case Facing.Right:
					return value;

				default:
					throw new ArgumentOutOfRangeException(nameof(facing));
			}
		}

		public static float FaceScalar(Facing facing, float value)
		{
			switch (facing)
			{
				case Facing.Left:
					return -value;

				case Facing.Right:
					return value;

				default:
					throw new ArgumentOutOfRangeException(nameof(facing));
			}
		}

        public static string GetPrefix(TeamSide side)
        {
            switch (side)
            {
                case TeamSide.Left:
                    return "p1";

                case TeamSide.Right:
                    return "p2";

                default:
                    throw new ArgumentOutOfRangeException(nameof(side));
            }
        }

        public static string GetMatePrefix(TeamSide side)
        {
            switch (side)
            {
                case TeamSide.Left:
                    return "p3";

                case TeamSide.Right:
                    return "p4";

                default:
                    throw new ArgumentOutOfRangeException(nameof(side));
            }
        }
	}
}