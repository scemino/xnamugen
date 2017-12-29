using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("DestroySelf")]
	internal class DestroySelf : StateController
	{
		public DestroySelf(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			var helper = character as Combat.Helper;
			if (helper != null) helper.Remove();
		}
	}
}