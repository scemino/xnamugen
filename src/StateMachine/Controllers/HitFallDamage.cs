using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitFallDamage")]
	internal class HitFallDamage : StateController
	{
		public HitFallDamage(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.Life -= character.DefensiveInfo.HitDef.FallDamage;
		}
	}
}