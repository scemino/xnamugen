using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ChangeAnim2")]
	internal class ChangeAnim2 : StateController
	{
		public ChangeAnim2(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_animationnumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_elementnumber = textsection.GetAttribute<Evaluation.Expression>("elem", null);
		}

		public override void Run(Combat.Character character)
		{
			var animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, null);
			var elementnumber = EvaluationHelper.AsInt32(character, ElementNumber, 0);

			if (animationnumber == null) return;
			if(character.StateManager.ForeignManager == null) return;

			--elementnumber;
			if (elementnumber < 0) elementnumber = 0;

			character.SetForeignAnimation(character.StateManager.ForeignManager.Character.AnimationManager, animationnumber.Value, elementnumber);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AnimationNumber == null) return false;

			return true;
		}

		public Evaluation.Expression AnimationNumber => m_animationnumber;

		public Evaluation.Expression ElementNumber => m_elementnumber;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_elementnumber;

		#endregion
	}
}