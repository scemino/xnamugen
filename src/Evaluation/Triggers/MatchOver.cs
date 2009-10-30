using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MatchOver")]
	static class MatchOver
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Engine.IsMatchOver());
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
