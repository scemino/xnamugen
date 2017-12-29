using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Offset")]
	internal class Offset : StateController
	{
		public Offset(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			var x = EvaluationHelper.AsSingle(character, X, null);
			var y = EvaluationHelper.AsSingle(character, Y, null);

			var offset = character.DrawOffset;

			if (x != null) offset.X = x.Value;
			if (y != null) offset.Y = y.Value;

			character.DrawOffset = offset;
		}

		public Evaluation.Expression X => m_x;

		public Evaluation.Expression Y => m_y;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_y;

		#endregion
	}
}