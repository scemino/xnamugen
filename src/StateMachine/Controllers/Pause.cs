using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Pause")]
	internal class Pause : StateController
	{
		public Pause(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_cmdbuffertime = textsection.GetAttribute<Evaluation.Expression>("endcmdbuftime", null);
			m_movetime = textsection.GetAttribute<Evaluation.Expression>("movetime", null);
			m_pausebackground = textsection.GetAttribute<Evaluation.Expression>("pausebg", null);
		}

		public override void Run(Combat.Character character)
		{
			var time = EvaluationHelper.AsInt32(character, Time, null);
			var buffertime = EvaluationHelper.AsInt32(character, EndCommandBufferTime, 0);
			var movetime = EvaluationHelper.AsInt32(character, MoveTime, 0);
			var pausebg = EvaluationHelper.AsBoolean(character, PauseBackgrounds, true);

			if (time == null) return;

			character.Engine.Pause.Set(character, time.Value, buffertime, movetime, false, pausebg);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Time == null) return false;

			return true;
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression EndCommandBufferTime => m_cmdbuffertime;

		public Evaluation.Expression MoveTime => m_movetime;

		public Evaluation.Expression PauseBackgrounds => m_pausebackground;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_cmdbuffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_movetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pausebackground;

		#endregion
	}
}