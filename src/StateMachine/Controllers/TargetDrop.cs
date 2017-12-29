using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetDrop")]
	internal class TargetDrop : StateController
	{
		public TargetDrop(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_id = textsection.GetAttribute<Evaluation.Expression>("excludeID", null);
			m_keepone = textsection.GetAttribute<Evaluation.Expression>("keepone", null);
		}

		public override void Run(Combat.Character character)
		{
			var excludeId = EvaluationHelper.AsInt32(character, Id, -1);
			var keepone = EvaluationHelper.AsBoolean(character, KeepOne, true);


			var removelist = new List<Combat.Character>();
			foreach (var target in character.GetTargets(int.MinValue))
			{
				if (excludeId != -1 && target.DefensiveInfo.HitDef.TargetId == excludeId) continue;

				removelist.Add(target);
			}

			if (removelist.Count > 0 && keepone) removelist.RemoveAt(0);

			foreach (var target in removelist) character.OffensiveInfo.TargetList.Remove(target);

			/*
			Boolean keptone = false;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, Int32.MinValue);
				if (target == null) continue;

				if (exclude_id != -1 && target.DefensiveInfo.HitDef.TargetId == exclude_id) continue;

				if (keptone == false)
				{
					keptone = true;
					continue;
				}

				character.OffensiveInfo.TargetList.Remove(target);
			}
			*/
		}

		public Evaluation.Expression Id => m_id;

		public Evaluation.Expression KeepOne => m_keepone;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_keepone;

		#endregion
	}
}