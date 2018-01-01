using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("StopSnd")]
	internal class StopSound : StateController
	{
		public StopSound(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_channel = textsection.GetAttribute<Evaluation.Expression>("channel", null);
		}

		public override void Run(Combat.Character character)
		{
			var channelnumber = EvaluationHelper.AsInt32(character, Channel, null);
			if (channelnumber == null) return;

            if (channelnumber == -1)
            {
                StateSystem.GetSubSystem<Audio.SoundSystem>().StopAllSounds();
            }
            else
            {
                character.SoundManager.Stop(channelnumber.Value);
            }
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Channel == null) return false;

			return true;
		}

		public Evaluation.Expression Channel => m_channel;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_channel;

		#endregion
	}
}