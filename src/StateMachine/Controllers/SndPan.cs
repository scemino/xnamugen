using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("SndPan")]
	class SndPan : StateController
	{
		public SndPan(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_channel = textsection.GetAttribute<Evaluation.Expression>("channel", null);
			m_pan = textsection.GetAttribute<Evaluation.Expression>("pan", null);
			m_abspan = textsection.GetAttribute<Evaluation.Expression>("abspan", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? channel = EvaluationHelper.AsInt32(character, Channel, null);
			Int32? pan = EvaluationHelper.AsInt32(character, Pan, null);
			Int32? abspan = EvaluationHelper.AsInt32(character, AbsolutePan, null);

			if (channel == null) return;

			if (pan != null) character.SoundManager.RelativePan(channel.Value, pan.Value);
			if (abspan != null) character.SoundManager.AbsolutePan(channel.Value, abspan.Value);
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Channel == null) return false;
			if (Pan != null && AbsolutePan != null) return false;

			return true;
		}

		public Evaluation.Expression Channel
		{
			get { return m_channel; }
		}

		public Evaluation.Expression Pan
		{
			get { return m_pan; }
		}

		public Evaluation.Expression AbsolutePan
		{
			get { return m_abspan; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_channel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pan;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_abspan;

		#endregion
	}
}