using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Random")]
	static class Random
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			xnaMugen.Random rng = character.Engine.GetSubSystem<xnaMugen.Random>();
			return new Number(rng.NewInt(0, 999));
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
