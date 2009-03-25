using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Trans")]
	class Trans : StateController
	{
		public Trans(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_blending = textsection.GetAttribute<Blending>("trans", new Blending());
			m_alpha = textsection.GetAttribute<Evaluation.Expression>("alpha", null);
		}

		public override void Run(Combat.Character character)
		{
			Point alpha = EvaluationHelper.AsPoint(character, Alpha, new Point(255, 0));

			if (Transparency.BlendType == BlendType.Add && Transparency.SourceFactor == 0 && Transparency.DestinationFactor == 0)
			{
				character.Transparency = new Blending(BlendType.Add, alpha.X, alpha.Y);
			}
			else
			{
				character.Transparency = Transparency;
			}
		}

		public Blending Transparency
		{
			get { return m_blending; }
		}

		public Evaluation.Expression Alpha
		{
			get { return m_alpha; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_alpha;

		#endregion
	}
}
