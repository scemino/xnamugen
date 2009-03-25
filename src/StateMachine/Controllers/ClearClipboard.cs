using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ClearClipboard")]
	class ClearClipboard: StateController
	{
		public ClearClipboard(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.Clipboard.Length = 0;
		}
	}
}