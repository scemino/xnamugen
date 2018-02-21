using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2Dist")]
	internal static class P2Dist
	{

		public static float Evaluate(Character character, ref bool error, Axis axis)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var opponent = character.GetOpponent();
			if (opponent == null)
			{
				error = true;
				return 0;
			}

			switch (axis)
			{
				case Axis.X:
					var distance = Math.Abs(character.CurrentLocation.X - opponent.CurrentLocation.X);
					if (character.CurrentFacing == xnaMugen.Facing.Right)
					{
						return opponent.CurrentLocation.X >= character.CurrentLocation.X ? distance : -distance;
					}
					else
					{
						return opponent.CurrentLocation.X >= character.CurrentLocation.X ? -distance : distance;
					}

				case Axis.Y:
					return opponent.CurrentLocation.Y - character.CurrentLocation.Y;

				default:
					error = true;
					return 0;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			var axis = parsestate.ConvertCurrentToken<Axis>();
			if (axis == Axis.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(axis);
			return parsestate.BaseNode;
		}
	}
}
