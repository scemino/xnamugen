using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ParentDist")]
	static class ParentDist
	{
		public static Number Evaluate(Object state, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return new Number();

			switch (axis)
			{
				case Axis.X:
					Single distance = Math.Abs(helper.CurrentLocation.X - helper.Parent.CurrentLocation.X);
					if (helper.CurrentFacing == xnaMugen.Facing.Right)
					{
						return (helper.Parent.CurrentLocation.X >= helper.CurrentLocation.X) ? new Number(distance) : new Number(-distance);
					}
					else
					{
						return (helper.Parent.CurrentLocation.X >= helper.CurrentLocation.X) ? new Number(-distance) : new Number(distance);
					}

				case Axis.Y:
					return new Number(helper.Parent.CurrentLocation.Y - helper.CurrentLocation.Y);

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
