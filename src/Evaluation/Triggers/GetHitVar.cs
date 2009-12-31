using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("GetHitVar")]
	static class GetHitVar
	{
		[Tag("fall.envshake.time")]
		public static Int32 GetFallShakeTime(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallTime;
		}

		[Tag("fall.envshake.freq")]
		public static Single GetFallShakeFreq(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallFrequency;
		}

		[Tag("fall.envshake.ampl")]
		public static Single GetFallShakeAmpl(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallAmplitude;
		}

		[Tag("fall.envshake.phase")]
		public static Single GetFallShakePhase(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallPhase;
		}


		[Tag("guarded")]
		public static Boolean GetGuarded(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.Blocked;
		}

		[Tag("chainid")]
		public static Int32 GetChainId(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.ChainId;
		}

		[Tag("fall")]
		public static Boolean GetFalling(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.IsFalling;
		}

		[Tag("fall.damage")]
		public static Int32 GetFallDamage(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallDamage;
		}

		[Tag("fall.recover")]
		public static Boolean GetCanFallRecover(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.HitDef.FallCanRecover;
		}

		[Tag("fall.kill")]
		public static Boolean GetCanFallKill(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.HitDef.CanFallKill;
		}

		[Tag("fall.recovertime")]
		public static Int32 GetFallRecoverTime(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallRecoverTime;
		}

		[Tag("fall.xvel")]
		public static Single GetFallVelocityX(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallVelocityX ?? character.CurrentVelocity.X;
		}

		[Tag("fall.yvel")]
		public static Single GetFallVelocityY(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallVelocityY;
		}

		[Tag("xveladd")]
		public static Int32 GetVelocityAddX(Object state, ref Boolean error)
		{
			error = true;
			return 0;
		}

		[Tag("yveladd")]
		public static Int32 GetVelocityAddY(Object state, ref Boolean error)
		{
			error = true;
			return 0;
		}

		[Tag("fallcount")]
		public static Int32 GetFallCount(Object state, ref Boolean error)
		{
			error = true;
			return 0;
		}

		[Tag("recovertime")]
		public static Int32 GetRecoverTime(Object state, ref Boolean error)
		{
			error = true;
			return 0;
		}

		[Tag("hitcount")]
		public static Int32 GetHitCount(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitCount;
		}

		[Tag("xvel")]
		public static Single GetHitVelocityX(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.GetHitVelocity().X;
		}

		[Tag("yvel")]
		public static Single GetHitVelocityY(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.GetHitVelocity().Y;
		}

		[Tag("type")]
		public static Int32 GetHitType(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			if (character.Life == 0) return 3;

			return (character.DefensiveInfo.HitStateType == xnaMugen.StateType.Airborne) ? GetAirHitType(state, ref error) : GetGroundHitType(state, ref error);
		}

		[Tag("animtype")]
		public static Int32 GetAnimType(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			HitAnimationType hat = HitAnimationType.Light;

			if (character.DefensiveInfo.IsFalling == true) hat = character.DefensiveInfo.HitDef.FallAnimationType;
			else if (character.DefensiveInfo.HitStateType == xnaMugen.StateType.Airborne) hat = character.DefensiveInfo.HitDef.AirAnimationType;
			else hat = character.DefensiveInfo.HitDef.GroundAnimationType;

			switch (hat)
			{
				case HitAnimationType.Light:
					return 0;

				case HitAnimationType.Medium:
					return 1;

				case HitAnimationType.Hard:
					return 2;

				case HitAnimationType.Back:
					return 3;

				case HitAnimationType.Up:
					return 4;

				case HitAnimationType.DiagUp:
					return 5;

				default:
					error = true;
					return 0;
			}
		}

		[Tag("airtype")]
		public static Int32 GetAirHitType(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (character.DefensiveInfo.HitDef.AirAttackEffect)
			{
				case AttackEffect.High:
					return 1;

				case AttackEffect.Low:
					return 2;

				case AttackEffect.Trip:
					return 3;

				default:
					return 4;
			}
		}

		[Tag("groundtype")]
		public static Int32 GetGroundHitType(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (character.DefensiveInfo.HitDef.GroundAttackEffect)
			{
				case AttackEffect.High:
					return 1;

				case AttackEffect.Low:
					return 2;

				case AttackEffect.Trip:
					return 3;

				default:
					return 0;
			}
		}

		[Tag("damage")]
		public static Int32 GetDamage(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return (character.DefensiveInfo.Blocked == true) ? character.DefensiveInfo.HitDef.GuardDamage : character.DefensiveInfo.HitDef.HitDamage;
		}

		[Tag("hitshaketime")]
		public static Int32 GetHitShakeTime(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitShakeTime;
		}

		[Tag("hittime")]
		public static Int32 GetHitTime(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitTime;
		}

		[Tag("slidetime")]
		public static Int32 GetSlideTime(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return (character.DefensiveInfo.Blocked == true) ? character.DefensiveInfo.HitDef.GuardSlideTime : character.DefensiveInfo.HitDef.GroundSlideTime;
		}

		[Tag("ctrltime")]
		public static Int32 GetGuardControlTime(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (character.DefensiveInfo.HitStateType)
			{
				case xnaMugen.StateType.Airborne:
					return character.DefensiveInfo.HitDef.AirGuardControlTime;

				default:
					return character.DefensiveInfo.HitDef.GuardControlTime;
			}
		}

		[Tag("xoff")]
		public static Int32 GetSnapX(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Point? location = character.DefensiveInfo.HitDef.SnapLocation;
			if (location.HasValue == false)
			{
				error = true;
				return 0;
			}

			return location.Value.X;
		}

		[Tag("yoff")]
		public static Int32 GetSnapY(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Point? location = character.DefensiveInfo.HitDef.SnapLocation;
			if (location.HasValue == false)
			{
				error = true;
				return 0;
			}

			return location.Value.Y;
		}

		[Tag("zoff")]
		public static Int32 GetSnapZ(Object state, ref Boolean error)
		{
			error = true;
			return 0;
		}

		[Tag("yaccel")]
		public static Single GetYAccleration(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.YAcceleration;
		}

		[Tag("isbound")]
		public static Boolean GetIsBound(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Combat.CharacterBind bind = character.Bind;

			return bind.IsActive == true && bind.IsTargetBind == true;
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
	}
}
