using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ForceFeedback")]
	internal class ForceFeedback : StateController
	{
		public ForceFeedback(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_rumbletype = textsection.GetAttribute("waveform", ForceFeedbackType.Sine);
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_freq = textsection.GetAttribute<Evaluation.Expression>("freq", null);
			m_ampl = textsection.GetAttribute<Evaluation.Expression>("ampl", null);
			m_self = textsection.GetAttribute<Evaluation.Expression>("self", null);
		}

		public override void Run(Combat.Character character)
		{
		}

		public ForceFeedbackType RumbleType => m_rumbletype;

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression Frequency => m_freq;

		public Evaluation.Expression Amplitude => m_ampl;

		public Evaluation.Expression SelfRumble => m_self;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ForceFeedbackType m_rumbletype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_freq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_ampl;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_self;

		#endregion
	}
}