using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("BindToParent")]
	class BindToParent: StateController
	{
		public BindToParent(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_facing = textsection.GetAttribute<Evaluation.Expression>("facing", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
		}

		public override void Run(Combat.Character character)
		{
			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return;

			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);
			Int32 facing = EvaluationHelper.AsInt32(character, Facing, 0);
			Vector2 offset = EvaluationHelper.AsVector2(character, Position, new Vector2(0, 0));

			helper.Bind.Set(helper.Parent, offset, time, facing, false);
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression Facing
		{
			get { return m_facing; }
		}

		public Evaluation.Expression Position
		{
			get { return m_position; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_position;

		#endregion
	}
}