using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Alive")]
	static class Alive
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Life > 0);

		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}