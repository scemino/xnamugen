using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitVel")]
	static class HitVel
	{
		public static Number Evaluate(Object state, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (axis)
			{
				case Axis.X:
					return new Number(character.DefensiveInfo.GetHitVelocity().X);

				case Axis.Y:
					return new Number(character.DefensiveInfo.GetHitVelocity().Y);

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
