namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("GetHitVar")]
	internal static class GetHitVar
	{
		[Tag("fall.envshake.time")]
		public static int GetFallShakeTime(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallTime;
		}

		[Tag("fall.envshake.freq")]
		public static float GetFallShakeFreq(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallFrequency;
		}

		[Tag("fall.envshake.ampl")]
		public static float GetFallShakeAmpl(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallAmplitude;
		}

		[Tag("fall.envshake.phase")]
		public static float GetFallShakePhase(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.EnvShakeFallPhase;
		}


		[Tag("guarded")]
		public static bool GetGuarded(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.Blocked;
		}

		[Tag("chainid")]
		public static int GetChainId(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.ChainId;
		}

		[Tag("fall")]
		public static bool GetFalling(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.IsFalling;
		}

		[Tag("fall.damage")]
		public static int GetFallDamage(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallDamage;
		}

		[Tag("fall.recover")]
		public static bool GetCanFallRecover(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.HitDef.FallCanRecover;
		}

		[Tag("fall.kill")]
		public static bool GetCanFallKill(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.HitDef.CanFallKill;
		}

		[Tag("fall.recovertime")]
		public static int GetFallRecoverTime(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallRecoverTime;
		}

		[Tag("fall.xvel")]
		public static float GetFallVelocityX(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallVelocityX ?? character.CurrentVelocity.X;
		}

		[Tag("fall.yvel")]
		public static float GetFallVelocityY(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.FallVelocityY;
		}

		[Tag("xveladd")]
		public static int GetVelocityAddX(object state, ref bool error)
		{
			error = true;
			return 0;
		}

		[Tag("yveladd")]
		public static int GetVelocityAddY(object state, ref bool error)
		{
			error = true;
			return 0;
		}

		[Tag("fallcount")]
		public static int GetFallCount(object state, ref bool error)
		{
			error = true;
			return 0;
		}

		[Tag("recovertime")]
		public static int GetRecoverTime(object state, ref bool error)
		{
			error = true;
			return 0;
		}

		[Tag("hitcount")]
		public static int GetHitCount(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitCount;
		}

		[Tag("xvel")]
		public static float GetHitVelocityX(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.GetHitVelocity().X;
		}

		[Tag("yvel")]
		public static float GetHitVelocityY(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.GetHitVelocity().Y;
		}

		[Tag("type")]
		public static int GetHitType(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			if (character.Life == 0) return 3;

			return character.DefensiveInfo.HitStateType == xnaMugen.StateType.Airborne ? GetAirHitType(state, ref error) : GetGroundHitType(state, ref error);
		}

		[Tag("animtype")]
		public static int GetAnimType(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var hat = HitAnimationType.Light;

			if (character.DefensiveInfo.IsFalling) hat = character.DefensiveInfo.HitDef.FallAnimationType;
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
		public static int GetAirHitType(object state, ref bool error)
		{
			var character = state as Combat.Character;
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
		public static int GetGroundHitType(object state, ref bool error)
		{
			var character = state as Combat.Character;
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
		public static int GetDamage(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.Blocked ? character.DefensiveInfo.HitDef.GuardDamage : character.DefensiveInfo.HitDef.HitDamage;
		}

		[Tag("hitshaketime")]
		public static int GetHitShakeTime(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitShakeTime;
		}

		[Tag("hittime")]
		public static int GetHitTime(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitTime;
		}

		[Tag("slidetime")]
		public static int GetSlideTime(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.Blocked ? character.DefensiveInfo.HitDef.GuardSlideTime : character.DefensiveInfo.HitDef.GroundSlideTime;
		}

		[Tag("ctrltime")]
		public static int GetGuardControlTime(object state, ref bool error)
		{
			var character = state as Combat.Character;
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
		public static int GetSnapX(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var location = character.DefensiveInfo.HitDef.SnapLocation;
			if (location.HasValue == false)
			{
				error = true;
				return 0;
			}

			return location.Value.X;
		}

		[Tag("yoff")]
		public static int GetSnapY(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var location = character.DefensiveInfo.HitDef.SnapLocation;
			if (location.HasValue == false)
			{
				error = true;
				return 0;
			}

			return location.Value.Y;
		}

		[Tag("zoff")]
		public static int GetSnapZ(object state, ref bool error)
		{
			error = true;
			return 0;
		}

		[Tag("yaccel")]
		public static float GetYAccleration(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.DefensiveInfo.HitDef.YAcceleration;
		}

		[Tag("isbound")]
		public static bool GetIsBound(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			var bind = character.Bind;

			return bind.IsActive && bind.IsTargetBind;
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.LeftParen) return null;
			++parsestate.TokenIndex;

			if (parsestate.CurrentToken == null) return null;
			var constant = parsestate.CurrentToken.ToString();

			parsestate.BaseNode.Arguments.Add(constant);
			++parsestate.TokenIndex;

			if (parsestate.CurrentSymbol != Symbol.RightParen) return null;
			++parsestate.TokenIndex;

			return parsestate.BaseNode;
		}
	}
}
