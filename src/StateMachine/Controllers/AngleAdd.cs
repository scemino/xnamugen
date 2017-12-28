using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleAdd")]
	class AngleAdd : StateController
	{
		public AngleAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Single angle = EvaluationHelper.AsSingle(character, AngleAddition, 0);

			character.DrawingAngle += angle;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AngleAddition == null) return false;

			return true;
		}

		public Evaluation.Expression AngleAddition
		{
			get { return m_angle; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_angle;

		#endregion
	}
}