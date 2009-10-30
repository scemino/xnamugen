using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("RoundNo")]
	static class RoundNo
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Engine.RoundNumber);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
