using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ParentVarSet")]
	internal class ParentVarSet : VarSet
	{
		public ParentVarSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			var helper = character as Combat.Helper;
			if (helper == null) return;

			if (IntNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, IntNumber, null);
				var value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.SetInteger(index.Value, false, value.Value) == false)
				{
				}
			}

			if (FloatNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, FloatNumber, null);
				var value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.SetFloat(index.Value, false, value.Value) == false)
				{
				}
			}

			if (SystemIntNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, SystemIntNumber, null);
				var value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.SetInteger(index.Value, true, value.Value) == false)
				{
				}
			}

			if (SystemFloatNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, SystemFloatNumber, null);
				var value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.SetFloat(index.Value, true, value.Value) == false)
				{
				}
			}
		}

	}
}