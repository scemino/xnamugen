using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("FallEnvShake")]
	internal class FallEnvShake : StateController
	{
		public FallEnvShake(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			var hitdef = character.DefensiveInfo.HitDef;

			if (hitdef.EnvShakeFallTime == 0) return;

			var envshake = character.Engine.EnvironmentShake;
			envshake.Set(hitdef.EnvShakeFallTime, hitdef.EnvShakeFallFrequency, hitdef.EnvShakeAmplitude, hitdef.EnvShakeFallPhase);

			hitdef.EnvShakeFallTime = 0;
		}
	}
}