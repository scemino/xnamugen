using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ChangeAnim")]
	class ChangeAnim : StateController
	{
		public ChangeAnim(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_animationnumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_elementnumber = textsection.GetAttribute<Evaluation.Expression>("elem", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, null);
			Int32 elementnumber = EvaluationHelper.AsInt32(character, ElementNumber, 0);

			if (animationnumber == null) return;

			--elementnumber;
			if (elementnumber < 0) elementnumber = 0;

			character.SetLocalAnimation(animationnumber.Value, elementnumber);
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AnimationNumber == null) return false;

			return true;
		}

		public Evaluation.Expression AnimationNumber
		{
			get { return m_animationnumber; }
		}

		public Evaluation.Expression ElementNumber
		{
			get { return m_elementnumber; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_elementnumber;

		#endregion
	}
}