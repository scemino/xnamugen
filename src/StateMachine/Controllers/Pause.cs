using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Pause")]
	class Pause : StateController
	{
		public Pause(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_cmdbuffertime = textsection.GetAttribute<Evaluation.Expression>("endcmdbuftime", null);
			m_movetime = textsection.GetAttribute<Evaluation.Expression>("movetime", null);
			m_pausebackground = textsection.GetAttribute<Evaluation.Expression>("pausebg", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? time = EvaluationHelper.AsInt32(character, Time, null);
			Int32 buffertime = EvaluationHelper.AsInt32(character, EndCommandBufferTime, 0);
			Int32 movetime = EvaluationHelper.AsInt32(character, MoveTime, 0);
			Boolean pausebg = EvaluationHelper.AsBoolean(character, PauseBackgrounds, true);

			if (time == null) return;

			character.Engine.Pause.Set(character, time.Value, buffertime, movetime, false, pausebg);
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

		public Evaluation.Expression EndCommandBufferTime
		{
			get { return m_cmdbuffertime; }
		}

		public Evaluation.Expression MoveTime
		{
			get { return m_movetime; }
		}

		public Evaluation.Expression PauseBackgrounds
		{
			get { return m_pausebackground; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_cmdbuffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_movetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pausebackground;

		#endregion
	}
}