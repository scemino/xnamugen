using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveReversed")]
	static class MoveReversed
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return (character.MoveType == xnaMugen.MoveType.Attack) ? character.OffensiveInfo.MoveReversed : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
