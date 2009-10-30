using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using xnaMugen.Collections;
using System.Globalization;

namespace xnaMugen
{
	[AttributeUsage(AttributeTargets.Method)]
	class StringConversionAttribute : Attribute
	{
		public StringConversionAttribute(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			m_type = type;
		}

		public Type Type
		{
			get { return m_type; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Type m_type;

		#endregion
	}

	class StringConverter : SubSystem
	{
		public StringConverter(SubSystems subsystems)
			: base(subsystems)
		{
			m_blendingregex = new Regex(@"^AS(\d+)d(\d+)$", RegexOptions.IgnoreCase);
			m_printdataregex = new Regex(@"^(\d+)\s*,\s*(\d+)\s*,?\s*?(-?\d+)?$", RegexOptions.IgnoreCase);
			m_hitpriorityregex = new Regex(@"^(\d+),\s*(\w+)$", RegexOptions.IgnoreCase);
			m_failure = new Object();
			m_conversionmap = BuildConversionMap();
		}

		ReadOnlyDictionary<Type, Converter<String, Object>> BuildConversionMap()
		{
			var conversionmap = new Dictionary<Type, Converter<String, Object>>();

			foreach (MethodInfo mi in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				StringConversionAttribute attrib = (StringConversionAttribute)Attribute.GetCustomAttribute(mi, typeof(StringConversionAttribute));
				if (attrib == null) continue;

				Converter<String, Object> d = (Converter<String, Object>)Delegate.CreateDelegate(typeof(Converter<String, Object>), this, mi);
				conversionmap.Add(attrib.Type, d);

				if (attrib.Type.IsValueType == true)
				{
					Type nullable_type = typeof(Nullable<>).MakeGenericType(attrib.Type);
					conversionmap.Add(nullable_type, d);
				}
			}

			return new ReadOnlyDictionary<Type, Converter<String, Object>>(conversionmap);
		}

		public T Convert<T>(String str)
		{
			if (str == null) throw new ArgumentNullException("str");

			T output;
			if (TryConvert(str, out output) == true) return output;

			Log.Write(LogLevel.Warning, LogSystem.StringConverter, "Cannot convert '{0}' to type: {1}", str, typeof(T).Name);
			return default(T);
		}

		public Boolean TryConvert<T>(String str, out T output)
		{
			Converter<String, Object> converter = null;
			if (m_conversionmap.TryGetValue(typeof(T), out converter) == false)
			{
				output = default(T);
				return false;
			}

			Object obj = converter(str);
			if (obj == Failure)
			{
				output = default(T);
				return false;
			}

			output = (T)obj;
			return true;
		}

		[StringConversion(typeof(Keys))]
		Object ToKeys(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;
			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				if (sc.Equals(key.ToString(), s) == true) return key;
			}

			return Failure;
		}

		[StringConversion(typeof(String))]
		Object ToString(String s)
		{
			return s;
		}

		[StringConversion(typeof(Rectangle))]
		Object ToRectangle(String s)
		{
			Evaluation.Expression expression = (Evaluation.Expression)ToExpression(s);

			Rectangle? rectangle = EvaluationHelper.AsRectangle(null, expression, null);
			if (rectangle == null) return Failure;

			return rectangle.Value;
		}

		[StringConversion(typeof(BackgroundLayer))]
		Object ToBackgroundLayer(String s)
		{
			Int32 layernumber;
			if (TryConvert<Int32>(s, out layernumber) == false) return Failure;

			if (layernumber == 0) return BackgroundLayer.Back;
			if (layernumber == 1) return BackgroundLayer.Front;

			return Failure;
		}

		[StringConversion(typeof(Int32))]
		Object ToInt32(String s)
		{
			Int32 val = 0;
			if (Int32.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out val) == true) return val;

			return Failure;
		}

		[StringConversion(typeof(Single))]
		Object ToSingle(String s)
		{
			Single val = 0;
			if (Single.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out val) == true) return val;

			return Failure;
		}

		[StringConversion(typeof(Boolean))]
		Object ToBoolean(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals("true", s) == true) return true;
			if (sc.Equals("false", s) == true) return false;

			Int32 intvalue;
			if (TryConvert<Int32>(s, out intvalue) == true)
			{
				return (intvalue > 0) ? true : false;
			}

			return Failure;
		}

		[StringConversion(typeof(Evaluation.Expression))]
		Object ToExpression(String s)
		{
			return GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(s);
		}

		[StringConversion(typeof(Evaluation.PrefixedExpression))]
		Object ToPrefixedExpression(String s)
		{
			return GetSubSystem<Evaluation.EvaluationSystem>().CreatePrefixedExpression(s);
		}

		[StringConversion(typeof(Point))]
		Object ToPoint(String s)
		{
			Evaluation.Expression expression;
			if (TryConvert<Evaluation.Expression>(s, out expression) == false) return Failure;

			Point? point = EvaluationHelper.AsPoint(null, expression, null);
			if (point == null) return Failure;

			return point.Value;
		}

		[StringConversion(typeof(Vector2))]
		Object ToVector2(String s)
		{
			Evaluation.Expression expression;
			if (TryConvert<Evaluation.Expression>(s, out expression) == false) return Failure;

			Vector2? vector = EvaluationHelper.AsVector2(null, expression, null);
			if (vector == null) return Failure;

			return vector.Value;
		}

		[StringConversion(typeof(Blending))]
		Object ToBlending(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "none") == true)
			{
				return new Blending();
			}

			if (sc.Equals(s, "addalpha") == true)
			{
				return new Blending(BlendType.Add, 0, 0);
			}

			if (sc.Equals(s, "add") == true || sc.Equals(s, "a") == true)
			{
				return new Blending(BlendType.Add, 255, 255);
			}

			if (sc.Equals(s, "add1") == true || sc.Equals(s, "a1") == true)
			{
				return new Blending(BlendType.Add, 255, 127);
			}

			if (Char.ToUpper(s[0]) == 'S')
			{
				return new Blending(BlendType.Subtract, 255, 255);
			}

			Match m = m_blendingregex.Match(s);
			if (m.Success == true)
			{
				Int32 source;
				Int32 destination;
				if (Int32.TryParse(m.Groups[1].Value, out source) == true && Int32.TryParse(m.Groups[2].Value, out destination) == true)
				{
					return new Blending(BlendType.Add, source, destination);
				}
			}

			return Failure;
		}

		[StringConversion(typeof(SpriteId))]
		Object ToSpriteId(String s)
		{
			Point p;
			if (TryConvert<Point>(s, out p) == false) return Failure;

			return new SpriteId(p.X, p.Y);
		}

		[StringConversion(typeof(Drawing.PrintData))]
		Object ToPrintData(String s)
		{
			Match m = m_printdataregex.Match(s);
			if (m.Success == false) return Failure;

			Int32 index = Int32.Parse(m.Groups[1].Value);
			Int32 color = Int32.Parse(m.Groups[2].Value);
			PrintJustification justification = PrintJustification.Center;

			if (m.Groups[3].Value == "")
			{
				justification = PrintJustification.Center;
			}
			else
			{
				Int32 just = Int32.Parse(m.Groups[3].Value);

				if (just == 0) justification = PrintJustification.Center;
				if (just > 0) justification = PrintJustification.Left;
				if (just < 0) justification = PrintJustification.Right;
			}

			return new Drawing.PrintData(index, color, justification);
		}

		[StringConversion(typeof(SoundId))]
		Object ToSoundId(String s)
		{
			Point p;
			if (TryConvert<Point>(s, out p) == false) return Failure;

			return new SoundId(p.X, p.Y);
		}

		[StringConversion(typeof(Facing))]
		Object ToFacing(String s)
		{
			Int32 intvalue;
			if (TryConvert<Int32>(s, out intvalue) == false) return Failure;

			return (intvalue >= 0) ? Facing.Right : Facing.Left;
		}

		[StringConversion(typeof(AttackStateType))]
		Object ToAttackStateType(String s)
		{
			AttackStateType ast = AttackStateType.None;

			if (s.IndexOf("s", StringComparison.OrdinalIgnoreCase) != -1) ast |= AttackStateType.Standing;
			if (s.IndexOf("c", StringComparison.OrdinalIgnoreCase) != -1) ast |= AttackStateType.Crouching;
			if (s.IndexOf("a", StringComparison.OrdinalIgnoreCase) != -1) ast |= AttackStateType.Air;

			if (ast == AttackStateType.None && s != String.Empty) return Failure;

			return ast;
		}

		[StringConversion(typeof(PositionType))]
		Object ToPositionType(String s)
		{
			StringComparer sc = StringComparer.InvariantCultureIgnoreCase;

			if (sc.Equals(s, "p1") == true) return PositionType.P1;
			if (sc.Equals(s, "p2") == true) return PositionType.P2;
			if (sc.Equals(s, "front") == true) return PositionType.Front;
			if (sc.Equals(s, "back") == true) return PositionType.Back;
			if (sc.Equals(s, "left") == true) return PositionType.Left;
			if (sc.Equals(s, "right") == true) return PositionType.Right;

			return Failure;
		}

		[StringConversion(typeof(Combat.HitType))]
		Object ToHitType(String s)
		{
			if (s.Length > 2) return Failure;

			AttackClass aclass = AttackClass.None;
			AttackPower apower = AttackPower.None;

			if (Char.ToUpper(s[0]) == 'N') apower = AttackPower.Normal;
			else if (Char.ToUpper(s[0]) == 'H') apower = AttackPower.Hyper;
			else if (Char.ToUpper(s[0]) == 'S') apower = AttackPower.Special;
			else if (Char.ToUpper(s[0]) == 'A') apower = AttackPower.All;
			else return Failure;

			if (s.Length == 1) aclass = AttackClass.All;
			else
			{
				if (Char.ToUpper(s[1]) == 'T') aclass = AttackClass.Throw;
				else if (Char.ToUpper(s[1]) == 'P') aclass = AttackClass.Projectile;
				else if (Char.ToUpper(s[1]) == 'A') aclass = AttackClass.Normal;
				else return Failure;
			}

			return new Combat.HitType(aclass, apower);
		}

		[StringConversion(typeof(Combat.HitAttribute))]
		Object ToHitAttribute(String s)
		{
			AttackStateType attackheight = AttackStateType.None;
			List<Combat.HitType> attackdata = new List<Combat.HitType>();

			Boolean first = true;
			foreach (String str in Regex.Split(s, @"\s*,\s*", RegexOptions.IgnoreCase))
			{
				if (first == true)
				{
					first = false;

					if (TryConvert(str, out attackheight) == false) return Failure;
				}
				else
				{
					Combat.HitType hittype;
					if (TryConvert(str, out hittype) == false) return Failure;

					attackdata.Add(hittype);
				}
			}

			return new Combat.HitAttribute(attackheight, new ReadOnlyList<Combat.HitType>(attackdata));
		}

		[StringConversion(typeof(SpriteEffects))]
		Object ToSpriteEffects(String s)
		{
			Int32 intvalue;
			if (TryConvert<Int32>(s, out intvalue) == false) return Failure;

			return (intvalue >= 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		}

		[StringConversion(typeof(StateType))]
		Object ToStateType(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "a") == true) return StateType.Airborne;
			if (sc.Equals(s, "c") == true) return StateType.Crouching;
			if (sc.Equals(s, "s") == true) return StateType.Standing;
			if (sc.Equals(s, "l") == true) return StateType.Prone;
			if (sc.Equals(s, "u") == true) return StateType.Unchanged;

			return Failure;
		}

		[StringConversion(typeof(HelperType))]
		Object ToHelperType(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "Normal") == true) return HelperType.Normal;
			if (sc.Equals(s, "Player") == true) return HelperType.Player;

#warning Temporary Hack
			if (sc.Equals(s, "Projectile") == true) return HelperType.Normal;

			return Failure;
		}

		[StringConversion(typeof(MoveType))]
		Object ToMoveType(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "a") == true) return MoveType.Attack;
			if (sc.Equals(s, "i") == true) return MoveType.Idle;
			if (sc.Equals(s, "h") == true) return MoveType.BeingHit;
			if (sc.Equals(s, "u") == true) return MoveType.Unchanged;

			return Failure;
		}

		[StringConversion(typeof(Physics))]
		Object ToPhsyics(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "s") == true) return Physics.Standing;
			if (sc.Equals(s, "c") == true) return Physics.Crouching;
			if (sc.Equals(s, "a") == true) return Physics.Airborne;
			if (sc.Equals(s, "n") == true) return Physics.None;
			if (sc.Equals(s, "u") == true) return Physics.Unchanged;

			return Failure;
		}

		[StringConversion(typeof(Combat.HitFlag))]
		Object ToHitFlag(String s)
		{
			Boolean high = false;
			Boolean low = false;
			Boolean air = false;
			Boolean falling = false;
			Boolean down = false;
			HitFlagCombo combo = HitFlagCombo.DontCare;

			if (s.IndexOf("H", StringComparison.InvariantCultureIgnoreCase) != -1) high = true;
			if (s.IndexOf("L", StringComparison.InvariantCultureIgnoreCase) != -1) low = true;
			if (s.IndexOf("M", StringComparison.InvariantCultureIgnoreCase) != -1) { high = true; low = true; }
			if (s.IndexOf("A", StringComparison.InvariantCultureIgnoreCase) != -1) air = true;
			if (s.IndexOf("D", StringComparison.InvariantCultureIgnoreCase) != -1) down = true;
			if (s.IndexOf("F", StringComparison.InvariantCultureIgnoreCase) != -1) falling = true;
			if (s.IndexOf("+", StringComparison.InvariantCultureIgnoreCase) != -1) combo = HitFlagCombo.Yes;
			if (s.IndexOf("-", StringComparison.InvariantCultureIgnoreCase) != -1) combo = HitFlagCombo.No;

			return new Combat.HitFlag(combo, high, low, air, falling, down);
		}

		[StringConversion(typeof(HitAnimationType))]
		Object ToHitAnimationType(String s)
		{
			if (s.Length == 0) return Failure;

			if (Char.ToUpper(s[0]) == 'L') return HitAnimationType.Light;
			if (Char.ToUpper(s[0]) == 'M') return HitAnimationType.Medium;
			if (Char.ToUpper(s[0]) == 'H') return HitAnimationType.Hard;
			if (Char.ToUpper(s[0]) == 'B') return HitAnimationType.Back;
			if (Char.ToUpper(s[0]) == 'U') return HitAnimationType.Up;
			if (Char.ToUpper(s[0]) == 'D') return HitAnimationType.DiagUp;

			return Failure;
		}

		[StringConversion(typeof(PriorityType))]
		Object ToPriorityType(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "Dodge") == true) return PriorityType.Dodge;
			if (sc.Equals(s, "Hit") == true) return PriorityType.Hit;
			if (sc.Equals(s, "Miss") == true) return PriorityType.Miss;

			return Failure;
		}

		[StringConversion(typeof(Combat.HitPriority))]
		Object ToHitPriority(String s)
		{
			Match m = m_hitpriorityregex.Match(s);
			if (m.Success == true)
			{
				Int32 power;
				PriorityType pt;
				if (TryConvert(m.Groups[1].Value, out power) == false || TryConvert(m.Groups[2].Value, out pt) == false) return Failure;

				return new Combat.HitPriority(pt, power);
			}
			else
			{
				Int32 power;
				if (TryConvert(s, out power) == false) return Failure;

				return new Combat.HitPriority(PriorityType.Miss, power);
			}
		}

		[StringConversion(typeof(AttackEffect))]
		Object ToAttackEffect(String s)
		{
			if (s.Length == 0) return Failure;

			if (Char.ToUpper(s[0]) == 'H') return AttackEffect.High;
			if (Char.ToUpper(s[0]) == 'L') return AttackEffect.Low;
			if (Char.ToUpper(s[0]) == 'T') return AttackEffect.Trip;
			if (Char.ToUpper(s[0]) == 'N') return AttackEffect.None;

			return Failure;
		}

		[StringConversion(typeof(ForceFeedbackType))]
		Object ToForceFeedbackType(String s)
		{
			ForceFeedbackType fft = ForceFeedbackType.None;

			if (s.IndexOf("sine", StringComparison.OrdinalIgnoreCase) != -1) fft |= ForceFeedbackType.Sine;
			if (s.IndexOf("square", StringComparison.OrdinalIgnoreCase) != -1) fft |= ForceFeedbackType.Square;

			return fft;
		}

		[StringConversion(typeof(AffectTeam))]
		Object ToAffectTeam(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (s.StartsWith("F", StringComparison.OrdinalIgnoreCase) == true) return AffectTeam.Friendly;
			if (s.StartsWith("E", StringComparison.OrdinalIgnoreCase) == true) return AffectTeam.Enemy;
			if (s.StartsWith("B", StringComparison.OrdinalIgnoreCase) == true) return AffectTeam.Both;

			return Failure;
		}

		[StringConversion(typeof(BindToTargetPostion))]
		Object ToBindToTargetPostion(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "foot") == true) return BindToTargetPostion.Foot;
			if (sc.Equals(s, "head") == true) return BindToTargetPostion.Head;
			if (sc.Equals(s, "mid") == true) return BindToTargetPostion.Mid;

			return Failure;
		}

		[StringConversion(typeof(Backgrounds.BackgroundType))]
		Object ToBackgroundType(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "normal") == true) return Backgrounds.BackgroundType.Static;
			else if (sc.Equals(s, "parallax") == true) return Backgrounds.BackgroundType.Parallax;
			else if (sc.Equals(s, "anim") == true) return Backgrounds.BackgroundType.Animated;
			else return Backgrounds.BackgroundType.None;
		}

		[StringConversion(typeof(Assertion))]
		Object ToAssertion(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "intro") == true) return Assertion.Intro;
			if (sc.Equals(s, "invisible") == true) return Assertion.Invisible;
			if (sc.Equals(s, "roundnotover") == true) return Assertion.RoundNotOver;
			if (sc.Equals(s, "nobardisplay") == true) return Assertion.NoBarDisplay;
			if (sc.Equals(s, "noBG") == true) return Assertion.NoBackground;
			if (sc.Equals(s, "noFG") == true) return Assertion.NoForeground;
			if (sc.Equals(s, "nostandguard") == true) return Assertion.NoStandGuard;
			if (sc.Equals(s, "nocrouchguard") == true) return Assertion.NoCrouchGuard;
			if (sc.Equals(s, "noairguard") == true) return Assertion.NoAirGuard;
			if (sc.Equals(s, "noautoturn") == true) return Assertion.NoAutoturn;
			if (sc.Equals(s, "nojugglecheck") == true) return Assertion.NoJuggleCheck;
			if (sc.Equals(s, "nokosnd") == true) return Assertion.NoKOSound;
			if (sc.Equals(s, "nokoslow") == true) return Assertion.NoKOSlow;
			if (sc.Equals(s, "noshadow") == true) return Assertion.NoShadow;
			if (sc.Equals(s, "nomusic") == true) return Assertion.NoMusic;
			if (sc.Equals(s, "nowalk") == true) return Assertion.NoWalk;
			if (sc.Equals(s, "timerfreeze") == true) return Assertion.TimerFreeze;
			if (sc.Equals(s, "unguardable") == true) return Assertion.Unguardable;
			if (sc.Equals(s, "GlobalNoShadow") == true) return Assertion.GlobalNoShadow;
			if (sc.Equals(s, "NoKO") == true) return Assertion.NoKO;

			return Failure;
		}

		[StringConversion(typeof(ScreenShotFormat))]
		Object ToScreenShotFormat(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "bmp") == true) return ScreenShotFormat.Bmp;
			if (sc.Equals(s, "jpg") == true) return ScreenShotFormat.Jpg;
			if (sc.Equals(s, "png") == true) return ScreenShotFormat.Png;

			return Failure;
		}

		[StringConversion(typeof(Axis))]
		Object ToAxis(String s)
		{
			StringComparer sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "X") == true) return Axis.X;
			if (sc.Equals(s, "Y") == true) return Axis.Y;

			return Failure;
		}

		Object Failure
		{
			get { return m_failure; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyDictionary<Type, Converter<String, Object>> m_conversionmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_blendingregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_printdataregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_hitpriorityregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Object m_failure;

		#endregion
	}
}