using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("SndPan")]
	internal class SndPan : StateController
	{
		public SndPan(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_channel = textsection.GetAttribute<Evaluation.Expression>("channel", null);
			m_pan = textsection.GetAttribute<Evaluation.Expression>("pan", null);
			m_abspan = textsection.GetAttribute<Evaluation.Expression>("abspan", null);
		}

		public override void Run(Combat.Character character)
		{
			var channel = EvaluationHelper.AsInt32(character, Channel, null);
			var pan = EvaluationHelper.AsInt32(character, Pan, null);
			var abspan = EvaluationHelper.AsInt32(character, AbsolutePan, null);

			if (channel == null) return;

			if (pan != null) character.SoundManager.RelativePan(channel.Value, pan.Value);
			if (abspan != null) character.SoundManager.AbsolutePan(channel.Value, abspan.Value);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Channel == null) return false;
			if (Pan != null && AbsolutePan != null) return false;

			return true;
		}

		public Evaluation.Expression Channel => m_channel;

		public Evaluation.Expression Pan => m_pan;

		public Evaluation.Expression AbsolutePan => m_abspan;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_channel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pan;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_abspan;

		#endregion
	}
}