using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Evaluation.Operations
{
	class Null : Function
	{
		public Null(List<IFunction> children, List<Object> arguments)
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
		public Addition(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count == 0) return new Number();

			Number result = new Number(0);
			for (Int32 i = 0; i != Children.Count; ++i) result += Children[i].Evaluate(state);

			return result;
		}
	}

	class Substraction : Function
	{
		public Substraction(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count == 0) return new Number();

			if (Children.Count == 1) return new Number(0) - Children[0].Evaluate(state);

			Number result = Children[0].Evaluate(state);
			for (Int32 i = 1; i != Children.Count; ++i) result -= Children[i].Evaluate(state);

			return result;
		}
	}

	class Multiplication : Function
	{
		public Multiplication(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0].Evaluate(state);
			for (Int32 i = 1; i != Children.Count; ++i) result *= Children[i].Evaluate(state);

			return result;
		}
	}

	class Division : Function
	{
		public Division(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0].Evaluate(state);
			for (Int32 i = 1; i != Children.Count; ++i) result /= Children[i].Evaluate(state);

			return result;
		}
	}

	class Modulus : Function
	{
		public Modulus(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0].Evaluate(state);
			for (Int32 i = 1; i != Children.Count; ++i) result %= Children[i].Evaluate(state);

			return result;
		}
	}

	class Exponent : Function
	{
		public Exponent(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number result = Children[0].Evaluate(state);
			for (Int32 i = 1; i != Children.Count; ++i) result = Number.Power(result, Children[i].Evaluate(state));

			return result;
		}
	}

	class Equality : Function
	{
		public Equality(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0].Evaluate(state) == Children[1].Evaluate(state);
		}
	}

	class Inequality : Function
	{
		public Inequality(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0].Evaluate(state) != Children[1].Evaluate(state);
		}
	}

	class LesserThan : Function
	{
		public LesserThan(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0].Evaluate(state) < Children[1].Evaluate(state);
		}
	}

	class LesserEquals : Function
	{
		public LesserEquals(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0].Evaluate(state) <= Children[1].Evaluate(state);
		}
	}

	class GreaterThan : Function
	{
		public GreaterThan(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0].Evaluate(state) > Children[1].Evaluate(state);
		}
	}

	class GreaterEquals : Function
	{
		public GreaterEquals(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			return Children[0].Evaluate(state) >= Children[1].Evaluate(state);
		}
	}

	class LogicOr : Function
	{
		public LogicOr(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			for (Int32 i = 0; i != Children.Count; ++i)
			{
				Number result = Children[i].Evaluate(state);
				if (result.NumberType == NumberType.None) return new Number();

				if (result.BooleanValue == true) return new Number(true);
			}

			return new Number(false);
		}
	}

	class LogicAnd : Function
	{
		public LogicAnd(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			for (Int32 i = 0; i != Children.Count; ++i)
			{
				Number result = Children[i].Evaluate(state);
				if (result.NumberType == NumberType.None) return new Number();

				if (result.BooleanValue == false) return new Number(false);
			}

			return new Number(true);
		}
	}

	class LogicXor : Function
	{
		public LogicXor(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count < 2) return new Number();

			Number first = Children[0].Evaluate(state);
			if (first.NumberType == NumberType.None) return new Number();

			Boolean value = first.BooleanValue;

			for (Int32 i = 1; i != Children.Count; ++i)
			{
				Number result = Children[i].Evaluate(state);
				if (result.NumberType == NumberType.None) return new Number();

				value = value ^ result.BooleanValue;
				if (value == false) return new Number(false);
			}

			return new Number(true);

		}
	}

	class BitOr : Function
	{
		public BitOr(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number lhs = Children[0].Evaluate(state);
			Number rhs = Children[1].Evaluate(state);

			if (lhs.NumberType != NumberType.Int || rhs.NumberType != NumberType.Int) return new Number();

			return new Number(lhs.IntValue | rhs.IntValue);
		}
	}

	class BitXor : Function
	{
		public BitXor(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number lhs = Children[0].Evaluate(state);
			Number rhs = Children[1].Evaluate(state);

			if (lhs.NumberType != NumberType.Int || rhs.NumberType != NumberType.Int) return new Number();

			return new Number(lhs.IntValue ^ rhs.IntValue);
		}
	}

	class BitAnd : Function
	{
		public BitAnd(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 2) return new Number();

			Number lhs = Children[0].Evaluate(state);
			Number rhs = Children[1].Evaluate(state);

			if (lhs.NumberType != NumberType.Int || rhs.NumberType != NumberType.Int) return new Number();

			return new Number(lhs.IntValue & rhs.IntValue);
		}
	}

	class LogicNot : Function
	{
		public LogicNot(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0].Evaluate(state);
			if (number.NumberType == NumberType.None) return new Number();

			return new Number(!number.BooleanValue);
		}
	}

	class BitNot : Function
	{
		public BitNot(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0].Evaluate(state);
			return (number.NumberType == NumberType.Int) ? new Number(~number.IntValue) : new Number();
		}
	}

	class Range : Function
	{
		public Range(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 3 || Arguments.Count != 3) return new Number();

			Operator compare = (Operator)Arguments[0];
			Symbol pre = (Symbol)Arguments[1];
			Symbol post = (Symbol)Arguments[2];

			Number lhs = Children[0].Evaluate(state);
			Number rhs1 = Children[1].Evaluate(state);
			Number rhs2 = Children[2].Evaluate(state);

			return Number.Range(lhs, rhs1, rhs2, compare, pre, post);
		}
	}

	class Assignment : Function
	{
		public Assignment(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number result = Children[1].Evaluate(state);
			if (result.NumberType == NumberType.None) return new Number();

			Function varfunc = Children[0] as Function;
			if (varfunc == null) return new Number();

			if (varfunc is Triggers.Var)
			{
				Number varindex = varfunc.Children[0].Evaluate(state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetInteger(varindex.IntValue, false, result.IntValue) == false)
				{
					return new Number();
				}
			}
			else if (varfunc is Triggers.FVar)
			{
				Number varindex = varfunc.Children[0].Evaluate(state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetFloat(varindex.IntValue, false, result.FloatValue) == false)
				{
					return new Number();
				}
			}
			else if (varfunc is Triggers.SysVar)
			{
				Number varindex = varfunc.Children[0].Evaluate(state);
				if (varindex.NumberType == NumberType.None) return new Number();

				if (character.Variables.SetInteger(varindex.IntValue, true, result.IntValue) == false)
				{
					return new Number();
				}
			}
			else if (varfunc is Triggers.SysFVar)
			{
				Number varindex = varfunc.Children[0].Evaluate(state);
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