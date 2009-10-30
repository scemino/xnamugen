using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Facing")]
	static class Facing
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (character.CurrentFacing)
			{
				case xnaMugen.Facing.Left:
					return new Number(-1);

				case xnaMugen.Facing.Right:
					return new Number(1);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
