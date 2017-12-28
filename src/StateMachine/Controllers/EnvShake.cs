using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("EnvShake")]
	class EnvShake : StateController
	{
		public EnvShake(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("Time", null);
			m_freq = textsection.GetAttribute<Evaluation.Expression>("freq", null);
			m_amplitude = textsection.GetAttribute<Evaluation.Expression>("ampl", null);
			m_phaseoffset = textsection.GetAttribute<Evaluation.Expression>("phase", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? time = EvaluationHelper.AsInt32(character, Time, null);
			Single frequency = Misc.Clamp(EvaluationHelper.AsSingle(character, Frequency, 60), 0, 180);
			Int32 amplitude = EvaluationHelper.AsInt32(character, Amplitude, -4);
			Single phase = EvaluationHelper.AsSingle(character, PhaseOffset, frequency >= 90 ? 0 : 90);

			if (time == null) return;

			Combat.EnvironmentShake envshake = character.Engine.EnvironmentShake;
			envshake.Set(time.Value, frequency, amplitude, phase);
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Time == null) return false;

			return true;
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression Frequency
		{
			get { return m_freq; }
		}

		public Evaluation.Expression Amplitude
		{
			get { return m_amplitude; }
		}

		public Evaluation.Expression PhaseOffset
		{
			get { return m_phaseoffset; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_freq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_amplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_phaseoffset;

		#endregion
	}
}