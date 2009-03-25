using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AppendToClipboard")]
	class AppendToClipboard : DisplayToClipboard
	{
		public AppendToClipboard(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			if (Parameters != null)
			{
				Evaluation.Result result = Parameters.Evaluate(character);
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