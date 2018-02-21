using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("LifeMax")]
	internal static class LifeMax
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.MaximumLife;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}