using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("RoundsExisted")]
	static class RoundsExisted
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.RoundsExisted;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
