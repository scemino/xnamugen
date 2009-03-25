using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	class HitDefinition
	{
		public HitDefinition()
		{
			Reset();
		}

		public void Reset()
		{
			HitAttribute = HitAttribute.Default;

			HitFlag = HitFlag.Default;
			GuardFlag = HitFlag.Default;
			Targeting = AffectTeam.Enemy;

			GroundAnimationType = HitAnimationType.Light;
			AirAnimationType = HitAnimationType.Light;
			FallAnimationType = HitAnimationType.Back;

			HitPriority = HitPriority.Default;

			HitDamage = 0;
			GuardDamage = 0;
		}

		public void Set(HitDefinition other)
		{
			if (other == null) throw new ArgumentNullException("other");

			HitAttribute = other.HitAttribute;
			HitFlag = other.HitFlag;
			GuardFlag = other.GuardFlag;
			Targeting = other.Targeting;
			GroundAnimationType = other.GroundAnimationType;
			AirAnimationType = other.AirAnimationType;
			FallAnimationType = other.FallAnimationType;
			HitPriority = other.HitPriority;
			HitDamage = other.HitDamage;
			GuardDamage = other.GuardDamage;
			PauseTime = other.PauseTime;
			ShakeTime = other.ShakeTime;
			GuardPauseTime = other.GuardPauseTime;
			GuardShakeTime = other.GuardShakeTime;
			PlayerSpark = other.PlayerSpark;
			SparkAnimation = other.SparkAnimation;
			GuardPlayerSpark = other.GuardPlayerSpark;
			GuardSparkAnimation = other.GuardSparkAnimation;
			SparkStartPosition = other.SparkStartPosition;
			PlayerSound = other.PlayerSound;
			HitSoundId = other.HitSoundId;
			GuardPlayerSound = other.GuardPlayerSound;
			GuardSoundId = other.GuardSoundId;
			GroundAttackEffect = other.GroundAttackEffect;
			AirAttackEffect = other.AirAttackEffect;
			GroundHitTime = other.GroundHitTime;
			DownHitTime = other.DownHitTime;
			AirHitTime = other.AirHitTime;
			GuardHitTime = other.GuardHitTime;
			GroundSlideTime = other.GroundSlideTime;
			GuardSlideTime = other.GuardSlideTime;
			GuardControlTime = other.GuardControlTime;
			AirGuardControlTime = other.AirGuardControlTime;
			GuardDistance = other.GuardDistance;
			YAcceleration = other.YAcceleration;
			GroundVelocity = other.GroundVelocity;
			AirVelocity = other.AirVelocity;
			DownVelocity = other.DownVelocity;
			GroundGuardVelocity = other.GroundGuardVelocity;
			AirGuardVelocity = other.AirGuardVelocity;
			GroundCornerPush = other.GroundCornerPush;
			AirCornerPush = other.AirCornerPush;
			DownCornerPush = other.DownCornerPush;
			GuardCornerPush = other.GuardCornerPush;
			AirGuardCornerPush = other.AirGuardCornerPush;
			JugglePointsNeeded = other.JugglePointsNeeded;
			MininumDistance = other.MininumDistance;
			MaximumDistance = other.MaximumDistance;
			SnapLocation = other.SnapLocation;
			P1SpritePriority = other.P1SpritePriority;
			P2SpritePriority = other.P2SpritePriority;
			P1Facing = other.P1Facing;
			P1GetP2Facing = other.P1GetP2Facing;
			P2Facing = other.P2Facing;
			P1NewState = other.P1NewState;
			P2UseP1State = other.P2UseP1State;
			P2NewState = other.P2NewState;
			ForceStand = other.ForceStand;
			Fall = other.Fall;
			AirFall = other.AirFall;
			FallVelocityX = other.FallVelocityX;
			FallVelocityY = other.FallVelocityY;
			FallCanRecover = other.FallCanRecover;
			FallRecoverTime = other.FallRecoverTime;
			FallDamage = other.FallDamage;
			DownBounce = other.DownBounce;
			TargetId = other.TargetId;
			ChainId = other.ChainId;
			NoChainId1 = other.NoChainId1;
			NoChainId2 = other.NoChainId2;
			HitOnce = other.HitOnce;
			CanKill = other.CanKill;
			CanGuardKill = other.CanGuardKill;
			CanFallKill = other.CanFallKill;
			NumberOfHits = other.NumberOfHits;
			P1HitPowerAdjustment = other.P1HitPowerAdjustment;
			P1GuardPowerAdjustment = other.P1GuardPowerAdjustment;
			P2HitPowerAdjustment = other.P2HitPowerAdjustment;
			P2GuardPowerAdjustment = other.P2GuardPowerAdjustment;
			PalFxTime = other.PalFxTime;
			PalFxAdd = other.PalFxAdd;
			PalFxMul = other.PalFxMul;
			PalFxSinAdd = other.PalFxSinAdd;
			PalFxInvert = other.PalFxInvert;
			PalFxBaseColor = other.PalFxBaseColor;
			EnvShakeTime = other.EnvShakeTime;
			EnvShakeFrequency = other.EnvShakeFrequency;
			EnvShakeAmplitude = other.EnvShakeAmplitude;
			EnvShakePhase = other.EnvShakePhase;
			EnvShakeFallTime = other.EnvShakeFallTime;
			EnvShakeFallFrequency = other.EnvShakeFallFrequency;
			EnvShakeFallAmplitude = other.EnvShakeFallAmplitude;
			EnvShakeFallPhase = other.EnvShakeFallPhase;
		}

		public HitAttribute HitAttribute { get; set; }

		public HitFlag HitFlag { get; set; }
		public HitFlag GuardFlag { get; set; }
		public AffectTeam Targeting { get; set; }

		public HitAnimationType GroundAnimationType { get; set; }
		public HitAnimationType AirAnimationType { get; set; }
		public HitAnimationType FallAnimationType { get; set; }

		public HitPriority HitPriority { get; set; }

		public Int32 HitDamage { get; set; }
		public Int32 GuardDamage { get; set; }

		public Int32 PauseTime { get; set; }
		public Int32 ShakeTime { get; set; }

		public Int32 GuardPauseTime { get; set; }
		public Int32 GuardShakeTime { get; set; }

		public Boolean PlayerSpark { get; set; }
		public Int32 SparkAnimation { get; set; }
		public Boolean GuardPlayerSpark { get; set; }
		public Int32 GuardSparkAnimation { get; set; }
		public Vector2 SparkStartPosition { get; set; }

		public Boolean PlayerSound { get; set; }
		public SoundId HitSoundId { get; set; }
		public Boolean GuardPlayerSound { get; set; }
		public SoundId GuardSoundId { get; set; }

		public AttackEffect GroundAttackEffect { get; set; }
		public AttackEffect AirAttackEffect { get; set; }
		public Int32 GroundHitTime { get; set; }
		public Int32 DownHitTime { get; set; }
		public Int32 AirHitTime { get; set; }
		public Int32 GuardHitTime { get; set; }
		public Int32 GroundSlideTime { get; set; }
		public Int32 GuardSlideTime { get; set; }
		public Int32 GuardControlTime { get; set; }
		public Int32 AirGuardControlTime { get; set; }
		public Int32 GuardDistance { get; set; }
		public Single YAcceleration { get; set; }
		public Vector2 GroundVelocity { get; set; }
		public Vector2 AirVelocity { get; set; }
		public Vector2 DownVelocity { get; set; }
		public Vector2 GroundGuardVelocity { get; set; }
		public Vector2 AirGuardVelocity { get; set; }
		public Single GroundCornerPush { get; set; }
		public Single AirCornerPush { get; set; }
		public Single DownCornerPush { get; set; }
		public Single GuardCornerPush { get; set; }
		public Single AirGuardCornerPush { get; set; }
		public Int32 JugglePointsNeeded { get; set; }
		public Point? MininumDistance { get; set; }
		public Point? MaximumDistance { get; set; }
		public Point? SnapLocation { get; set; }

		public Int32 P1SpritePriority { get; set; }
		public Int32 P2SpritePriority { get; set; }

		public Int32 P1Facing { get; set; }
		public Int32 P1GetP2Facing { get; set; }
		public Int32 P2Facing { get; set; }

		public Int32? P1NewState { get; set; }
		public Boolean P2UseP1State { get; set; }
		public Int32? P2NewState { get; set; }

		public Boolean ForceStand { get; set; }
		public Boolean Fall { get; set; }
		public Boolean AirFall { get; set; }
		public Single? FallVelocityX { get; set; }
		public Single FallVelocityY { get; set; }
		public Boolean FallCanRecover { get; set; }
		public Int32 FallRecoverTime { get; set; }
		public Int32 FallDamage { get; set; }
		public Boolean DownBounce { get; set; }

		public Int32 TargetId { get; set; }
		public Int32 ChainId { get; set; }
		public Int32 NoChainId1 { get; set; }
		public Int32 NoChainId2 { get; set; }

		public Boolean HitOnce { get; set; }
		public Boolean CanKill { get; set; }
		public Boolean CanGuardKill { get; set; }
		public Boolean CanFallKill { get; set; }

		public Int32 NumberOfHits { get; set; }

		public Int32 P1HitPowerAdjustment { get; set; }
		public Int32 P1GuardPowerAdjustment { get; set; }
		public Int32 P2HitPowerAdjustment { get; set; }
		public Int32 P2GuardPowerAdjustment { get; set; }

		public Int32 PalFxTime { get; set; }
		public Vector3 PalFxAdd { get; set; }
		public Vector3 PalFxMul { get; set; }
		public Vector4 PalFxSinAdd { get; set; }
		public Boolean PalFxInvert { get; set; }
		public Single PalFxBaseColor { get; set; }

		public Int32 EnvShakeTime { get; set; }
		public Single EnvShakeFrequency { get; set; }
		public Int32 EnvShakeAmplitude { get; set; }
		public Single EnvShakePhase { get; set; }

		public Int32 EnvShakeFallTime { get; set; }
		public Single EnvShakeFallFrequency { get; set; }
		public Int32 EnvShakeFallAmplitude { get; set; }
		public Single EnvShakeFallPhase { get; set; }
	}
}