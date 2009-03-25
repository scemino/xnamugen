using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Evaluation.Operators
{
	class Assignment : IFunction
	{
		public EvaluationResult Evaluate(EvaluationState state, ReadOnlyList<IEvaluate> args, ReadOnlyList<Object> data)
		{
			Combat.Character character = state.Character as Combat.Character;
			if (character == null) return new EvaluationResult();

			EvaluationResult result = args[1].Evaluate(state);
			if (result.ResultType == ResultType.None) return new EvaluationResult();

			FuncCaller varcaller = args[0] as FuncCaller;
			if (varcaller == null) return new EvaluationResult();

			if (varcaller.Function is Triggers.Var)
			{
				EvaluationResult varindex = varcaller.Args[0].Evaluate(state);
				if (varindex.ResultType == ResultType.None) return new EvaluationResult();

				if (character.Variables.SetInterger(varindex.IntValue, false, result.IntValue) == false)
				{
					return new EvaluationResult();
				}
			}
			else if (varcaller.Function is Triggers.FVar)
			{
				EvaluationResult varindex = varcaller.Args[0].Evaluate(state);
				if (varindex.ResultType == ResultType.None) return new EvaluationResult();

				if (character.Variables.SetFloat(varindex.IntValue, false, result.FloatValue) == false)
				{
					return new EvaluationResult();
				}
			}
			else if (varcaller.Function is Triggers.SysVar)
			{
				EvaluationResult varindex = varcaller.Args[0].Evaluate(state);
				if (varindex.ResultType == ResultType.None) return new EvaluationResult();

				if (character.Variables.SetInterger(varindex.IntValue, true, result.IntValue) == false)
				{
					return new EvaluationResult();
				}
			}
			else if (varcaller.Function is Triggers.SysFVar)
			{
				EvaluationResult varindex = varcaller.Args[0].Evaluate(state);
				if (varindex.ResultType == ResultType.None) return new EvaluationResult();

				if (character.Variables.SetFloat(varindex.IntValue, true, result.FloatValue) == false)
				{
					return new EvaluationResult();
				}
			}
			else
			{
				return new EvaluationResult();
			}

			return result;
		}
	}
}