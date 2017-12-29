using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("DisplayToClipboard")]
	internal class DisplayToClipboard : StateController
	{
		public DisplayToClipboard(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_formatstring = textsection.GetAttribute<string>("text", null);
			m_params = textsection.GetAttribute<Evaluation.Expression>("params", null);
		}

		public override void Run(Combat.Character character)
		{
			if (Parameters != null)
			{
				character.Clipboard.Length = 0;
				character.Clipboard.Append(BuildString(Parameters.Evaluate(character)));
			}
			else
			{
				character.Clipboard.Length = 0;
				character.Clipboard.Append(FormatString);
			}
		}

		protected string BuildString(Evaluation.Number[] args)
		{
			if (args == null) throw new ArgumentNullException(nameof(args));

			return StateSystem.GetSubSystem<StringFormatter>().BuildString(FormatString, args);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (FormatString == null) return false;

			return true;
		}

		public string FormatString => m_formatstring;

		public Evaluation.Expression Parameters => m_params;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_formatstring;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_params;

		#endregion
	}
}