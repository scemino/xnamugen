using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetVelAdd")]
	internal class TargetVelAdd : StateController
	{
		public TargetVelAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			var x = EvaluationHelper.AsSingle(character, X, null);
			var y = EvaluationHelper.AsSingle(character, Y, null);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);

			foreach (var target in character.GetTargets(targetId))
			{
				var velocity = new Vector2(0, 0);

				if (x != null) velocity.X = x.Value;
				if (y != null) velocity.Y = y.Value;

				target.CurrentVelocity += velocity;
			}
		}

		public Evaluation.Expression TargetId => m_targetid;

		public Evaluation.Expression X => m_x;

		public Evaluation.Expression Y => m_y;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_y;

		#endregion
	}
}