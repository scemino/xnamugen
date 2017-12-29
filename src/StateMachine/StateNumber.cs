namespace xnaMugen.StateMachine
{
	internal static class StateNumber
	{
		public static readonly int Standing = 0;
		public static readonly int StandToCrouch = 10;
		public static readonly int Crouching = 11;
		public static readonly int CrouchToStand = 12;
		public static readonly int Walking = 20;
		public static readonly int JumpStart = 40;
		public static readonly int AirJumpStart = 45;
		public static readonly int JumpUp = 50;
		public static readonly int JumpDown = 51;
		public static readonly int JumpLand = 52;
		public static readonly int RunForward = 100;
		public static readonly int RunBack = 105;
		public static readonly int RunBack2Land = 106;
		public static readonly int RunUp = 110;
		public static readonly int RunDown = 111;
		public static readonly int GuardStart = 120;
		public static readonly int StandingGuard = 130;
		public static readonly int CrouchingGuard = 131;
		public static readonly int AirGuard = 132;
		public static readonly int GuardEnd = 140;
		public static readonly int StandingGuardHitShaking = 150;
		public static readonly int StandingGuardHitKnockedBack = 151;
		public static readonly int CrouchingGuardHitShaking = 152;
		public static readonly int CrouchingGuardHitKnockedBack = 153;
		public static readonly int AirGuardHitShaking = 154;
		public static readonly int AirGuardHitKnockedBack = 155;
		public static readonly int LoseTimeOverPose = 170;
		public static readonly int WinPose = 180;
		public static readonly int PreIntro = 190;
		public static readonly int Intro = 191;
		public static readonly int StandingHitShaking = 5000;
		public static readonly int StandingHitSlide = 5001;
		public static readonly int CrouchingHitShaking = 5010;
		public static readonly int CrouchingHitSlide = 5011;
		public static readonly int AirHitShaking = 5020;
		public static readonly int AirHitGoingUp = 5030;
		public static readonly int AirHitTransition = 5035;
		public static readonly int AirHitRecovery = 5040;
		public static readonly int AirHitFalling = 5050;
		public static readonly int HitTrip = 5070;
		public static readonly int HitTrip2 = 5071;
		public static readonly int HitProneShaking = 5080;
		public static readonly int HitProneSlide = 5081;
		public static readonly int HitBounce = 5100;
		public static readonly int HitBounce2 = 5101;
		public static readonly int HitLieDown = 5110;
		public static readonly int HitGetUp = 5120;
		public static readonly int HitLieDead = 5150;
		public static readonly int HitFallRecover = 5200;
		public static readonly int HitFallRecover2 = 5201;
		public static readonly int HitAirFallRecover = 5210;
		public static readonly int Continue = 5500;
		public static readonly int Initialize = 5900;
	}
}