using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AppendToClipboard")]
	internal class AppendToClipboard : DisplayToClipboard
	{
		public AppendToClipboard(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			if (Parameters != null)
			{
				var result = Parameters.Evaluate(character);
				if (result == null) return;

				character.Clipboard.Append(BuildString(result));
			}
			else
			{
				character.Clipboard.Append(FormatString);
			}
		}
	}
}