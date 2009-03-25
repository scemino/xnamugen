using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("DrawGame")]
	class DrawGame : Function
	{
		public DrawGame(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}
		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Engine.Team1.VictoryStatus.Lose && character.Engine.Team2.VictoryStatus.Lose);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
