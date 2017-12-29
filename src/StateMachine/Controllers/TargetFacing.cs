using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetFacing")]
	internal class TargetFacing : StateController
	{
		public TargetFacing(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_facing = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			var facing = EvaluationHelper.AsInt32(character, Facing, 0);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);

			foreach (var target in character.GetTargets(targetId))
			{
				if (facing > 0)
				{
					target.CurrentFacing = character.CurrentFacing;
				}
				else if (facing < 0)
				{
					target.CurrentFacing = Misc.FlipFacing(character.CurrentFacing);
				}
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Facing == null) return false;

			return true;
		}

		public Evaluation.Expression Facing => m_facing;

		public Evaluation.Expression TargetId => m_targetid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		#endregion
	}
}