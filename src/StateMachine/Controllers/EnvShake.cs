using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("EnvShake")]
	internal class EnvShake : StateController
	{
		public EnvShake(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("Time", null);
			m_freq = textsection.GetAttribute<Evaluation.Expression>("freq", null);
			m_amplitude = textsection.GetAttribute<Evaluation.Expression>("ampl", null);
			m_phaseoffset = textsection.GetAttribute<Evaluation.Expression>("phase", null);
		}

		public override void Run(Combat.Character character)
		{
			var time = EvaluationHelper.AsInt32(character, Time, null);
			var frequency = Misc.Clamp(EvaluationHelper.AsSingle(character, Frequency, 60), 0, 180);
			var amplitude = EvaluationHelper.AsInt32(character, Amplitude, -4);
			var phase = EvaluationHelper.AsSingle(character, PhaseOffset, frequency >= 90 ? 0 : 90);

			if (time == null) return;

			var envshake = character.Engine.EnvironmentShake;
			envshake.Set(time.Value, frequency, amplitude, phase);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Time == null) return false;

			return true;
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression Frequency => m_freq;

		public Evaluation.Expression Amplitude => m_amplitude;

		public Evaluation.Expression PhaseOffset => m_phaseoffset;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_freq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_amplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_phaseoffset;

		#endregion
	}
}