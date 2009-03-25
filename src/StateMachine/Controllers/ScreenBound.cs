using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ScreenBound")]
	class ScreenBound : StateController
	{
		public ScreenBound(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_boundflag = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_movecamera = textsection.GetAttribute<Evaluation.Expression>("movecamera", null);
		}

		public override void Run(Combat.Character character)
		{
			Boolean boundflag = EvaluationHelper.AsBoolean(character, BoundFlag, false);
			Point movecamera = EvaluationHelper.AsPoint(character, MoveCamera, new Point(0, 0));

			character.ScreenBound = boundflag;
			character.CameraFollowX = movecamera.X > 0;
			character.CameraFollowY = movecamera.Y > 0;
		}

		public Evaluation.Expression BoundFlag
		{
			get { return m_boundflag; }
		}

		public Evaluation.Expression MoveCamera
		{
			get { return m_movecamera; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_boundflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_movecamera;

		#endregion
	}
}