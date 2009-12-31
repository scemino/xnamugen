using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Alive")]
	static class Alive
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Life > 0;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}