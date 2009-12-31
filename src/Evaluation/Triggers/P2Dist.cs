using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2Dist")]
	static class P2Dist
	{

		public static Single Evaluate(Object state, ref Boolean error, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null)
			{
				error = true;
				return 0;
			}

			switch (axis)
			{
				case Axis.X:
					Single distance = Math.Abs(character.CurrentLocation.X - opponent.CurrentLocation.X);
					if (character.CurrentFacing == xnaMugen.Facing.Right)
					{
						return (opponent.CurrentLocation.X >= character.CurrentLocation.X) ? distance : -distance;
					}
					else
					{
						return (opponent.CurrentLocation.X >= character.CurrentLocation.X) ? -distance : distance;
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
			Axis axis = parsestate.ConvertCurrentToken<Axis>();
			if (axis == Axis.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(axis);
			return parsestate.BaseNode;
		}
	}
}
