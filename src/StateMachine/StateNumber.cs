using System;

namespace xnaMugen.StateMachine
{
	static class StateNumber
	{
		public static readonly Int32 Standing = 0;
		public static readonly Int32 StandToCrouch = 10;
		public static readonly Int32 Crouching = 11;
		public static readonly Int32 CrouchToStand = 12;
		public static readonly Int32 Walking = 20;
		public static readonly Int32 JumpStart = 40;
		public static readonly Int32 AirJumpStart = 45;
		public static readonly Int32 JumpUp = 50;
		public static readonly Int32 JumpDown = 51;
		public static readonly Int32 JumpLand = 52;
		public static readonly Int32 RunForward = 100;
		public static readonly Int32 RunBack = 105;
		public static readonly Int32 RunBack2Land = 106;
		public static readonly Int32 RunUp = 110;
		public static readonly Int32 RunDown = 111;
		public static readonly Int32 GuardStart = 120;
		public static readonly Int32 StandingGuard = 130;
		public static readonly Int32 CrouchingGuard = 131;
		public static readonly Int32 AirGuard = 132;
		public static readonly Int32 GuardEnd = 140;
		public static readonly Int32 StandingGuardHitShaking = 150;
		public static readonly Int32 StandingGuardHitKnockedBack = 151;
		public static readonly Int32 CrouchingGuardHitShaking = 152;
		public static readonly Int32 CrouchingGuardHitKnockedBack = 153;
		public static readonly Int32 AirGuardHitShaking = 154;
		public static readonly Int32 AirGuardHitKnockedBack = 155;
		public static readonly Int32 LoseTimeOverPose = 170;
		public static readonly Int32 WinPose = 180;
		public static readonly Int32 PreIntro = 190;
		public static readonly Int32 Intro = 191;
		public static readonly Int32 StandingHitShaking = 5000;
		public static readonly Int32 StandingHitSlide = 5001;
		public static readonly Int32 CrouchingHitShaking = 5010;
		public static readonly Int32 CrouchingHitSlide = 5011;
		public static readonly Int32 AirHitShaking = 5020;
		public static readonly Int32 AirHitGoingUp = 5030;
		public static readonly Int32 AirHitTransition = 5035;
		public static readonly Int32 AirHitRecovery = 5040;
		public static readonly Int32 AirHitFalling = 5050;
		public static readonly Int32 HitTrip = 5070;
		public static readonly Int32 HitTrip2 = 5071;
		public static readonly Int32 HitProneShaking = 5080;
		public static readonly Int32 HitProneSlide = 5081;
		public static readonly Int32 HitBounce = 5100;
		public static readonly Int32 HitBounce2 = 5101;
		public static readonly Int32 HitLieDown = 5110;
		public static readonly Int32 HitGetUp = 5120;
		public static readonly Int32 HitLieDead = 5150;
		public static readonly Int32 HitFallRecover = 5200;
		public static readonly Int32 HitFallRecover2 = 5201;
		public static readonly Int32 HitAirFallRecover = 5210;
		public static readonly Int32 Continue = 5500;
		public static readonly Int32 Initialize = 5900;
	}
}