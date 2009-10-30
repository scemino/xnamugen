using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	static class _Assignment
	{
		delegate Object FirstArgGetter(Object obj);

		static _Assignment()
		{
			s_getters = new Dictionary<Type, FirstArgGetter>();
		}

		static FirstArgGetter Get(Type type)
		{
			FirstArgGetter getter;
			if (s_getters.TryGetValue(type, out getter) == true) return getter;

			DynamicMethod method = new DynamicMethod("Getter", typeof(EvaluationCallback), new Type[] { typeof(Object) }, type);
			ILGenerator generator = method.GetILGenerator();

			FieldInfo info = type.GetField("m_arg1", BindingFlags.NonPublic | BindingFlags.Instance);

			generator.Emit(OpCodes.Ldarg, 0);
			generator.Emit(OpCodes.Ldfld, info);
			generator.Emit(OpCodes.Ret);

			getter = (FirstArgGetter)method.CreateDelegate(typeof(FirstArgGetter));
			s_getters[type] = getter;

			return getter;
		}

		public static Number RedirectState(Object state, EvaluationCallback lhs, EvaluationCallback rhs)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number result = rhs(state);
			if (result.NumberType == NumberType.None) return new Number();

			EvaluationCallback indexcallback = Get(lhs.Target.GetType())(lhs.Target) as EvaluationCallback;
			if (indexcallback == null) return new Number();

			Number varindex = indexcallback(state);
			if (varindex.NumberType == NumberType.None) return new Number();

			if (lhs.Target.GetType().Name == typeof(Var).Name)
			{
				if (character.Variables.SetInteger(varindex.IntValue, false, result.IntValue) == true) return result;
			}
			else if (lhs.Target.GetType().Name == typeof(FVar).Name)
			{
				if (character.Variables.SetFloat(varindex.IntValue, false, result.FloatValue) == true) return result;
			}
			else if (lhs.Target.GetType().Name == typeof(SysVar).Name)
			{
				if (character.Variables.SetInteger(varindex.IntValue, true, result.IntValue) == true) return result;
			}
			else if (lhs.Target.GetType().Name == typeof(SysFVar).Name)
			{
				if (character.Variables.SetFloat(varindex.IntValue, true, result.FloatValue) == true) return result;
			}

			return new Number();
		}

		static Dictionary<Type, FirstArgGetter> s_getters;
	}
}