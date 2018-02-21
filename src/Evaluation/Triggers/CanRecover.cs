using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("CanRecover")]
	internal static class CanRecover
	{
        public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.IsFalling == false ? false : character.DefensiveInfo.HitDef.FallCanRecover;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
