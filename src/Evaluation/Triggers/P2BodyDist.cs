using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2BodyDist")]
	class P2BodyDist : Function
	{
		public P2BodyDist(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 1) return new Number();

			Combat.Player opponent = character.GetOpponent();
			if (opponent == null) return new Number();

			Axis axis = (Axis)Arguments[0];

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
