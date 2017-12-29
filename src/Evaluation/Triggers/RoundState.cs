namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("RoundState")]
	internal static class RoundState
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (character.Engine.RoundState)
			{
				case xnaMugen.RoundState.PreIntro:
					return 0;
				
				case xnaMugen.RoundState.Intro:
					return 1;
			
				case xnaMugen.RoundState.Fight:
					return 2;

				case xnaMugen.RoundState.PreOver:
					return 3;

				case xnaMugen.RoundState.Over:
					return 4;

				default:
					return -1;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
