using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Width")]
	class Width : StateController
	{
		public Width(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_edge = textsection.GetAttribute<Evaluation.Expression>("edge", null);
			m_player = textsection.GetAttribute<Evaluation.Expression>("player", null);

			Evaluation.Expression exp_value = textsection.GetAttribute<Evaluation.Expression>("value", null);
			if (exp_value != null)
			{
				m_edge = exp_value;
				m_player = exp_value;
			}
		}

		public override void Run(Combat.Character character)
		{
			Point? playerwidth = EvaluationHelper.AsPoint(character, PlayerWidth, null);
			if (playerwidth != null)
			{
				character.Dimensions.SetOverride(playerwidth.Value.X, playerwidth.Value.Y);
			}

			Point? edgewidth = EvaluationHelper.AsPoint(character, EdgeWidth, null);
			if (edgewidth != null)
			{
				character.Dimensions.SetEdgeOverride(edgewidth.Value.X, edgewidth.Value.Y);
			}
		}

		public Evaluation.Expression EdgeWidth
		{
			get { return m_edge; }
		}

		public Evaluation.Expression PlayerWidth
		{
			get { return m_player; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_edge;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_player;

		#endregion
	}
}