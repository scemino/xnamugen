using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PlaySnd")]
	internal class PlaySnd : StateController
	{
		public PlaySnd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_soundid = textsection.GetAttribute<Evaluation.PrefixedExpression>("value", null);
			m_volume = textsection.GetAttribute<Evaluation.Expression>("volume", null);
			m_channel = textsection.GetAttribute<Evaluation.Expression>("channel", null);
			m_channelpriority = textsection.GetAttribute<Evaluation.Expression>("lowpriority", null);
			m_freqmul = textsection.GetAttribute<Evaluation.Expression>("freqmul", null);
			m_loop = textsection.GetAttribute<Evaluation.Expression>("loop", null);
			m_pan = textsection.GetAttribute<Evaluation.Expression>("pan", null);
			m_abspan = textsection.GetAttribute<Evaluation.Expression>("abspan", null);
		}

		public override void Run(Combat.Character character)
		{
			var soundid = EvaluationHelper.AsSoundId(character, SoundId, null);
			var volume = EvaluationHelper.AsInt32(character, Volume, 0);
			var channelindex = EvaluationHelper.AsInt32(character, ChannelNumber, -1);
			var priority = EvaluationHelper.AsBoolean(character, ChannelPriority, false);
			var frequencymultiplier = EvaluationHelper.AsSingle(character, FrequencyMultiplier, 1.0f);
			var loop = EvaluationHelper.AsBoolean(character, LoopSound, false);
			var pan = EvaluationHelper.AsInt32(character, PanSound, null);
			var abspan = EvaluationHelper.AsInt32(character, PanSoundAbsolute, null);

			if (soundid == null) return;

			//Audio.SoundManager soundmanager = SoundId.IsCommon(false) ? character.Engine.CommonSounds : character.SoundManager;

			//Audio.Channel channel = soundmanager.Play(channelindex, soundid.Value, priority, volume, frequencymultiplier, loop);
			//if (channel != null && pan != null) channel.RelativePan(pan.Value);
			//if (channel != null && abspan != null) channel.AbsolutePan(abspan.Value);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (SoundId == null) return false;
			if (PanSound != null && PanSoundAbsolute != null) return false;

			return true;
		}

		public Evaluation.PrefixedExpression SoundId => m_soundid;

		public Evaluation.Expression Volume => m_volume;

		public Evaluation.Expression ChannelNumber => m_channel;

		public Evaluation.Expression ChannelPriority => m_channelpriority;

		public Evaluation.Expression FrequencyMultiplier => m_freqmul;

		public Evaluation.Expression LoopSound => m_loop;

		public Evaluation.Expression PanSound => m_pan;

		public Evaluation.Expression PanSoundAbsolute => m_abspan;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_soundid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_volume;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_channel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_channelpriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_freqmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_loop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pan;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_abspan;

		#endregion
	}
}