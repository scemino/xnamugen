using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleMul")]
	internal class AngleMul : StateController
	{
		public AngleMul(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var angle = EvaluationHelper.AsSingle(character, AngleMultiplier, 1);

			character.DrawingAngle *= angle;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AngleMultiplier == null) return false;

			return true;
		}

		public Evaluation.Expression AngleMultiplier => m_angle;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_angle;

		#endregion
	}
}