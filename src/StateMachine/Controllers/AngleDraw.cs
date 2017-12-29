using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleDraw")]
	internal class AngleDraw : StateController
	{
		public AngleDraw(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_scale = textsection.GetAttribute<Evaluation.Expression>("scale", null);
		}


		public override void Run(Combat.Character character)
		{
			var angle = EvaluationHelper.AsSingle(character, Angle, character.DrawingAngle);

			character.DrawingAngle = angle;
			character.AngleDraw = true;

			var scale = EvaluationHelper.AsVector2(character, Scale, null);
			if (scale != null) character.DrawScale = scale.Value;
		}

		public Evaluation.Expression Angle => m_angle;

		public Evaluation.Expression Scale => m_scale;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_angle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_scale;

		#endregion
	}
}