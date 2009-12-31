using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MatchOver")]
	static class MatchOver
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Engine.IsMatchOver();
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
