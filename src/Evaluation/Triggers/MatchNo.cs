using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MatchNo")]
	static class MatchNo
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

#warning Not really implimented. Just a quick fix.
			return new Number(1);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
