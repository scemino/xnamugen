using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TicksPerSecond")]
	static class TicksPerSecond
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

#warning Hack
			return 60;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}