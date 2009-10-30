using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("CanRecover")]
	static class CanRecover
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (character.DefensiveInfo.IsFalling == false) return new Number(false);

			return new Number(character.DefensiveInfo.HitDef.FallCanRecover);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
