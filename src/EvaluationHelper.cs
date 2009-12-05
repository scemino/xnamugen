using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using xnaMugen.Evaluation;

namespace xnaMugen
{
	static class EvaluationHelper
	{
		public static Int32 AsInt32(Object state, IExpression expression, Int32 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);
			return (result.Length > 0) ? result[0].IntValue : failover;
		}

		public static Int32? AsInt32(Object state, IExpression expression, Int32? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);
			return (result.Length > 0) ? result[0].IntValue : failover;
		}

		public static Single AsSingle(Object state, IExpression expression, Single failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);
			return (result.Length > 0) ? result[0].FloatValue : failover;
		}

		public static Single? AsSingle(Object state, IExpression expression, Single? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);
			return (result.Length > 0) ? result[0].FloatValue : failover;
		}

		public static Boolean AsBoolean(Object state, IExpression expression, Boolean failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);
			return (result.Length > 0) ? result[0].BooleanValue : failover;
		}

		public static Boolean? AsBoolean(Object state, IExpression expression, Boolean? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);
			return (result.Length > 0) ? result[0].BooleanValue : failover;
		}

		public static Point AsPoint(Object state, IExpression expression, Point failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Point(result[0].IntValue, result[1].IntValue);
				}
				else
				{
					return new Point(result[0].IntValue, 0);
				}
			}

			return failover;
		}

		public static Point? AsPoint(Object state, IExpression expression, Point? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Point(result[0].IntValue, result[1].IntValue);
				}
				else
				{
					return new Point(result[0].IntValue, 0);
				}
			}

			return failover;
		}

		public static SoundId AsSoundId(Object state, IExpression expression, SoundId failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new SoundId(result[0].IntValue, result[1].IntValue);
				}
				else
				{
					return new SoundId(result[0].IntValue, 0);
				}
			}

			return failover;
		}

		public static SoundId? AsSoundId(Object state, IExpression expression, SoundId? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new SoundId(result[0].IntValue, result[1].IntValue);
				}
				else
				{
					return new SoundId(result[0].IntValue, 0);
				}
			}

			return failover;
		}

		public static Vector2 AsVector2(Object state, IExpression expression, Vector2 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Vector2(result[0].FloatValue, result[1].FloatValue);
				}
				else
				{
					return new Vector2(result[0].FloatValue, 0);
				}
			}

			return failover;
		}

		public static Vector2? AsVector2(Object state, IExpression expression, Vector2? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					return new Vector2(result[0].FloatValue, result[1].FloatValue);
				}
				else
				{
					return new Vector2(result[0].FloatValue, 0);
				}
			}

			return failover;
		}

		public static Vector3 AsVector3(Object state, IExpression expression, Vector3 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					if (result.Length > 2 && result[2].NumberType != NumberType.None)
					{
						return new Vector3(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue);
					}
					else
					{
						return new Vector3(result[0].FloatValue, result[1].FloatValue, 0);
					}
				}
				else
				{
					return new Vector3(result[0].FloatValue, 0, 0);
				}
			}

			return failover;
		}

		public static Vector3? AsVector3(Object state, IExpression expression, Vector3? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 0 && result[0].NumberType != NumberType.None)
			{
				if (result.Length > 1 && result[1].NumberType != NumberType.None)
				{
					if (result.Length > 2 && result[2].NumberType != NumberType.None)
					{
						return new Vector3(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue);
					}
					else
					{
						return new Vector3(result[0].FloatValue, result[1].FloatValue, 0);
					}
				}
				else
				{
					return new Vector3(result[0].FloatValue, 0, 0);
				}
			}

			return failover;
		}

		public static Vector4 AsVector4(Object state, IExpression expression, Vector4 failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

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
						else
						{
							return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, 0);
						}
					}
					else
					{
						return new Vector4(result[0].FloatValue, result[1].FloatValue, 0, 0);
					}
				}
				else
				{
					return new Vector4(result[0].FloatValue, 0, 0, 0);
				}
			}

			return failover;
		}

		public static Vector4? AsVector4(Object state, IExpression expression, Vector4? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

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
						else
						{
							return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, 0);
						}
					}
					else
					{
						return new Vector4(result[0].FloatValue, result[1].FloatValue, 0, 0);
					}
				}
				else
				{
					return new Vector4(result[0].FloatValue, 0, 0, 0);
				}
			}

			return failover;
		}

		public static Rectangle AsRectangle(Object state, IExpression expression, Rectangle failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 3 && result[0].NumberType != NumberType.None && result[1].NumberType != NumberType.None && result[2].NumberType != NumberType.None && result[3].NumberType != NumberType.None)
			{
				return new Rectangle(result[0].IntValue, result[1].IntValue, result[2].IntValue - result[0].IntValue, result[3].IntValue - result[1].IntValue);
			}

			return failover;
		}

		public static Rectangle? AsRectangle(Object state, IExpression expression, Rectangle? failover)
		{
			if (expression == null || expression.IsValid == false) return failover;

			Number[] result = expression.Evaluate(state);

			if (result.Length > 3 && result[0].NumberType != NumberType.None && result[1].NumberType != NumberType.None && result[2].NumberType != NumberType.None && result[3].NumberType != NumberType.None)
			{
				return new Rectangle(result[0].IntValue, result[1].IntValue, result[2].IntValue - result[0].IntValue, result[3].IntValue - result[1].IntValue);
			}

			return failover;
		}

		public static Boolean IsCommon(PrefixedExpression expression, Boolean failover)
		{
			return (expression != null) ? expression.IsCommon(failover) : failover;
		}
	}
}
