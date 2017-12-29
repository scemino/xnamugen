using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("SprPriority")]
	internal class SprPriority : StateController
	{
		public SprPriority(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_priority = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var spritepriority = EvaluationHelper.AsInt32(character, Priority, 0);
			spritepriority = Misc.Clamp(spritepriority, -5, 5);

			character.DrawOrder = spritepriority;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Priority == null) return false;

			return true;
		}

		public Evaluation.Expression Priority => m_priority;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_priority;

		#endregion
	}
}