using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Facing")]
	class Facing : Function
	{
		public Facing(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}
		public override Number Evaluate(Object state)
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

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
