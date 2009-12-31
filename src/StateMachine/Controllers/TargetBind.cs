using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetBind")]
	class TargetBind : StateController
	{
		public TargetBind(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_pos = textsection.GetAttribute<Evaluation.Expression>("pos", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);
			Int32 target_id = EvaluationHelper.AsInt32(character, TargetId, Int32.MinValue);
			Vector2 position = EvaluationHelper.AsVector2(character, Position, new Vector2(0, 0));

			foreach (Combat.Character target in character.GetTargets(target_id))
			{
				target.Bind.Set(character, position, time, 0, true);
			}
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_id; }
		}

		public Evaluation.Expression Position
		{
			get { return m_pos; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pos;

		#endregion
	}
}