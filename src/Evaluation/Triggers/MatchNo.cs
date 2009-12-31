using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MatchNo")]
	static class MatchNo
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

#warning Not really implimented. Just a quick fix.
			return 1;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
