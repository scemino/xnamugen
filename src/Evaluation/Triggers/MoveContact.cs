using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveContact")]
	static class MoveContact
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (character.MoveType == xnaMugen.MoveType.Attack)
			{
				return new Number(character.OffensiveInfo.MoveContact);
			}
			else
			{
				return new Number(0);
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
