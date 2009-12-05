using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VarRandom")]
	class VarRandom : StateController
	{
		public VarRandom(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_intnumber = textsection.GetAttribute<Evaluation.Expression>("v", null);
			m_range = textsection.GetAttribute<Evaluation.Expression>("range", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? varindex = EvaluationHelper.AsInt32(character, IntNumber, null);
			if (varindex == null) return;

			Int32 min = 0;
			Int32 max = 1;
			if (GetRange(character, out min, out max) == false) return;

			if (min > max) Misc.Swap(ref min, ref max);

			Int32 randomvalue = StateSystem.GetSubSystem<Random>().NewInt(min, max);

			if (character.Variables.SetInteger(varindex.Value, false, randomvalue) == false)
			{
			}
		}

		Boolean GetRange(Combat.Character character, out Int32 min, out Int32 max)
		{
			if (Range == null)
			{
				min = 0;
				max = 1000;
				return true;
			}

			Evaluation.Number[] result = Range.Evaluate(character);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					min = result[0].IntValue;
					max = result[1].IntValue;
					return true;
				}
				else
				{
					min = 0;
					max = result[0].IntValue;
					return true;
				}
			}

			min = 0;
			max = 1;
			return false;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (IntNumber == null) return false;

			return true;
		}

		public Evaluation.Expression IntNumber
		{
			get { return m_intnumber; }
		}

		public Evaluation.Expression Range
		{
			get { return m_range; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_intnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_range;

		#endregion
	}
}