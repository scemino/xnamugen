using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("SuperPause")]
	internal class SuperPause : StateController
	{
		public SuperPause(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_cmdbuffertime = textsection.GetAttribute<Evaluation.Expression>("endcmdbuftime", null);
			m_movetime = textsection.GetAttribute<Evaluation.Expression>("movetime", null);
			m_pausebackground = textsection.GetAttribute<Evaluation.Expression>("pausebg", null);
			m_sparknumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("anim", null);
			m_soundid = textsection.GetAttribute<Evaluation.PrefixedExpression>("sound", null);
			m_pausebackground = textsection.GetAttribute<Evaluation.Expression>("pausebg", null);
			m_animposition = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_darken = textsection.GetAttribute<Evaluation.Expression>("darken", null);
			m_p2defmul = textsection.GetAttribute<Evaluation.Expression>("p2defmul", null);
			m_poweradd = textsection.GetAttribute<Evaluation.Expression>("poweradd", null);
			m_unhittable = textsection.GetAttribute<Evaluation.Expression>("unhittable", null);
		}

		public override void Run(Combat.Character character)
		{
			int? time = EvaluationHelper.AsInt32(character, Time, 30);
			var buffertime = EvaluationHelper.AsInt32(character, EndCommandBufferTime, 0);
			var movetime = EvaluationHelper.AsInt32(character, MoveTime, 0);
			var pausebg = EvaluationHelper.AsBoolean(character, PauseBackgrounds, true);
			var power = EvaluationHelper.AsInt32(character, PowerAdd, 0);

#warning Documentation states that default should be 30. Testing looks to be 100.
			var animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, 100);

			var soundid = EvaluationHelper.AsSoundId(character, SoundId, null);
			var animationposition = EvaluationHelper.AsPoint(character, AnimationPosition, new Point(0, 0));
			var darkenscreen = EvaluationHelper.AsBoolean(character, DarkenScreen, true);
			var p2defmul = EvaluationHelper.AsSingle(character, P2DefenseMultiplier, null);
			var unhittable = EvaluationHelper.AsBoolean(character, Unhittable, true);

			if (time == null) return;

			var pause = character.Engine.SuperPause;
			pause.Set(character, time.Value, buffertime, movetime, false, pausebg);

			character.BasePlayer.Power += power;

			var data = new Combat.ExplodData();
			data.PositionType = PositionType.P1;
			data.Location = (Vector2)animationposition;
			data.RemoveTime = -2;
			data.CommonAnimation = EvaluationHelper.IsCommon(AnimationNumber, true);
			data.AnimationNumber = animationnumber;
			data.Scale = Vector2.One;
			data.SuperMove = true;
			data.Creator = character;
			data.Offseter = character;
            data.DrawOnTop = true;

			var explod = new Combat.Explod(character.Engine, data);
			if(explod.IsValid) explod.Engine.Entities.Add(explod);

			if (soundid != null)
			{
				Audio.SoundManager soundmanager = SoundId.IsCommon(true) ? character.Engine.CommonSounds : character.SoundManager;
				soundmanager.Play(soundid.Value);
			}
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression EndCommandBufferTime => m_cmdbuffertime;

		public Evaluation.Expression MoveTime => m_movetime;

		public Evaluation.Expression PauseBackgrounds => m_pausebackground;

		public Evaluation.PrefixedExpression AnimationNumber => m_sparknumber;

		public Evaluation.PrefixedExpression SoundId => m_soundid;

		public Evaluation.Expression AnimationPosition => m_animposition;

		public Evaluation.Expression DarkenScreen => m_darken;

		public Evaluation.Expression P2DefenseMultiplier => m_p2defmul;

		public Evaluation.Expression PowerAdd => m_poweradd;

		public Evaluation.Expression Unhittable => m_unhittable;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_cmdbuffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_movetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pausebackground;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_sparknumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_soundid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_animposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_darken;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p2defmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_poweradd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_unhittable;

		#endregion
	}
}