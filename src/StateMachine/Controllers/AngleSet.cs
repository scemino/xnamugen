using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleSet")]
	class AngleSet : StateController
	{
		public AngleSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Single angle = EvaluationHelper.AsSingle(character, NewAngleValue, character.DrawingAngle);

			character.DrawingAngle = angle;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (NewAngleValue == null) return false;

			return true;
		}

		public Evaluation.Expression NewAngleValue
		{
			get { return m_angle; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_angle;

		#endregion
	}
}