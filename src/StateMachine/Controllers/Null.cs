using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Null")]
	internal class Null : StateController
	{
		public Null(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
		}
	}
}
