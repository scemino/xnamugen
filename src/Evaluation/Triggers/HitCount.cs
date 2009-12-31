using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitCount")]
	static class HitCount
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.OffensiveInfo.HitCount;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
