using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PlaySnd")]
	class PlaySnd : StateController
	{
		public PlaySnd(StateSystem statesystem, String label, TextSection textsection)
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
			SoundId? soundid = EvaluationHelper.AsSoundId(character, SoundId, null);
			Int32 volume = EvaluationHelper.AsInt32(character, Volume, 0);
			Int32 channelindex = EvaluationHelper.AsInt32(character, ChannelNumber, -1);
			Boolean priority = EvaluationHelper.AsBoolean(character, ChannelPriority, false);
			Single frequencymultiplier = EvaluationHelper.AsSingle(character, FrequencyMultiplier, 1.0f);
			Boolean loop = EvaluationHelper.AsBoolean(character, LoopSound, false);
			Int32? pan = EvaluationHelper.AsInt32(character, PanSound, null);
			Int32? abspan = EvaluationHelper.AsInt32(character, PanSoundAbsolute, null);

			if (soundid == null) return;

			Audio.SoundManager soundmanager = SoundId.IsCommon(false) ? character.Engine.CommonSounds : character.SoundManager;

			Audio.Channel channel = soundmanager.Play(channelindex, soundid.Value, priority, volume, frequencymultiplier, loop);
			if (channel != null && pan != null) channel.RelativePan(pan.Value);
			if (channel != null && abspan != null) channel.AbsolutePan(abspan.Value);
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (SoundId == null) return false;
			if (PanSound != null && PanSoundAbsolute != null) return false;

			return true;
		}

		public Evaluation.PrefixedExpression SoundId
		{
			get { return m_soundid; }
		}

		public Evaluation.Expression Volume
		{
			get { return m_volume; }
		}

		public Evaluation.Expression ChannelNumber
		{
			get { return m_channel; }
		}

		public Evaluation.Expression ChannelPriority
		{
			get { return m_channelpriority; }
		}

		public Evaluation.Expression FrequencyMultiplier
		{
			get { return m_freqmul; }
		}

		public Evaluation.Expression LoopSound
		{
			get { return m_loop; }
		}

		public Evaluation.Expression PanSound
		{
			get { return m_pan; }
		}

		public Evaluation.Expression PanSoundAbsolute
		{
			get { return m_abspan; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_soundid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_volume;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_channel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_channelpriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_freqmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_loop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pan;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_abspan;

		#endregion
	}
}