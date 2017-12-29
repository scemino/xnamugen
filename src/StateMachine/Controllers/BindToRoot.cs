using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("BindToRoot")]
	internal class BindToRoot : StateController
	{
		public BindToRoot(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_facing = textsection.GetAttribute<Evaluation.Expression>("facing", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
		}

		public override void Run(Combat.Character character)
		{
			var helper = character as Combat.Helper;
			if (helper == null) return;

			var time = EvaluationHelper.AsInt32(character, Time, 1);
			var facing = EvaluationHelper.AsInt32(character, Facing, 0);
			var offset = EvaluationHelper.AsVector2(character, Position, new Vector2(0, 0));

			helper.Bind.Set(helper.BasePlayer, offset, time, facing, false);
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression Facing => m_facing;

		public Evaluation.Expression Position => m_position;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_position;

		#endregion
	}
}