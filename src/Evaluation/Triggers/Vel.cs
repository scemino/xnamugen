using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Vel")]
	static class Vel
	{
		public static Number Evaluate(Object state, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (axis)
			{
				case Axis.X:
					return new Number(character.CurrentVelocity.X);

				case Axis.Y:
					return new Number(character.CurrentVelocity.Y);

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
