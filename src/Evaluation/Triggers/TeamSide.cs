using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TeamSide")]
	class TeamSide : Function
	{
		public TeamSide(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (character.Team.Side)
			{
				case xnaMugen.TeamSide.Left:
					return new Number(1);

				case xnaMugen.TeamSide.Right:
					return new Number(2);

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
