using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("SelfState")]
	internal class SelfState : StateController
	{
		public SelfState(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_statenumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_control = textsection.GetAttribute<Evaluation.Expression>("ctrl", null);
			m_animationnumber = textsection.GetAttribute<Evaluation.Expression>("anim", null);
		}

		public override void Run(Combat.Character character)
		{
			var statenumber = EvaluationHelper.AsInt32(character, StateNumber, null);
			var playercontrol = EvaluationHelper.AsBoolean(character, Control, null);
			var animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, null);

			if (statenumber == null) return;

			character.StateManager.ForeignManager = null;
			character.StateManager.ChangeState(statenumber.Value);

			if (playercontrol != null)
			{
				if (playercontrol == true) character.PlayerControl = PlayerControl.InControl;
				if (playercontrol == false) character.PlayerControl = PlayerControl.NoControl;
			}

			if (animationnumber != null) character.SetLocalAnimation(animationnumber.Value, 0);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (StateNumber == null) return false;

			return true;
		}

		public Evaluation.Expression StateNumber => m_statenumber;

		public Evaluation.Expression Control => m_control;

		public Evaluation.Expression AnimationNumber => m_animationnumber;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_control;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_animationnumber;

		#endregion
	}
}