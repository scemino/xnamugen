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
using System.Linq;
using xnaMugen.Combat;
using xnaMugen.Evaluation;

namespace xnaMugen
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class StringConversionAttribute : Attribute
	{
		public StringConversionAttribute(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			m_type = type;
		}

		public Type Type => m_type;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type m_type;

		#endregion
	}

	internal class StringConverter : SubSystem
	{
		public StringConverter(SubSystems subsystems)
			: base(subsystems)
		{
			m_blendingregex = new Regex(@"^AS(\d+)d(\d+)$", RegexOptions.IgnoreCase);
			m_printdataregex = new Regex(@"^(\d+)\s*,\s*(\d+)\s*,?\s*?(-?\d+)?$", RegexOptions.IgnoreCase);
			m_hitpriorityregex = new Regex(@"^(\d+),\s*(\w+)$", RegexOptions.IgnoreCase);
			m_failure = new object();
			m_conversionmap = BuildConversionMap();
		}

		private ReadOnlyDictionary<Type, Converter<string, object>> BuildConversionMap()
		{
			var conversionmap = new Dictionary<Type, Converter<string, object>>();

			foreach (var mi in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				var attrib = (StringConversionAttribute)Attribute.GetCustomAttribute(mi, typeof(StringConversionAttribute));
				if (attrib == null) continue;

				var d = (Converter<string, object>)Delegate.CreateDelegate(typeof(Converter<string, object>), this, mi);
				conversionmap.Add(attrib.Type, d);

				if (attrib.Type.IsValueType)
				{
					var nullableType = typeof(Nullable<>).MakeGenericType(attrib.Type);
					conversionmap.Add(nullableType, d);
				}
			}

			return new ReadOnlyDictionary<Type, Converter<string, object>>(conversionmap);
		}

		public T Convert<T>(string str)
		{
			if (str == null) throw new ArgumentNullException(nameof(str));

			if (TryConvert(str, out T output)) return output;

			Log.Write(LogLevel.Warning, LogSystem.StringConverter, "Cannot convert '{0}' to type: {1}", str, typeof(T).Name);
			return default(T);
		}

		public bool TryConvert<T>(string str, out T output)
		{
			if (m_conversionmap.TryGetValue(typeof(T), out var converter) == false)
			{
				output = default(T);
				return false;
			}

			var obj = converter(str);
			if (obj == Failure)
			{
				output = default(T);
				return false;
			}

			output = (T)obj;
			return true;
		}

		[StringConversion(typeof(Keys))]
		private object ToKeys(string s)
		{
			if (!Enum.TryParse(s, true, out Keys key)) return Failure;
			return key;
		}

		[StringConversion(typeof(string))]
		private object ToString(string s)
		{
			return s;
		}

		[StringConversion(typeof(CombatMode))]
		private object ToCombatMode(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "None")) return CombatMode.None;
			if (sc.Equals(s, "Versus")) return CombatMode.Versus;
			if (sc.Equals(s, "TeamArcade")) return CombatMode.TeamArcade;
			if (sc.Equals(s, "TeamVersus")) return CombatMode.TeamVersus;
			if (sc.Equals(s, "TeamCoop")) return CombatMode.TeamCoop;
			if (sc.Equals(s, "Survival")) return CombatMode.Survival;
			if (sc.Equals(s, "SurvivalCoop")) return CombatMode.SurvivalCoop;
			if (sc.Equals(s, "Training")) return CombatMode.Training;

			return Failure;
		}

		[StringConversion(typeof(Rectangle))]
		private object ToRectangle(string s)
		{
			var expression = (Expression)ToExpression(s);

			var rectangle = EvaluationHelper.AsRectangle(null, expression, null);
			if (rectangle == null) return Failure;

			return rectangle.Value;
		}

		[StringConversion(typeof(BackgroundLayer))]
		private object ToBackgroundLayer(string s)
		{
			if (TryConvert(s, out int layernumber) == false) return Failure;

			if (layernumber == 0) return BackgroundLayer.Back;
			if (layernumber == 1) return BackgroundLayer.Front;

			return Failure;
		}

		[StringConversion(typeof(int))]
		private object ToInt32(string s)
		{
            s = RemoveTrailingGarbage(s);
			if (int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var val)) return val;

			return Failure;
		}

        [StringConversion(typeof(float))]
        private object ToSingle(string s)
        {
            s = RemoveTrailingGarbage(s);
            if (float.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var val)) return val;

			return Failure;
		}
		
		private static string RemoveTrailingGarbage(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;
			for (var i = 0; i < s.Length; i++)
			{
				if (!char.IsDigit(s[i]) && s[i] != '.' && s[i] != '+' && s[i] != '-' && s[i] != 'e' && s[i] != 'E')
				{
					return s.Substring(0, i);
				}
			}
			return s;
		}

		[StringConversion(typeof(bool))]
		private object ToBoolean(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals("true", s)) return true;
			if (sc.Equals("false", s)) return false;

			if (TryConvert(s, out int intvalue))
			{
				return intvalue > 0;
			}

			return Failure;
		}

		[StringConversion(typeof(Expression))]
		private object ToExpression(string s)
		{
			return GetSubSystem<EvaluationSystem>().CreateExpression(s);
		}

		[StringConversion(typeof(PrefixedExpression))]
		private object ToPrefixedExpression(string s)
		{
			return GetSubSystem<EvaluationSystem>().CreatePrefixedExpression(s);
		}

		[StringConversion(typeof(Point))]
		private object ToPoint(string s)
		{
			if (TryConvert(s, out Expression expression) == false) return Failure;

			var point = EvaluationHelper.AsPoint(null, expression, null);
			if (point == null) return Failure;

			return point.Value;
		}

		[StringConversion(typeof(Vector2))]
		private object ToVector2(string s)
		{
			if (TryConvert(s, out Expression expression) == false) return Failure;

			var vector = EvaluationHelper.AsVector2(null, expression, null);
			if (vector == null) return Failure;

			return vector.Value;
		}

        [StringConversion(typeof(Vector3))]
        private object ToVector3(string s)
        {
            if (TryConvert(s, out Expression expression) == false) return Failure;

            var vector = EvaluationHelper.AsVector3(null, expression, null);
            if (vector == null) return Failure;

            return vector.Value;
        }

		[StringConversion(typeof(Blending))]
		private object ToBlending(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return new Blending();
			}
			
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "none"))
			{
				return new Blending();
			}

			if (sc.Equals(s, "addalpha"))
			{
				return new Blending(BlendType.Add, 0, 0);
			}

			if (sc.Equals(s, "add") || sc.Equals(s, "a"))
			{
				return new Blending(BlendType.Add, 255, 255);
			}

			if (sc.Equals(s, "add1") || sc.Equals(s, "a1"))
			{
				return new Blending(BlendType.Add, 255, 127);
			}

			if (char.ToUpper(s[0]) == 'S')
			{
				return new Blending(BlendType.Subtract, 255, 255);
			}

			var m = m_blendingregex.Match(s);
			if (!m.Success) return Failure;
			
			if (int.TryParse(m.Groups[1].Value, out var source) && int.TryParse(m.Groups[2].Value, out var destination))
			{
				return new Blending(BlendType.Add, source, destination);
			}

			return Failure;
		}

		[StringConversion(typeof(SpriteId))]
		private object ToSpriteId(string s)
		{
			if (TryConvert(s, out Point p) == false) return Failure;

			return new SpriteId(p.X, p.Y);
		}

		[StringConversion(typeof(Drawing.PrintData))]
		private object ToPrintData(string s)
		{
			var m = m_printdataregex.Match(s);
			if (m.Success == false) return Failure;

			var index = int.Parse(m.Groups[1].Value);
			var color = int.Parse(m.Groups[2].Value);
			var justification = PrintJustification.Center;

			if (m.Groups[3].Value == "")
			{
				justification = PrintJustification.Center;
			}
			else
			{
				var just = int.Parse(m.Groups[3].Value);

				if (just == 0) justification = PrintJustification.Center;
				if (just > 0) justification = PrintJustification.Left;
				if (just < 0) justification = PrintJustification.Right;
			}

			return new Drawing.PrintData(index, color, justification);
		}

		[StringConversion(typeof(SoundId))]
		private object ToSoundId(string s)
		{
			if (TryConvert(s, out Point p) == false) return Failure;

			return new SoundId(p.X, p.Y);
		}

		[StringConversion(typeof(Facing))]
		private object ToFacing(string s)
		{
			if (TryConvert(s, out int intvalue) == false) return Failure;

			return intvalue >= 0 ? Facing.Right : Facing.Left;
		}

		[StringConversion(typeof(AttackStateType))]
		private object ToAttackStateType(string s)
		{
			var ast = AttackStateType.None;

			if (s.IndexOf("s", StringComparison.OrdinalIgnoreCase) != -1) ast |= AttackStateType.Standing;
			if (s.IndexOf("c", StringComparison.OrdinalIgnoreCase) != -1) ast |= AttackStateType.Crouching;
			if (s.IndexOf("a", StringComparison.OrdinalIgnoreCase) != -1) ast |= AttackStateType.Air;

			if (ast == AttackStateType.None && s != string.Empty) return Failure;

			return ast;
		}

		[StringConversion(typeof(PositionType))]
		private object ToPositionType(string s)
		{
			var sc = StringComparer.InvariantCultureIgnoreCase;

			if (sc.Equals(s, "p1")) return PositionType.P1;
			if (sc.Equals(s, "p2")) return PositionType.P2;
			if (sc.Equals(s, "front")) return PositionType.Front;
			if (sc.Equals(s, "back")) return PositionType.Back;
			if (sc.Equals(s, "left")) return PositionType.Left;
			if (sc.Equals(s, "right")) return PositionType.Right;

			return Failure;
		}

		[StringConversion(typeof(HitType))]
		private object ToHitType(string s)
		{
			if (s.Length > 2) return Failure;

			AttackClass aclass;
			AttackPower apower;

			switch (char.ToUpper(s[0]))
			{
				case 'N':
					apower = AttackPower.Normal;
					break;
				case 'H':
					apower = AttackPower.Hyper;
					break;
				case 'S':
					apower = AttackPower.Special;
					break;
				case 'A':
					apower = AttackPower.All;
					break;
				default:
					return Failure;
			}

			if (s.Length == 1) aclass = AttackClass.All;
			else
			{
				switch (char.ToUpper(s[1]))
				{
					case 'T':
						aclass = AttackClass.Throw;
						break;
					case 'P':
						aclass = AttackClass.Projectile;
						break;
					case 'A':
						aclass = AttackClass.Normal;
						break;
					default:
						return Failure;
				}
			}

			return new HitType(aclass, apower);
		}

		[StringConversion(typeof(HitAttribute))]
		private object ToHitAttribute(string s)
		{
			var attackheight = AttackStateType.None;
			var attackdata = new List<HitType>();

			var first = true;
			foreach (var str in Regex.Split(s, @"\s*,\s*", RegexOptions.IgnoreCase))
			{
				if (first)
				{
					first = false;

					if (TryConvert(str, out attackheight) == false) return Failure;
				}
				else
				{
					if (TryConvert(str, out HitType hittype) == false) return Failure;

					attackdata.Add(hittype);
				}
			}

			return new HitAttribute(attackheight, new ReadOnlyList<HitType>(attackdata));
		}

		[StringConversion(typeof(SpriteEffects))]
		private object ToSpriteEffects(string s)
		{
			if (TryConvert(s, out int intvalue) == false) return Failure;

			return intvalue >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		}

		[StringConversion(typeof(StateType))]
		private object ToStateType(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "a")) return StateType.Airborne;
			if (sc.Equals(s, "c")) return StateType.Crouching;
			if (sc.Equals(s, "s")) return StateType.Standing;
			if (sc.Equals(s, "l")) return StateType.Prone;
			if (sc.Equals(s, "u")) return StateType.Unchanged;

			return Failure;
		}

		[StringConversion(typeof(HelperType))]
		private object ToHelperType(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "Normal")) return HelperType.Normal;
			if (sc.Equals(s, "Player")) return HelperType.Player;

#warning Temporary Hack
			if (sc.Equals(s, "Projectile")) return HelperType.Normal;

			return Failure;
		}

		[StringConversion(typeof(MoveType))]
		private object ToMoveType(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "a")) return MoveType.Attack;
			if (sc.Equals(s, "i")) return MoveType.Idle;
			if (sc.Equals(s, "h")) return MoveType.BeingHit;
			if (sc.Equals(s, "u")) return MoveType.Unchanged;

			return Failure;
		}

		[StringConversion(typeof(Physics))]
		private object ToPhsyics(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "s")) return Physics.Standing;
			if (sc.Equals(s, "c")) return Physics.Crouching;
			if (sc.Equals(s, "a")) return Physics.Airborne;
			if (sc.Equals(s, "n")) return Physics.None;
			if (sc.Equals(s, "u")) return Physics.Unchanged;

			return Failure;
		}

		[StringConversion(typeof(HitFlag))]
		private object ToHitFlag(string s)
		{
			var high = false;
			var low = false;
			var air = false;
			var falling = false;
			var down = false;
			var combo = HitFlagCombo.DontCare;

			if (s.IndexOf("H", StringComparison.InvariantCultureIgnoreCase) != -1) high = true;
			if (s.IndexOf("L", StringComparison.InvariantCultureIgnoreCase) != -1) low = true;
			if (s.IndexOf("M", StringComparison.InvariantCultureIgnoreCase) != -1) { high = true; low = true; }
			if (s.IndexOf("A", StringComparison.InvariantCultureIgnoreCase) != -1) air = true;
			if (s.IndexOf("D", StringComparison.InvariantCultureIgnoreCase) != -1) down = true;
			if (s.IndexOf("F", StringComparison.InvariantCultureIgnoreCase) != -1) falling = true;
			if (s.IndexOf("+", StringComparison.InvariantCultureIgnoreCase) != -1) combo = HitFlagCombo.Yes;
			if (s.IndexOf("-", StringComparison.InvariantCultureIgnoreCase) != -1) combo = HitFlagCombo.No;

			return new HitFlag(combo, high, low, air, falling, down);
		}

		[StringConversion(typeof(HitAnimationType))]
		private object ToHitAnimationType(string s)
		{
			if (s.Length == 0) return Failure;

			if (char.ToUpper(s[0]) == 'L') return HitAnimationType.Light;
			if (char.ToUpper(s[0]) == 'M') return HitAnimationType.Medium;
			if (char.ToUpper(s[0]) == 'H') return HitAnimationType.Hard;
			if (char.ToUpper(s[0]) == 'B') return HitAnimationType.Back;
			if (char.ToUpper(s[0]) == 'U') return HitAnimationType.Up;
			if (char.ToUpper(s[0]) == 'D') return HitAnimationType.DiagUp;

			return Failure;
		}

		[StringConversion(typeof(PriorityType))]
		private object ToPriorityType(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "Dodge")) return PriorityType.Dodge;
			if (sc.Equals(s, "Hit")) return PriorityType.Hit;
			if (sc.Equals(s, "Miss")) return PriorityType.Miss;

			return Failure;
		}

		[StringConversion(typeof(HitPriority))]
		private object ToHitPriority(string s)
		{
			var m = m_hitpriorityregex.Match(s);
			if (m.Success)
			{
				if (TryConvert(m.Groups[1].Value, out int power) == false || TryConvert(m.Groups[2].Value, out PriorityType pt) == false) return Failure;

				return new HitPriority(pt, power);
			}
			else
			{
				if (TryConvert(s, out int power) == false) return Failure;

				return new HitPriority(PriorityType.Miss, power);
			}
		}

		[StringConversion(typeof(AttackEffect))]
		private object ToAttackEffect(string s)
		{
			if (s.Length == 0) return Failure;

			if (char.ToUpper(s[0]) == 'H') return AttackEffect.High;
			if (char.ToUpper(s[0]) == 'L') return AttackEffect.Low;
			if (char.ToUpper(s[0]) == 'T') return AttackEffect.Trip;
			if (char.ToUpper(s[0]) == 'N') return AttackEffect.None;

			return Failure;
		}

		[StringConversion(typeof(ForceFeedbackType))]
		private object ToForceFeedbackType(string s)
		{
			var fft = ForceFeedbackType.None;

			if (s.IndexOf("sine", StringComparison.OrdinalIgnoreCase) != -1) fft |= ForceFeedbackType.Sine;
			if (s.IndexOf("square", StringComparison.OrdinalIgnoreCase) != -1) fft |= ForceFeedbackType.Square;

			return fft;
		}

		[StringConversion(typeof(AffectTeam))]
		private object ToAffectTeam(string s)
		{
			if (s.StartsWith("F", StringComparison.OrdinalIgnoreCase)) return AffectTeam.Friendly;
			if (s.StartsWith("E", StringComparison.OrdinalIgnoreCase)) return AffectTeam.Enemy;
			if (s.StartsWith("B", StringComparison.OrdinalIgnoreCase)) return AffectTeam.Both;

			return Failure;
		}

		[StringConversion(typeof(BindToTargetPostion))]
		private object ToBindToTargetPostion(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "foot")) return BindToTargetPostion.Foot;
			if (sc.Equals(s, "head")) return BindToTargetPostion.Head;
			if (sc.Equals(s, "mid")) return BindToTargetPostion.Mid;

			return Failure;
		}

		[StringConversion(typeof(Backgrounds.BackgroundType))]
		private object ToBackgroundType(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "normal")) return Backgrounds.BackgroundType.Static;
			if (sc.Equals(s, "parallax")) return Backgrounds.BackgroundType.Parallax;
			if (sc.Equals(s, "anim")) return Backgrounds.BackgroundType.Animated;
			return Backgrounds.BackgroundType.None;
		}

		[StringConversion(typeof(Assertion))]
		private object ToAssertion(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "intro")) return Assertion.Intro;
			if (sc.Equals(s, "invisible")) return Assertion.Invisible;
			if (sc.Equals(s, "roundnotover")) return Assertion.RoundNotOver;
			if (sc.Equals(s, "nobardisplay")) return Assertion.NoBarDisplay;
			if (sc.Equals(s, "noBG")) return Assertion.NoBackground;
			if (sc.Equals(s, "noFG")) return Assertion.NoForeground;
			if (sc.Equals(s, "nostandguard")) return Assertion.NoStandGuard;
			if (sc.Equals(s, "nocrouchguard")) return Assertion.NoCrouchGuard;
			if (sc.Equals(s, "noairguard")) return Assertion.NoAirGuard;
			if (sc.Equals(s, "noautoturn")) return Assertion.NoAutoturn;
			if (sc.Equals(s, "nojugglecheck")) return Assertion.NoJuggleCheck;
			if (sc.Equals(s, "nokosnd")) return Assertion.NoKOSound;
			if (sc.Equals(s, "nokoslow")) return Assertion.NoKOSlow;
			if (sc.Equals(s, "noshadow")) return Assertion.NoShadow;
			if (sc.Equals(s, "nomusic")) return Assertion.NoMusic;
			if (sc.Equals(s, "nowalk")) return Assertion.NoWalk;
			if (sc.Equals(s, "timerfreeze")) return Assertion.TimerFreeze;
			if (sc.Equals(s, "unguardable")) return Assertion.Unguardable;
			if (sc.Equals(s, "GlobalNoShadow")) return Assertion.GlobalNoShadow;
			if (sc.Equals(s, "NoKO")) return Assertion.NoKO;

			return Failure;
		}

		[StringConversion(typeof(ScreenShotFormat))]
		private object ToScreenShotFormat(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "bmp")) return ScreenShotFormat.Bmp;
			if (sc.Equals(s, "jpg")) return ScreenShotFormat.Jpg;
			if (sc.Equals(s, "png")) return ScreenShotFormat.Png;

			return Failure;
		}

		[StringConversion(typeof(Axis))]
		private object ToAxis(string s)
		{
			var sc = StringComparer.OrdinalIgnoreCase;

			if (sc.Equals(s, "X")) return Axis.X;
			if (sc.Equals(s, "Y")) return Axis.Y;

			return Failure;
		}

		private object Failure => m_failure;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyDictionary<Type, Converter<string, object>> m_conversionmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_blendingregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_printdataregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_hitpriorityregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object m_failure;

		#endregion
	}
}