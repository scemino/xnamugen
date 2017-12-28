using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("FallEnvShake")]
	class FallEnvShake : StateController
	{
		public FallEnvShake(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			Combat.HitDefinition hitdef = character.DefensiveInfo.HitDef;

			if (hitdef.EnvShakeFallTime == 0) return;

			Combat.EnvironmentShake envshake = character.Engine.EnvironmentShake;
			envshake.Set(hitdef.EnvShakeFallTime, hitdef.EnvShakeFallFrequency, hitdef.EnvShakeAmplitude, hitdef.EnvShakeFallPhase);

			hitdef.EnvShakeFallTime = 0;
		}
	}
}