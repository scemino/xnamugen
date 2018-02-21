using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Random")]
	internal static class Random
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var rng = character.Engine.GetSubSystem<xnaMugen.Random>();
			return rng.NewInt(0, 999);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
