using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleAdd")]
	internal class AngleAdd : StateController
	{
		public AngleAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var angle = EvaluationHelper.AsSingle(character, AngleAddition, 0);

			character.DrawingAngle += angle;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AngleAddition == null) return false;

			return true;
		}

		public Evaluation.Expression AngleAddition => m_angle;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_angle;

		#endregion
	}
}