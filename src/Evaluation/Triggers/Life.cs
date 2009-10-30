using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Life")]
	static class Life
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if(character == null) return new Number();

			return new Number(character.Life);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
