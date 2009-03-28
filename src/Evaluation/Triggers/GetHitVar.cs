using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("GetHitVar")]
	class GetHitVar : Function
	{
		public GetHitVar(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		static GetHitVar()
		{
			HitVarMap = new Dictionary<String, Converter<Combat.Character, Number>>(StringComparer.OrdinalIgnoreCase);

			HitVarMap["xveladd"] = x => new Number();
			HitVarMap["yveladd"] = x => new Number();
			HitVarMap["type"] = GetHitType;
			HitVarMap["animtype"] = GetAnimType;
			HitVarMap["airtype"] = GetAirHitType;
			HitVarMap["groundtype"] = GetGroundHitType;
			HitVarMap["damage"] = GetDamage;
			HitVarMap["hitcount"] = GetHitCount;
			HitVarMap["fallcount"] = x => new Number();
			HitVarMap["hitshaketime"] = GetHitShakeTime;
			HitVarMap["hittime"] = GetHitTime;
			HitVarMap["slidetime"] = GetSlideTime;
			HitVarMap["ctrltime"] = GetGuardControlTime;
			HitVarMap["recovertime"] = x => new Number();
			HitVarMap["xoff"] = GetSnapX;
			HitVarMap["yoff"] = GetSnapY;
			HitVarMap["zoff"] = x => new Number();
			HitVarMap["xvel"] = x => GetHitVelocity(x, Axis.X);
			HitVarMap["yvel"] = x => GetHitVelocity(x, Axis.Y);
			HitVarMap["yaccel"] = GetYAccleration;
			HitVarMap["chainid"] = x => new Number(x.DefensiveInfo.HitDef.ChainId);
			HitVarMap["guarded"] = x => new Number(x.DefensiveInfo.Blocked);
			HitVarMap["isbound"] = GetIsBound;
			HitVarMap["fall"] = x => new Number(x.DefensiveInfo.IsFalling);
			HitVarMap["fall.damage"] = x => new Number(x.DefensiveInfo.HitDef.FallDamage);
			HitVarMap["fall.xvel"] = x => new Number(x.DefensiveInfo.HitDef.FallVelocityX ?? x.CurrentVelocity.X);
			HitVarMap["fall.yvel"] = x => new Number(x.DefensiveInfo.HitDef.FallVelocityY);
			HitVarMap["fall.recover"] = x => new Number(x.DefensiveInfo.HitDef.FallCanRecover);
			HitVarMap["fall.recovertime"] = x => new Number(x.DefensiveInfo.HitDef.FallRecoverTime);
			HitVarMap["fall.kill"] = x => new Number(x.DefensiveInfo.HitDef.CanFallKill);
			HitVarMap["fall.envshake.time"] = x => new Number(x.DefensiveInfo.HitDef.EnvShakeFallTime);
			HitVarMap["fall.envshake.freq"] = x => new Number(x.DefensiveInfo.HitDef.EnvShakeFallFrequency);
			HitVarMap["fall.envshake.ampl"] = x => new Number(x.DefensiveInfo.HitDef.EnvShakeFallAmplitude);
			HitVarMap["fall.envshake.phase"] = x => new Number(x.DefensiveInfo.HitDef.EnvShakeFallPhase);
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			String consttype = (String)Arguments[0];

			if (HitVarMap.ContainsKey(consttype) == true) return HitVarMap[consttype](character);
			return new Number();
		}

		static Number GetHitCount(Combat.Character character)
		{
			return new Number(character.DefensiveInfo.HitCount);
		}

		static Number GetHitVelocity(Combat.Character character, Axis axis)
		{
			if (axis == Axis.X)
			{
                return new Number(character.DefensiveInfo.HitVelocity.X);
			}

			if (axis == Axis.Y)
			{
                return new Number(character.DefensiveInfo.HitVelocity.Y);
			}

			return new Number();
		}

		static Number GetHitType(Combat.Character p)
		{
			if (p.Life == 0) return new Number(3);

			return (p.DefensiveInfo.HitStateType == xnaMugen.StateType.Airborne) ? GetAirHitType(p) : GetGroundHitType(p);
		}

		static Number GetAnimType(Combat.Character p)
		{
			HitAnimationType hat = HitAnimationType.Light;

			if (p.DefensiveInfo.IsFalling == true) hat = p.DefensiveInfo.HitDef.FallAnimationType;
			else if (p.DefensiveInfo.HitStateType == xnaMugen.StateType.Airborne) hat = p.DefensiveInfo.HitDef.AirAnimationType;
			else hat = p.DefensiveInfo.HitDef.GroundAnimationType;

			switch (hat)
			{
				case HitAnimationType.None:
					return new Number();

				case HitAnimationType.Light:
					return new Number(0);

				case HitAnimationType.Medium:
					return new Number(1);

				case HitAnimationType.Hard:
					return new Number(2);

				case HitAnimationType.Back:
					return new Number(3);

				case HitAnimationType.Up:
					return new Number(4);

				case HitAnimationType.DiagUp:
					return new Number(5);

				default:
					return new Number(0);
			}
		}

		static Number GetAirHitType(Combat.Character p)
		{
			switch (p.DefensiveInfo.HitDef.AirAttackEffect)
			{
				case AttackEffect.None:
					return new Number(0);

				case AttackEffect.High:
					return new Number(1);

				case AttackEffect.Low:
					return new Number(2);

				case AttackEffect.Trip:
					return new Number(3);

				default:
					return new Number(0);
			}
		}

		static Number GetGroundHitType(Combat.Character p)
		{
			switch (p.DefensiveInfo.HitDef.GroundAttackEffect)
			{
				case AttackEffect.None:
					return new Number(0);

				case AttackEffect.High:
					return new Number(1);

				case AttackEffect.Low:
					return new Number(2);

				case AttackEffect.Trip:
					return new Number(3);

				default:
					return new Number(0);
			}
		}

		static Number GetDamage(Combat.Character p)
		{
			return (p.DefensiveInfo.Blocked == true) ? new Number(p.DefensiveInfo.HitDef.GuardDamage) : new Number(p.DefensiveInfo.HitDef.HitDamage);
		}

		static Number GetHitShakeTime(Combat.Character p)
		{
			return new Number(p.DefensiveInfo.HitShakeTime);
		}

		static Number GetHitTime(Combat.Character p)
		{
			return new Number(p.DefensiveInfo.HitTime);
		}

		static Number GetSlideTime(Combat.Character p)
		{
			return (p.DefensiveInfo.Blocked == true) ? new Number(p.DefensiveInfo.HitDef.GuardSlideTime) : new Number(p.DefensiveInfo.HitDef.GroundSlideTime);
		}

		static Number GetGuardControlTime(Combat.Character p)
		{
			switch (p.DefensiveInfo.HitStateType)
			{
				case xnaMugen.StateType.Airborne:
					return new Number(p.DefensiveInfo.HitDef.AirGuardControlTime);

				default:
					return new Number(p.DefensiveInfo.HitDef.GuardControlTime);
			}
		}

		static Number GetSnapX(Combat.Character p)
		{
			return (p.DefensiveInfo.HitDef.SnapLocation != null) ? new Number(p.DefensiveInfo.HitDef.SnapLocation.Value.X) : new Number();
		}

		static Number GetSnapY(Combat.Character p)
		{
			return (p.DefensiveInfo.HitDef.SnapLocation != null) ? new Number(p.DefensiveInfo.HitDef.SnapLocation.Value.Y) : new Number();
		}

		static Number GetYAccleration(Combat.Character p)
		{
			return new Number(p.DefensiveInfo.HitDef.YAcceleration);
		}

		static Number GetIsBound(Combat.Character p)
		{
			Combat.CharacterBind bind = p.Bind;

			return new Number(bind.IsActive == true && bind.IsTargetBind == true);
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.LeftParen) return null;
			++parsestate.TokenIndex;

			if (parsestate.CurrentToken == null) return null;
			String constant = parsestate.CurrentToken.ToString();

			parsestate.BaseNode.Arguments.Add(constant);
			++parsestate.TokenIndex;

			if (parsestate.CurrentSymbol != Symbol.RightParen) return null;
			++parsestate.TokenIndex;

			return parsestate.BaseNode;
		}

		static Dictionary<String, Converter<Combat.Character, Number>> HitVarMap { get; set; }
	}
}
