using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Random")]
	static class Random
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			xnaMugen.Random rng = character.Engine.GetSubSystem<xnaMugen.Random>();
			return rng.NewInt(0, 999);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
