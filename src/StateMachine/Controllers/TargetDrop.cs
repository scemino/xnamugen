using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;

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
			Int32 exclude_id = EvaluationHelper.AsInt32(character, Id, -1);
			Boolean keepone = EvaluationHelper.AsBoolean(character, KeepOne, true);


			List<Combat.Character> removelist = new List<Combat.Character>();
			foreach (Combat.Character target in character.GetTargets(Int32.MinValue))
			{
				if (exclude_id != -1 && target.DefensiveInfo.HitDef.TargetId == exclude_id) continue;

				removelist.Add(target);
			}

			if (removelist.Count > 0 && keepone == true) removelist.RemoveAt(0);

			foreach (Combat.Character target in removelist) character.OffensiveInfo.TargetList.Remove(target);

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