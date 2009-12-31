using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Vel")]
	static class Vel
	{
		public static Single Evaluate(Object state, ref Boolean error, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (axis)
			{
				case Axis.X:
					return character.CurrentVelocity.X;

				case Axis.Y:
					return character.CurrentVelocity.Y;

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
