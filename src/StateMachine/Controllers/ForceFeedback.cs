using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ForceFeedback")]
	class ForceFeedback : StateController
	{
		public ForceFeedback(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_rumbletype = textsection.GetAttribute<ForceFeedbackType>("waveform", ForceFeedbackType.Sine);
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_freq = textsection.GetAttribute<Evaluation.Expression>("freq", null);
			m_ampl = textsection.GetAttribute<Evaluation.Expression>("ampl", null);
			m_self = textsection.GetAttribute<Evaluation.Expression>("self", null);
		}

		public override void Run(Combat.Character character)
		{
		}

		public ForceFeedbackType RumbleType
		{
			get { return m_rumbletype; }
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
			get { return m_ampl; }
		}

		public Evaluation.Expression SelfRumble
		{
			get { return m_self; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ForceFeedbackType m_rumbletype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_freq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_ampl;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_self;

		#endregion
	}
}