using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ClearClipboard")]
	internal class ClearClipboard: StateController
	{
		public ClearClipboard(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.Clipboard.Length = 0;
		}
	}
}