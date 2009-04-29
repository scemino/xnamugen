using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using xnaMugen.Evaluation;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen
{
#warning The EvaluationHelper is not threadsafe beacuse of the shared Result.

	static class EvaluationHelper
	{
		static EvaluationHelper()
		{
			s_result = new Result();
		}

		public static Int32 AsInt32(Object state, IExpression expression, Int32 failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true) return result[0].IntValue;

			return failover;
		}

		public static Int32? AsInt32(Object state, IExpression expression, Int32? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true) return result[0].IntValue;

			return failover;
		}

		public static Single AsSingle(Object state, IExpression expression, Single failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true) return result[0].FloatValue;

			return failover;
		}

		public static Single? AsSingle(Object state, IExpression expression, Single? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true) return result[0].FloatValue;

			return failover;
		}

		public static Boolean AsBoolean(Object state, IExpression expression, Boolean failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == false) return failover;

			return result[0].BooleanValue;
		}

		public static Boolean? AsBoolean(Object state, IExpression expression, Boolean? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == false) return failover;

			return result[0].BooleanValue;
		}

		public static Point AsPoint(Object state, IExpression expression, Point failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Point(result[0].IntValue, result[1].IntValue);
			}

			if (result.IsValid(0) == true)
			{
				return new Point(result[0].IntValue, 0);
			}

			return failover;
		}

		public static Point? AsPoint(Object state, IExpression expression, Point? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Point(result[0].IntValue, result[1].IntValue);
			}

			if (result.IsValid(0) == true)
			{
				return new Point(result[0].IntValue, 0);
			}

			return failover;
		}

		public static SoundId AsSoundId(Object state, IExpression expression, SoundId failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new SoundId(result[0].IntValue, result[1].IntValue);
			}

			if (result.IsValid(0) == true)
			{
				return new SoundId(result[0].IntValue, 0);
			}

			return failover;
		}

		public static SoundId? AsSoundId(Object state, IExpression expression, SoundId? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new SoundId(result[0].IntValue, result[1].IntValue);
			}

			if (result.IsValid(0) == true)
			{
				return new SoundId(result[0].IntValue, 0);
			}

			return failover;
		}

		public static Vector2 AsVector2(Object state, IExpression expression, Vector2 failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Vector2(result[0].FloatValue, result[1].FloatValue);
			}

			if (result.IsValid(0) == true)
			{
				return new Vector2(result[0].FloatValue, 0);
			}

			return failover;
		}

		public static Vector2? AsVector2(Object state, IExpression expression, Vector2? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Vector2(result[0].FloatValue, result[1].FloatValue);
			}

			if (result.IsValid(0) == true)
			{
				return new Vector2(result[0].FloatValue, 0);
			}

			return failover;
		}

		public static Vector3 AsVector3(Object state, IExpression expression, Vector3 failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true)
			{
				return new Vector3(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Vector3(result[0].FloatValue, result[1].FloatValue, 0);
			}

			if (result.IsValid(0) == true)
			{
				return new Vector3(result[0].FloatValue, 0, 0);
			}

			return failover;
		}

		public static Vector3? AsVector3(Object state, IExpression expression, Vector3? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true)
			{
				return new Vector3(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Vector3(result[0].FloatValue, result[1].FloatValue, 0);
			}

			if (result.IsValid(0) == true)
			{
				return new Vector3(result[0].FloatValue, 0, 0);
			}

			return failover;
		}

		public static Vector4 AsVector4(Object state, IExpression expression, Vector4 failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true && result.IsValid(3) == true)
			{
				return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, result[3].FloatValue);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true)
			{
				return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, 0);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Vector4(result[0].FloatValue, result[1].FloatValue, 0, 0);
			}

			if (result.IsValid(0) == true)
			{
				return new Vector4(result[0].FloatValue, 0, 0, 0);
			}

			return failover;
		}

		public static Vector4? AsVector4(Object state, IExpression expression, Vector4? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true && result.IsValid(3) == true)
			{
				return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, result[3].FloatValue);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true)
			{
				return new Vector4(result[0].FloatValue, result[1].FloatValue, result[2].FloatValue, 0);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Vector4(result[0].FloatValue, result[1].FloatValue, 0, 0);
			}

			if (result.IsValid(0) == true)
			{
				return new Vector4(result[0].FloatValue, 0, 0, 0);
			}

			return failover;
		}

		public static Rectangle AsRectangle(Object state, IExpression expression, Rectangle failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true && result.IsValid(3) == true)
			{
				return new Rectangle(result[0].IntValue, result[1].IntValue, result[2].IntValue - result[0].IntValue, result[3].IntValue - result[1].IntValue);
			}

			return failover;
		}

		public static Rectangle? AsRectangle(Object state, IExpression expression, Rectangle? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true && result.IsValid(3) == true)
			{
				return new Rectangle(result[0].IntValue, result[1].IntValue, result[2].IntValue - result[0].IntValue, result[3].IntValue - result[1].IntValue);
			}

			return failover;
		}

		public static Color AsColor(Object state, IExpression expression, Color failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true)
			{
				return new Color(result[0].FloatValue / 255.0f, result[1].FloatValue / 255.0f, result[2].FloatValue / 255.0f);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Color(result[0].FloatValue / 255.0f, result[1].FloatValue / 255.0f, 0);
			}

			if (result.IsValid(0) == true)
			{
				return new Color(result[0].FloatValue / 255.0f, 0, 0);
			}

			return failover;
		}

		public static Color? AsColor(Object state, IExpression expression, Color? failover)
		{
			Evaluation.Result result = Evaluate(state, expression);
			if (result == null) return failover;

			if (result.IsValid(0) == true && result.IsValid(1) == true && result.IsValid(2) == true)
			{
				return new Color(result[0].FloatValue / 255.0f, result[1].FloatValue / 255.0f, result[2].FloatValue / 255.0f);
			}

			if (result.IsValid(0) == true && result.IsValid(1) == true)
			{
				return new Color(result[0].FloatValue / 255.0f, result[1].FloatValue / 255.0f, 0);
			}

			if (result.IsValid(0) == true)
			{
				return new Color(result[0].FloatValue / 255.0f, 0, 0);
			}

			return failover;
		}

		public static Boolean IsCommon(PrefixedExpression expression, Boolean failover)
		{
			return (expression != null) ? expression.IsCommon(failover) : failover;
		}

		static Evaluation.Result Evaluate(Object state, IExpression expression)
		{
			if (expression == null || expression.IsValid == false) return null;

			expression.Evaluate(state, s_result);

			return s_result;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static Result s_result;

		#endregion
	}
}
