using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VarRandom")]
	internal class VarRandom : StateController
	{
		public VarRandom(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_intnumber = textsection.GetAttribute<Evaluation.Expression>("v", null);
			m_range = textsection.GetAttribute<Evaluation.Expression>("range", null);
		}

		public override void Run(Combat.Character character)
		{
			var varindex = EvaluationHelper.AsInt32(character, IntNumber, null);
			if (varindex == null) return;

			int min;
			int max;
			if (GetRange(character, out min, out max) == false) return;

			if (min > max) Misc.Swap(ref min, ref max);

			var randomvalue = StateSystem.GetSubSystem<Random>().NewInt(min, max);

			if (character.Variables.SetInteger(varindex.Value, false, randomvalue) == false)
			{
			}
		}

		private bool GetRange(Combat.Character character, out int min, out int max)
		{
			if (Range == null)
			{
				min = 0;
				max = 1000;
				return true;
			}

			var result = Range.Evaluate(character);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					min = result[0].IntValue;
					max = result[1].IntValue;
					return true;
				}

				min = 0;
				max = result[0].IntValue;
				return true;
			}

			min = 0;
			max = 1;
			return false;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (IntNumber == null) return false;

			return true;
		}

		public Evaluation.Expression IntNumber => m_intnumber;

		public Evaluation.Expression Range => m_range;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_intnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_range;

		#endregion
	}
}