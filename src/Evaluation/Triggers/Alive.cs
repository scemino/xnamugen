using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Alive")]
	internal static class Alive
	{
        public static bool Evaluate(Character character, ref bool error)
		{
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