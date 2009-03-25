using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Evaluation.Operations
{
	class Null : Function
	{
		public Null(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			return new Number();
		}
	}

	class Addition : Function
	{
		public Addition(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count == 0) return new Number();

			Number result = new Number(0);
			for (Int32 i = 0; i != Children.Count; ++i) result += Children[i](state);

			return result;
		}
	}

	class Substraction : Function
	{
		public Substraction(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count == 0) return new Number();

			if (Children.Count == 1) return new Number(0) - Children[0](state);

			Number result = Children[0](state);
			for (Int32 i = 1; i != Children.Count; ++i) result -= Children[i](state);

			return result;
		}
	}

	class Multiplication : Function
	{
		public Multiplication(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0](state);
			for (Int32 i = 1; i != Children.Count; ++i) result *= Children[i](state);

			return result;
		}
	}

	class Division : Function
	{
		public Division(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0](state);
			for (Int32 i = 1; i != Children.Count; ++i) result /= Children[i](state);

			return result;
		}
	}

	class Modulus : Function
	{
		public Modulus(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0](state);
			for (Int32 i = 1; i != Children.Count; ++i) result %= Children[i](state);

			return result;
		}
	}

	class Exponent : Function
	{
		public Exponent(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0](state);
			for (Int32 i = 1; i != Children.Count; ++i) result = Number.Power(result, Children[i](state));

			return result;
		}
	}

	class Equality : Function
	{
		public Equality(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0](state) == Children[1](state);
		}
	}

	class Inequality : Function
	{
		public Inequality(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0](state) != Children[1](state);
		}
	}

	class LesserThan : Function
	{
		public LesserThan(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0](state) < Children[1](state);
		}
	}

	class LesserEquals : Function
	{
		public LesserEquals(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0](state) <= Children[1](state);
		}
	}

	class GreaterThan : Function
	{
		public GreaterThan(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0](state) > Children[1](state);
		}
	}

	class GreaterEquals : Function
	{
		public GreaterEquals(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0](state) >= Children[1](state);
		}
	}

	class LogicOr : Function
	{
		public LogicOr(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			for (Int32 i = 0; i != Children.Count; ++i)
			{
				Number result = Children[i](state);
				if (result.NumberType == NumberType.None) return new Number();

				if (result.BooleanValue == true) return new Number(true);
			}

			return new Number(false);
		}
	}

	class LogicAnd : Function
	{
		public LogicAnd(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			for (Int32 i = 0; i != Children.Count; ++i)
			{
				Number result = Children[i](state);
				if (result.NumberType == NumberType.None) return new Number();

				if (result.BooleanValue == false) return new Number(false);
			}

			return new Number(true);
		}
	}

	class LogicXor : Function
	{
		public LogicXor(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number first = Children[0](state);
			if (first.NumberType == NumberType.None) return new Number();

			Boolean value = first.BooleanValue;

			for (Int32 i = 1; i != Children.Count; ++i)
			{
				Number result = Children[i](state);
				if (result.NumberType == NumberType.None) return new Number();

				value = value ^ result.BooleanValue;
				if (value == false) return new Number(false);
			}

			return new Number(true);

		}
	}

	class BitOr : Function
	{
		public BitOr(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number lhs = Children[0](state);
			Number rhs = Children[1](state);

			if (lhs.NumberType != NumberType.Int || rhs.NumberType != NumberType.Int) return new Number();

			return new Number(lhs.IntValue | rhs.IntValue);
		}
	}

	class BitXor : Function
	{
		public BitXor(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number lhs = Children[0](state);
			Number rhs = Children[1](state);

			if (lhs.NumberType != NumberType.Int || rhs.NumberType != NumberType.Int) return new Number();

			return new Number(lhs.IntValue ^ rhs.IntValue);
		}
	}

	class BitAnd : Function
	{
		public BitAnd(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number lhs = Children[0](state);
			Number rhs = Children[1](state);

			if (lhs.NumberType != NumberType.Int || rhs.NumberType != NumberType.Int) return new Number();

			return new Number(lhs.IntValue & rhs.IntValue);
		}
	}

	class LogicNot : Function
	{
		public LogicNot(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0](state);
			if (number.NumberType == NumberType.None) return new Number();

			return new Number(!number.BooleanValue);
		}
	}

	class BitNot : Function
	{
		public BitNot(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0](state);
			return (number.NumberType == NumberType.Int) ? new Number(~number.IntValue) : new Number();
		}
	}

	class Range : Function
	{
		public Range(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 3 || Arguments.Count != 3) return new Number();

			Operator compare = (Operator)Arguments[0];
			Symbol pre = (Symbol)Arguments[1];
			Symbol post = (Symbol)Arguments[2];

			Number lhs = Children[0](state);
			Number rhs1 = Children[1](state);
			Number rhs2 = Children[2](state);

			return Number.Range(lhs, rhs1, rhs2, compare, pre, post);
		}
	}

	class Assignment : Function
	{
		public Assignment(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number result = Children[1](state);
			if (result.NumberType == NumberType.None) return new Number();

			Function varfunc = Children[0].Target as Function;
			if (varfunc == null) return new Number();

			if (varfunc is Triggers.Var)
			{
				Number varindex = varfunc.Children[0](state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetInteger(varindex.IntValue, false, result.IntValue) == false)
				{
					return new Number();
				}
			}
			else if (varfunc is Triggers.FVar)
			{
				Number varindex = varfunc.Children[0](state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetFloat(varindex.IntValue, false, result.FloatValue) == false)
				{
					return new Number();
				}
			}
			else if (varfunc is Triggers.SysVar)
			{
				Number varindex = varfunc.Children[0](state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetInteger(varindex.IntValue, true, result.IntValue) == false)
				{
					return new Number();
				}
			}
			else if (varfunc is Triggers.SysFVar)
			{
				Number varindex = varfunc.Children[0](state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetFloat(varindex.IntValue, true, result.FloatValue) == false)
				{
					return new Number();
				}
			}
			else
			{
				return new Number();
			}

			return result;
		}
	}

}