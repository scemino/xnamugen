using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetDrop")]
	class TargetDrop : StateController
	{
		public TargetDrop(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_id = textsection.GetAttribute<Evaluation.Expression>("excludeID", null);
			m_keepone = textsection.GetAttribute<Evaluation.Expression>("keepone", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? exclude_id = EvaluationHelper.AsInt32(character, Id, null);
			Boolean keepone = EvaluationHelper.AsBoolean(character, KeepOne, true);

			Boolean keptone = false;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, null);
				if (target == null) continue;

				if (exclude_id != -1 && target.DefensiveInfo.HitDef.TargetId == exclude_id) continue;

				if (keptone == false)
				{
					keptone = true;
					continue;
				}

				character.OffensiveInfo.TargetList.Remove(target);
			}
		}

		public Evaluation.Expression Id
		{
			get { return m_id; }
		}

		public Evaluation.Expression KeepOne
		{
			get { return m_keepone; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_keepone;

		#endregion
	}
}