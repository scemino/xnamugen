using Microsoft.Xna.Framework;
using xnaMugen.Evaluation;

namespace xnaMugen
{
	internal static class EvaluationHelper
	{
		public static int AsInt32(object state, IExpression expression, int failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);
			return result.Length > 0 ? result[0].IntValue : failover;
		}

		public static int? AsInt32(object state, IExpression expression, int? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);
			return result.Length > 0 ? result[0].IntValue : failover;
		}

		public static float AsSingle(object state, IExpression expression, float failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);
			return result.Length > 0 ? result[0].FloatValue : failover;
		}

		public static float? AsSingle(object state, IExpression expression, float? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);
			return result.Length > 0 ? result[0].FloatValue : failover;
		}

		public static bool AsBoolean(object state, IExpression expression, bool failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);
			return result.Length > 0 ? result[0].BooleanValue : failover;
		}

		public static bool? AsBoolean(object state, IExpression expression, bool? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);
			return result.Length > 0 ? result[0].BooleanValue : failover;
		}

		public static Point AsPoint(object state, IExpression expression, Point failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Point(result[0].IntValue, result[1].IntValue);
				}

				return new Point(result[0].IntValue, 0);
			}

			return failover;
		}

		public static Point? AsPoint(object state, IExpression expression, Point? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Point(result[0].IntValue, result[1].IntValue);
				}

				return new Point(result[0].IntValue, 0);
			}

			return failover;
		}

		public static SoundId AsSoundId(object state, IExpression expression, SoundId failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new SoundId(result[0].IntValue, result[1].IntValue);
				}

				return new SoundId(result[0].IntValue, 0);
			}

			return failover;
		}

		public static SoundId? AsSoundId(object state, IExpression expression, SoundId? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new SoundId(result[0].IntValue, result[1].IntValue);
				}

				return new SoundId(result[0].IntValue, 0);
			}

			return failover;
		}

		public static Vector2 AsVector2(object state, IExpression expression, Vector2 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Vector2(result[0].FloatValue, result[1].FloatValue);
				}

				return new Vector2(result[0].FloatValue, 0);
			}

			return failover;
		}

		public static Vector2? AsVector2(object state, IExpression expression, Vector2? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Vector2(result[0].FloatValue, result[1].FloatValue);
				}

				return new Vector2(result[0].FloatValue, 0);
			}

			return failover;
		}

		public static Vector3 AsVector3(object state, IExpression expression, Vector3 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					if (result.Length > 2 && result[2].NumberType != NumberType.None)
					{
						return new Vector3(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue);
					}

					return new Vector3(result[0].FloatValue, result[1].FloatValue, 0);
				}

				return new Vector3(result[0].FloatValue, 0, 0);
			}

			return failover;
		}

		public static Vector3? AsVector3(object state, IExpression expression, Vector3? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					if (result.Length > 2 && result[2].NumberType != NumberType.None)
					{
						return new Vector3(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue);
					}

					return new Vector3(result[0].FloatValue, result[1].FloatValue, 0);
				}

				return new Vector3(result[0].FloatValue, 0, 0);
			}

			return failover;
		}

		public static Vector4 AsVector4(object state, IExpression expression, Vector4 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					if (result.Length > 2 && result[2].NumberType != NumberType.None)
					{
						if (result.Length > 3 && result[3].NumberType != NumberType.None)
						{
							return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, result[3].FloatValue);
						}

						return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, 0);
					}

					return new Vector4(result[0].FloatValue, result[1].FloatValue, 0, 0);
				}

				return new Vector4(result[0].FloatValue, 0, 0, 0);
			}

			return failover;
		}

		public static Vector4? AsVector4(object state, IExpression expression, Vector4? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					if (result.Length > 2 && result[2].NumberType != NumberType.None)
					{
						if (result.Length > 3 && result[3].NumberType != NumberType.None)
						{
							return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, result[3].FloatValue);
						}

						return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, 0);
					}

					return new Vector4(result[0].FloatValue, result[1].FloatValue, 0, 0);
				}

				return new Vector4(result[0].FloatValue, 0, 0, 0);
			}

			return failover;
		}

		public static Rectangle AsRectangle(object state, IExpression expression, Rectangle failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 3 && result[0].NumberType != NumberType.None && result[1].NumberType != NumberType.None && result[2].NumberType != NumberType.None && result[3].NumberType != NumberType.None)
			{
				return new Rectangle(result[0].IntValue, result[1].IntValue, result[2].IntValue - result[0].IntValue, result[3].IntValue - result[1].IntValue);
			}

			return failover;
		}

		public static Rectangle? AsRectangle(object state, IExpression expression, Rectangle? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			var result = expression.Evaluate(state);

			if (result.Length > 3 && result[0].NumberType != NumberType.None && result[1].NumberType != NumberType.None && result[2].NumberType != NumberType.None && result[3].NumberType != NumberType.None)
			{
				return new Rectangle(result[0].IntValue, result[1].IntValue, result[2].IntValue - result[0].IntValue, result[3].IntValue - result[1].IntValue);
			}

			return failover;
		}

		public static bool IsCommon(PrefixedExpression expression, bool failover)
		{
			return expression != null ? expression.IsCommon(failover) : failover;
		}
	}
}
