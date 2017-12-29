using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleSet")]
	internal class AngleSet : StateController
	{
		public AngleSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var angle = EvaluationHelper.AsSingle(character, NewAngleValue, character.DrawingAngle);

			character.DrawingAngle = angle;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (NewAngleValue == null) return false;

			return true;
		}

		public Evaluation.Expression NewAngleValue => m_angle;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_angle;

		#endregion
	}
}