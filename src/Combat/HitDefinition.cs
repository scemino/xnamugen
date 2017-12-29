using System;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class HitDefinition
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
			if (other == null) throw new ArgumentNullException(nameof(other));

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

		public int HitDamage { get; set; }
		public int GuardDamage { get; set; }

		public int PauseTime { get; set; }
		public int ShakeTime { get; set; }

		public int GuardPauseTime { get; set; }
		public int GuardShakeTime { get; set; }

		public bool PlayerSpark { get; set; }
		public int SparkAnimation { get; set; }
		public bool GuardPlayerSpark { get; set; }
		public int GuardSparkAnimation { get; set; }
		public Vector2 SparkStartPosition { get; set; }

		public bool PlayerSound { get; set; }
		public SoundId HitSoundId { get; set; }
		public bool GuardPlayerSound { get; set; }
		public SoundId GuardSoundId { get; set; }

		public AttackEffect GroundAttackEffect { get; set; }
		public AttackEffect AirAttackEffect { get; set; }
		public int GroundHitTime { get; set; }
		public int DownHitTime { get; set; }
		public int AirHitTime { get; set; }
		public int GuardHitTime { get; set; }
		public int GroundSlideTime { get; set; }
		public int GuardSlideTime { get; set; }
		public int GuardControlTime { get; set; }
		public int AirGuardControlTime { get; set; }
		public int GuardDistance { get; set; }
		public float YAcceleration { get; set; }
		public Vector2 GroundVelocity { get; set; }
		public Vector2 AirVelocity { get; set; }
		public Vector2 DownVelocity { get; set; }
		public Vector2 GroundGuardVelocity { get; set; }
		public Vector2 AirGuardVelocity { get; set; }
		public float GroundCornerPush { get; set; }
		public float AirCornerPush { get; set; }
		public float DownCornerPush { get; set; }
		public float GuardCornerPush { get; set; }
		public float AirGuardCornerPush { get; set; }
		public int JugglePointsNeeded { get; set; }
		public Point? MininumDistance { get; set; }
		public Point? MaximumDistance { get; set; }
		public Point? SnapLocation { get; set; }

		public int P1SpritePriority { get; set; }
		public int P2SpritePriority { get; set; }

		public int P1Facing { get; set; }
		public int P1GetP2Facing { get; set; }
		public int P2Facing { get; set; }

		public int? P1NewState { get; set; }
		public bool P2UseP1State { get; set; }
		public int? P2NewState { get; set; }

		public bool ForceStand { get; set; }
		public bool Fall { get; set; }
		public bool AirFall { get; set; }
		public float? FallVelocityX { get; set; }
		public float FallVelocityY { get; set; }
		public bool FallCanRecover { get; set; }
		public int FallRecoverTime { get; set; }
		public int FallDamage { get; set; }
		public bool DownBounce { get; set; }

		public int TargetId { get; set; }
		public int ChainId { get; set; }
		public int NoChainId1 { get; set; }
		public int NoChainId2 { get; set; }

		public bool HitOnce { get; set; }
		public bool CanKill { get; set; }
		public bool CanGuardKill { get; set; }
		public bool CanFallKill { get; set; }

		public int NumberOfHits { get; set; }

		public int P1HitPowerAdjustment { get; set; }
		public int P1GuardPowerAdjustment { get; set; }
		public int P2HitPowerAdjustment { get; set; }
		public int P2GuardPowerAdjustment { get; set; }

		public int PalFxTime { get; set; }
		public Vector3 PalFxAdd { get; set; }
		public Vector3 PalFxMul { get; set; }
		public Vector4 PalFxSinAdd { get; set; }
		public bool PalFxInvert { get; set; }
		public float PalFxBaseColor { get; set; }

		public int EnvShakeTime { get; set; }
		public float EnvShakeFrequency { get; set; }
		public int EnvShakeAmplitude { get; set; }
		public float EnvShakePhase { get; set; }

		public int EnvShakeFallTime { get; set; }
		public float EnvShakeFallFrequency { get; set; }
		public int EnvShakeFallAmplitude { get; set; }
		public float EnvShakeFallPhase { get; set; }
	}
}