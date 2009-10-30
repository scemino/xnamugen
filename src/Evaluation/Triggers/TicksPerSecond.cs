using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TicksPerSecond")]
	static class TicksPerSecond
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

#warning Hack
			return new Number(60);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}