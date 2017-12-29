using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ScreenBound")]
	internal class ScreenBound : StateController
	{
		public ScreenBound(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_boundflag = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_movecamera = textsection.GetAttribute<Evaluation.Expression>("movecamera", null);
		}

		public override void Run(Combat.Character character)
		{
			var boundflag = EvaluationHelper.AsBoolean(character, BoundFlag, false);
			var movecamera = EvaluationHelper.AsPoint(character, MoveCamera, new Point(0, 0));

			character.ScreenBound = boundflag;
			character.CameraFollowX = movecamera.X > 0;
			character.CameraFollowY = movecamera.Y > 0;
		}

		public Evaluation.Expression BoundFlag => m_boundflag;

		public Evaluation.Expression MoveCamera => m_movecamera;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_boundflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_movecamera;

		#endregion
	}
}