using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Trans")]
	internal class Trans : StateController
	{
		public Trans(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_blending = textsection.GetAttribute("trans", new Blending());
			m_alpha = textsection.GetAttribute<Evaluation.Expression>("alpha", null);
		}

		public override void Run(Combat.Character character)
		{
			var alpha = EvaluationHelper.AsPoint(character, Alpha, new Point(255, 0));

			if (Transparency.BlendType == BlendType.Add && Transparency.SourceFactor == 0 && Transparency.DestinationFactor == 0)
			{
				character.Transparency = new Blending(BlendType.Add, alpha.X, alpha.Y);
			}
			else
			{
				character.Transparency = Transparency;
			}
		}

		public Blending Transparency => m_blending;

		public Evaluation.Expression Alpha => m_alpha;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_alpha;

		#endregion
	}
}
