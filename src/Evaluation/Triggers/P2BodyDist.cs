using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2BodyDist")]
	static class P2BodyDist
	{
		public static Number Evaluate(Object state, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null) return new Number();

			switch (axis)
			{
				case Axis.X:
					Single mylocation = character.GetFrontLocation();
					Single opplocation = opponent.GetFrontLocation();
					Single distance = Math.Abs(mylocation - opplocation);
					if (character.CurrentFacing == xnaMugen.Facing.Right)
					{
						return (opponent.CurrentLocation.X >= character.CurrentLocation.X) ? new Number(distance) : new Number(-distance);
					}
					else
					{
						return (opponent.CurrentLocation.X >= character.CurrentLocation.X) ? new Number(-distance) : new Number(distance);
					}

				case Axis.Y:
					return new Number(opponent.CurrentLocation.Y - character.CurrentLocation.Y);

				default:
					return new Number();
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
