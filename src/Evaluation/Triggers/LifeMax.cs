using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("LifeMax")]
	static class LifeMax
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.BasePlayer.Constants.MaximumLife);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}