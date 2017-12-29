using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetBind")]
	internal class TargetBind : StateController
	{
		public TargetBind(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_pos = textsection.GetAttribute<Evaluation.Expression>("pos", null);
		}

		public override void Run(Combat.Character character)
		{
			var time = EvaluationHelper.AsInt32(character, Time, 1);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);
			var position = EvaluationHelper.AsVector2(character, Position, new Vector2(0, 0));

			foreach (var target in character.GetTargets(targetId))
			{
				target.Bind.Set(character, position, time, 0, true);
			}
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression TargetId => m_id;

		public Evaluation.Expression Position => m_pos;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pos;

		#endregion
	}
}