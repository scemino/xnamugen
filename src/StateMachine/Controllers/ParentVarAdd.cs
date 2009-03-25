using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ParentVarAdd")]
	class ParentVarAdd : VarAdd
	{	
		public ParentVarAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			Combat.Helper helper = character as Combat.Helper;
			if (helper == null) return;

			if (IntNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, IntNumber, null);
				Int32? value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.AddInteger(index.Value, false, value.Value) == false)
				{
				}
			}

			if (FloatNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, FloatNumber, null);
				Single? value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.AddFloat(index.Value, false, value.Value) == false)
				{
				}
			}

			if (SystemIntNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, SystemIntNumber, null);
				Int32? value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.AddInteger(index.Value, true, value.Value) == false)
				{
				}
			}

			if (SystemFloatNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, SystemFloatNumber, null);
				Single? value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && helper.Parent.Variables.AddFloat(index.Value, true, value.Value) == false)
				{
				}
			}
		}
	}
}