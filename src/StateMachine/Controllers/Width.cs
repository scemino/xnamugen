using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Width")]
	internal class Width : StateController
	{
		public Width(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_edge = textsection.GetAttribute<Evaluation.Expression>("edge", null);
			m_player = textsection.GetAttribute<Evaluation.Expression>("player", null);

			var expValue = textsection.GetAttribute<Evaluation.Expression>("value", null);
			if (expValue != null)
			{
				m_edge = expValue;
				m_player = expValue;
			}
		}

		public override void Run(Combat.Character character)
		{
			var playerwidth = EvaluationHelper.AsPoint(character, PlayerWidth, null);
			if (playerwidth != null)
			{
				character.Dimensions.SetOverride(playerwidth.Value.X, playerwidth.Value.Y);
			}

			var edgewidth = EvaluationHelper.AsPoint(character, EdgeWidth, null);
			if (edgewidth != null)
			{
				character.Dimensions.SetEdgeOverride(edgewidth.Value.X, edgewidth.Value.Y);
			}
		}

		public Evaluation.Expression EdgeWidth => m_edge;

		public Evaluation.Expression PlayerWidth => m_player;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_edge;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_player;

		#endregion
	}
}