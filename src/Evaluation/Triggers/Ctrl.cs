using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ctrl")]
	internal static class Ctrl
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.PlayerControl == PlayerControl.InControl;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
