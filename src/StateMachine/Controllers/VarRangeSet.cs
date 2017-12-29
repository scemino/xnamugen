using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VarRangeSet")]
	internal class VarRangeSet : StateController
	{
		public VarRangeSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_intnumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_floatnumber = textsection.GetAttribute<Evaluation.Expression>("fvalue", null);
			m_startrrange = textsection.GetAttribute<Evaluation.Expression>("first", null);
			m_endrange = textsection.GetAttribute<Evaluation.Expression>("last", null);
		}

		public override void Run(Combat.Character character)
		{
			var start = EvaluationHelper.AsInt32(character, StartRange, null);
			var end = EvaluationHelper.AsInt32(character, EndRange, null);

			if (IntNumber != null)
			{
				var value = EvaluationHelper.AsInt32(character, IntNumber, null);
				if (value != null)
				{
					for (var i = 0; i != character.Variables.IntegerVariables.Count; ++i)
					{
						if (i < start || i > end) continue;
						character.Variables.SetInteger(i, false, value.Value);
					}
				}
			}

			if (FloatNumber != null)
			{
				var value = EvaluationHelper.AsSingle(character, FloatNumber, null);
				if (value != null)
				{
					for (var i = 0; i != character.Variables.FloatVariables.Count; ++i)
					{
						if (i < start || i > end) continue;
						character.Variables.SetFloat(i, false, value.Value);
					}
				}
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if ((IntNumber != null ^ FloatNumber != null) == false) return false;

			return true;
		}

		public Evaluation.Expression IntNumber => m_intnumber;

		public Evaluation.Expression FloatNumber => m_floatnumber;

		public Evaluation.Expression StartRange => m_startrrange;

		public Evaluation.Expression EndRange => m_endrange;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_intnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_floatnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_startrrange;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_endrange;

		#endregion
	}
}