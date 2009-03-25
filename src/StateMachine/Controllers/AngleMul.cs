using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleMul")]
	class AngleMul : StateController
	{
		public AngleMul(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Single angle = EvaluationHelper.AsSingle(character, AngleMultiplier, 1);

			character.DrawingAngle *= angle;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AngleMultiplier == null) return false;

			return true;
		}

		public Evaluation.Expression AngleMultiplier
		{
			get { return m_angle; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_angle;

		#endregion
	}
}