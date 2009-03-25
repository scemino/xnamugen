using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VarRangeSet")]
	class VarRangeSet : StateController
	{
		public VarRangeSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_intnumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_floatnumber = textsection.GetAttribute<Evaluation.Expression>("fvalue", null);
			m_startrrange = textsection.GetAttribute<Evaluation.Expression>("first", null);
			m_endrange = textsection.GetAttribute<Evaluation.Expression>("last", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? start = EvaluationHelper.AsInt32(character, StartRange, null);
			Int32? end = EvaluationHelper.AsInt32(character, EndRange, null);

			if (IntNumber != null)
			{
				Int32? value = EvaluationHelper.AsInt32(character, IntNumber, null);
				if (value != null)
				{
					for (Int32 i = 0; i != character.Variables.IntegerVariables.Count; ++i)
					{
						if (i < start || i > end) continue;
						character.Variables.SetInteger(i, false, value.Value);
					}
				}
			}

			if (FloatNumber != null)
			{
				Single? value = EvaluationHelper.AsSingle(character, FloatNumber, null);
				if (value != null)
				{
					for (Int32 i = 0; i != character.Variables.FloatVariables.Count; ++i)
					{
						if (i < start || i > end) continue;
						character.Variables.SetFloat(i, false, value.Value);
					}
				}
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if ((IntNumber != null ^ FloatNumber != null) == false) return false;

			return true;
		}

		public Evaluation.Expression IntNumber
		{
			get { return m_intnumber; }
		}

		public Evaluation.Expression FloatNumber
		{
			get { return m_floatnumber; }
		}

		public Evaluation.Expression StartRange
		{
			get { return m_startrrange; }
		}

		public Evaluation.Expression EndRange
		{
			get { return m_endrange; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_intnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_floatnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_startrrange;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_endrange;

		#endregion
	}
}