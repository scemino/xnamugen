using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AttackDist")]
	class AttackDist: StateController
	{
		public AttackDist(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_distance = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? distance = EvaluationHelper.AsInt32(character, GuardDistance, null);

			if (distance != null && character.OffensiveInfo.ActiveHitDef == true)
			{
				character.OffensiveInfo.HitDef.GuardDistance = distance.Value;
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (GuardDistance == null) return false;

			return true;
		}

		public Evaluation.Expression GuardDistance
		{
			get { return m_distance; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_distance;

		#endregion
	}
}
