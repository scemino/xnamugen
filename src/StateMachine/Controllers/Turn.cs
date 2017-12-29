using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Turn")]
	internal class Turn : StateController
	{
		public Turn(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.CurrentFacing = Misc.FlipFacing(character.CurrentFacing);
		}
	}
}