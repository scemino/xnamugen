using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("CtrlSet")]
	class CtrlSet : StateController
	{
		public CtrlSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_control = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Boolean? control = EvaluationHelper.AsBoolean(character, Control, null);

			if (control != null)
			{
				if (control == true) character.PlayerControl = PlayerControl.InControl;
				if (control == false) character.PlayerControl = PlayerControl.NoControl;
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Control == null) return false;

			return true;
		}

		public Evaluation.Expression Control
		{
			get { return m_control; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_control;

		#endregion
	}
}