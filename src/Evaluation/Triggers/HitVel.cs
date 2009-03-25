using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitVel")]
	class HitVel : Function
	{
		public HitVel(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}
		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 1) return new Number();

			Axis axis = (Axis)Arguments[0];

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
