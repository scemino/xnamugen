using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ID")]
	static class ID
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Id);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
