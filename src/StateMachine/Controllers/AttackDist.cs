using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AttackDist")]
	internal class AttackDist: StateController
	{
		public AttackDist(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_distance = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var distance = EvaluationHelper.AsInt32(character, GuardDistance, null);

			if (distance != null && character.OffensiveInfo.ActiveHitDef)
			{
				character.OffensiveInfo.HitDef.GuardDistance = distance.Value;
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (GuardDistance == null) return false;

			return true;
		}

		public Evaluation.Expression GuardDistance => m_distance;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_distance;

		#endregion
	}
}
