using xnaMugen.Combat;

namespace xnaMugen.Evaluation
{
	internal static class SpecialFunctions
	{
		public static int Assignment_Var(Character character, int index, int value)
		{
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetInteger(index, false, value)) return value;

			throw new EvaluationException();
		}

		public static float Assignment_FVar(Character character, int index, float value)
		{
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetFloat(index, false, value)) return value;

			throw new EvaluationException();
		}

		public static int Assignment_SysVar(Character character, int index, int value)
		{
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetInteger(index, true, value)) return value;

			throw new EvaluationException();
		}

		public static float Assignment_SysFVar(Character character, int index, float value)
		{
			if (character == null) throw new EvaluationException();

			if (character.Variables.SetFloat(index, true, value)) return value;

			throw new EvaluationException();
		}

		public static bool Range(int lhs, int rhs1, int rhs2, Operator optype, Symbol pretype, Symbol posttype)
		{
			if (optype != Operator.Equals && optype != Operator.NotEquals) throw new EvaluationException();

			bool pre, post;

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

			return optype == Operator.Equals ? pre && post : !(pre && post);
		}

		public static bool Range(float lhs, float rhs1, float rhs2, Operator optype, Symbol pretype, Symbol posttype)
		{
			if (optype != Operator.Equals && optype != Operator.NotEquals) throw new EvaluationException();

			bool pre, post;

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

			return optype == Operator.Equals ? pre && post : !(pre && post);
		}

		public static bool LogicalOperation(Operator @operator, int lhs, int rhs)
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

		public static bool LogicalOperation(Operator @operator, float lhs, float rhs)
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