using System;

namespace xnaMugen
{
	internal enum Axis { None, X, Y }

	internal enum ScreenShotFormat { None, Jpg, Bmp, Png }

	internal enum ScreenType { None, Title, Select, Versus, Combat, Replay }

	internal enum FadeDirection { None, In, Out }

	internal enum Assertion { None, Intro, Invisible, RoundNotOver, NoBarDisplay, NoBackground, NoForeground, NoStandGuard, NoAirGuard, NoCrouchGuard, NoAutoturn, NoJuggleCheck, NoKOSound, NoKOSlow, NoShadow, GlobalNoShadow, NoMusic, NoWalk, TimerFreeze, Unguardable, NoKO }

	internal enum BindToTargetPostion { None, Foot, Mid, Head }

	internal enum Victory { None, Normal, Special, Hyper, NormalThrow, Cheese, Time, Suicude, TeamKill }

    internal enum TeamSide { None, Left, Right }
	
    internal enum TeamMode { None, Single, Simul, Turns }

	internal enum GameSpeed { Normal, Slow }

	internal enum DrawMode { None, Normal, Font, OutlinedRectangle, FilledRectangle, Lines }

	internal enum CollisionType { None, PlayerPush, CharacterHit, ProjectileHit, ProjectileCollision }

	[Flags]
	internal enum AttackStateType { None = 0, Standing = 1, Crouching = 2, Air = 4 }

	internal enum AttackPower { None = 0, Normal, Special, Hyper, All }

	internal enum AttackClass { None = 0, Normal, Throw, Projectile, All }

	internal enum HitFlagCombo { No = 0, Yes, DontCare }

	[Flags]
	internal enum AffectTeam { None = 0, Enemy = 1, Friendly = 2, Both = Enemy | Friendly }

	internal enum HitAnimationType { None = 0, Light, Medium, Hard, Back, Up, DiagUp }

	internal enum PriorityType { None, Hit, Dodge, Miss }

	internal enum AttackEffect { None = 0, High, Low, Trip }

	internal enum HelperType { Normal = 0, Player, Projectile }

	internal enum PositionType { None = 0, P1, P2, Front, Back, Left, Right }

	internal enum ClsnType { None, Type1Attack, Type2Normal }

	internal enum Facing { Left, Right }

	internal enum BlendType { None, Add, Subtract }

	internal enum BackgroundLayer { Front, Back }

	internal enum NumberType { None, Int, Float }

	internal enum PrintJustification { Left, Right, Center }

	[Flags]
	internal enum PlayerButton { None = 0, Up = 1, Down = 2, Left = 4, Right = 8, A = 16, B = 32, C = 64, X = 128, Y = 256, Z = 512, Taunt = 1024, Pause = 2048 }

	[Flags]
	internal enum SystemButton { None = 0, Pause = 1, PauseStep = 2, Quit = 4, DebugDraw = 8, FullLifeAndPower = 16, TestCheat = 32, TakeScreenshot = 64 }

	internal enum RoundState { None, PreIntro, Intro, Fight, PreOver, Over }

	internal enum IntroState { None, Running, RoundNumber, Fight }

	internal enum CommandDirection { None = 0, B, DB, D, DF, F, UF, U, UB, B4Way, U4Way, F4Way, D4Way }

	internal enum StateType { None, Unchanged, Standing, Crouching, Airborne, Prone }

	internal enum MoveType { None, Idle, Attack, BeingHit, Unchanged }

	internal enum Physics { None, Unchanged, Standing, Crouching, Airborne }

	internal enum PlayerControl { Unchanged, InControl, NoControl }

	[Flags]
	internal enum CommandButton { None = 0, A = 1, B = 2, C = 4, X = 8, Y = 16, Z = 32, Taunt = 64 }

	[Flags]
	internal enum ForceFeedbackType { None = 0, Sine = 1, Square = 2 }

	internal enum ButtonState { Up, Down, Pressed, Released }

	internal enum ProjectileDataType { None, Hit, Guarded, Cancel }

	internal enum PauseState { Unpaused, Paused, PauseStep }

	internal enum MainMenuOption { Arcade = 0, Versus = 1, TeamArcade = 2, TeamVersus = 3, TeamCoop = 4, Survival = 5, SurvivalCoop = 6, Training = 7, Watch = 8, Options = 9, Quit = 10 }

	internal enum EntityUpdateOrder { Character, Projectile, Explod };

	internal enum ProjectileState { Normal, Removing, Canceling, Kill }

	internal enum PlayerSelectType { Profile, Random }

	internal enum CursorDirection { Up, Down, Left, Right }

	internal enum ElementType { None, Static, Animation, Text }

	internal enum CombatMode { None, Versus, TeamArcade, TeamVersus, TeamCoop, Survival, SurvivalCoop, Training }
}