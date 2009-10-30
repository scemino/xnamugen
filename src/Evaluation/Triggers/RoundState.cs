using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("RoundState")]
	static class RoundState
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (character.Engine.RoundState)
			{
				case xnaMugen.RoundState.PreIntro:
					return new Number(0);
				
				case xnaMugen.RoundState.Intro:
					return new Number(1);
			
				case xnaMugen.RoundState.Fight:
					return new Number(2);

				case xnaMugen.RoundState.PreOver:
					return new Number(3);

				case xnaMugen.RoundState.Over:
					return new Number(4);

				default:
					return new Number(-1);
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
