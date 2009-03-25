using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ChangeState")]
	class ChangeState : StateController
	{
		public ChangeState(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_statenumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_control = textsection.GetAttribute<Evaluation.Expression>("ctrl", null);
			m_animationnumber = textsection.GetAttribute<Evaluation.Expression>("anim", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? statenumber = EvaluationHelper.AsInt32(character, StateNumber, null);
			Boolean? playercontrol = EvaluationHelper.AsBoolean(character, Control, null);
			Int32? animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, null);

			if (statenumber == null) return;
			character.StateManager.ChangeState(statenumber.Value);

			if (playercontrol != null)
			{
				if (playercontrol == true) character.PlayerControl = PlayerControl.InControl;
				if (playercontrol == false) character.PlayerControl = PlayerControl.NoControl;
			}

			if (animationnumber != null) character.SetLocalAnimation(animationnumber.Value, 0);
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (StateNumber == null) return false;

			return true;
		}

		public Evaluation.Expression StateNumber
		{
			get { return m_statenumber; }
		}

		public Evaluation.Expression Control
		{
			get { return m_control; }
		}

		public Evaluation.Expression AnimationNumber
		{
			get { return m_animationnumber; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_control;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_animationnumber;

		#endregion
	}
}