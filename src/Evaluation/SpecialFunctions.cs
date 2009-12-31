using System;

namespace xnaMugen.Evaluation
{
	static class SpecialFunctions
	{
		public static Int32 Assignment_Var(Object state, Int32 index, Int32 value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetInteger(index, false, value) == true) return value;

			throw new EvaluationException();
		}

		public static Single Assignment_FVar(Object state, Int32 index, Single value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetFloat(index, false, value) == true) return value;

			throw new EvaluationException();
		}

		public static Int32 Assignment_SysVar(Object state, Int32 index, Int32 value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetInteger(index, true, value) == true) return value;

			throw new EvaluationException();
		}

		public static Single Assignment_SysFVar(Object state, Int32 index, Single value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetFloat(index, true, value) == true) return value;

			throw new EvaluationException();
		}

		public static Boolean Range(Int32 lhs, Int32 rhs1, Int32 rhs2, Operator optype, Symbol pretype, Symbol posttype)
		{
			if (optype != Operator.Equals && optype != Operator.NotEquals) throw new EvaluationException();

			Boolean pre, post;

			switch (pretype)
			{
				case Symbol.LeftBracket:
					pre = lhs >= rhs1;
					break;

				case Symbol.LeftParen:
					pre = lhs > rhs1;
					break;
				default:
					throw new EvaluationException();
			}

			switch (posttype)
			{
				case Symbol.RightBracket:
					post = lhs <= rhs2;
					break;

				case Symbol.RightParen:
					post = lhs < rhs2;
					break;
				default:
					throw new EvaluationException();
			}

			return (optype == Operator.Equals) ? (pre && post) : !(pre && post);
		}

		public static Boolean Range(Single lhs, Single rhs1, Single rhs2, Operator optype, Symbol pretype, Symbol posttype)
		{
			if (optype != Operator.Equals && optype != Operator.NotEquals) throw new EvaluationException();

			Boolean pre, post;

			switch (pretype)
			{
				case Symbol.LeftBracket:
					pre = lhs >= rhs1;
					break;

				case Symbol.LeftParen:
					pre = lhs > rhs1;
					break;
				default:
					throw new EvaluationException();
			}

			switch (posttype)
			{
				case Symbol.RightBracket:
					post = lhs <= rhs2;
					break;

				case Symbol.RightParen:
					post = lhs < rhs2;
					break;
				default:
					throw new EvaluationException();
			}

			return (optype == Operator.Equals) ? (pre && post) : !(pre && post);
		}

		public static Boolean LogicalOperation(Operator @operator, Int32 lhs, Int32 rhs)
		{
			switch (@operator)
			{
				case Operator.Equals:
					return lhs == rhs;

				case Operator.NotEquals:
					return lhs != rhs;

				case Operator.Lesser:
					return lhs < rhs;

				case Operator.LesserEquals:
					return lhs <= rhs;

				case Operator.Greater:
					return lhs > rhs;

				case Operator.GreaterEquals:
					return lhs >= rhs;

				default:
					return false;
			}
		}

		public static Boolean LogicalOperation(Operator @operator, Single lhs, Single rhs)
		{
			switch (@operator)
			{
				case Operator.Equals:
					return lhs == rhs;

				case Operator.NotEquals:
					return lhs != rhs;

				case Operator.Lesser:
					return lhs < rhs;

				case Operator.LesserEquals:
					return lhs <= rhs;

				case Operator.Greater:
					return lhs > rhs;

				case Operator.GreaterEquals:
					return lhs >= rhs;

				default:
					return false;
			}
		}

	}
}