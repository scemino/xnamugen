using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitCount")]
	internal static class HitCount
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.OffensiveInfo.HitCount;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
