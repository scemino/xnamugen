using System;

namespace xnaMugen
{
	enum Axis { None, X, Y }

	enum ScreenShotFormat { None, Jpg, Bmp, Png }

	enum ScreenType { None, Title, Select, Versus, Combat }

	enum FadeDirection { None, In, Out }

	enum Assertion { None, Intro, Invisible, RoundNotOver, NoBarDisplay, NoBackground, NoForeground, NoStandGuard, NoAirGuard, NoCrouchGuard, NoAutoturn, NoJuggleCheck, NoKOSound, NoKOSlow, NoShadow, GlobalNoShadow, NoMusic, NoWalk, TimerFreeze, Unguardable, NoKO }

	enum BindToTargetPostion { None, Foot, Mid, Head }

	enum Victory { None, Normal, Special, Hyper, NormalThrow, Cheese, Time, Suicude, TeamKill }

	enum TeamSide { None, Left, Right }

	enum GameSpeed { Normal, Slow }

	enum DrawMode { None, Normal, Font, OutlinedRectangle, Lines, Shadow }

	enum CollisionType { None, PlayerPush, CharacterHit, ProjectileHit, ProjectileCollision }

	[Flags]
	enum AttackStateType { None = 0, Standing = 1, Crouching = 2, Air = 4 }

	enum AttackPower { None = 0, Normal, Special, Hyper, All }

	enum AttackClass { None = 0, Normal, Throw, Projectile, All }

	enum HitFlagCombo { No = 0, Yes, DontCare }

	[Flags]
	enum AffectTeam { None = 0, Enemy = 1, Friendly = 2, Both = Enemy | Friendly }

	enum HitAnimationType { None = 0, Light, Medium, Hard, Back, Up, DiagUp }

	enum PriorityType { None, Hit, Dodge, Miss }

	enum AttackEffect { None = 0, High, Low, Trip }

	enum HelperType { Normal = 0, Player, Projectile }

	enum PositionType { None = 0, P1, P2, Front, Back, Left, Right }

	enum ClsnType { None, Type1Attack, Type2Normal }

	enum Facing { Left, Right }

	enum BlendType { None, Add, Subtract }

	enum BackgroundLayer { Front, Back }

	enum NumberType { None, Int, Float }

	enum PrintJustification { Left, Right, Center }

	enum PlayerButton { None = 0, A, B, C, X, Y, Z, Up, Down, Left, Right, Taunt, Pause }

	enum SystemButton { None, Pause, PauseStep, Quit, DebugDraw, FullLifeAndPower, TestCheat, TakeScreenshot }

	enum RoundState { None, PreIntro, Intro, Fight, PreOver, Over }

	enum IntroState { None, Running, RoundNumber, Fight }

	enum CommandDirection { None = 0, B, DB, D, DF, F, UF, U, UB, B4Way, U4Way, F4Way, D4Way }

	enum StateType { None, Unchanged, Standing, Crouching, Airborne, Prone }

	enum MoveType { None, Idle, Attack, BeingHit, Unchanged }

	enum Physics { None, Unchanged, Standing, Crouching, Airborne }

	enum PlayerControl { Unchanged, InControl, NoControl }

	[Flags]
	enum CommandButton { None = 0, A = 1, B = 2, C = 4, X = 8, Y = 16, Z = 32, Taunt = 64 }

	[Flags]
	enum ForceFeedbackType { None = 0, Sine = 1, Square = 2 }

	enum ButtonState { Up, Down, Pressed, Released }

	enum ProjectileDataType { None, Hit, Guarded, Cancel }

	enum PauseState { Unpaused, Paused, PauseStep }

	enum MainMenuOption { Arcade = 0, Versus = 1, TeamArcade = 2, TeamVersus = 3, TeamCoop = 4, Survival = 5, SurvivalCoop = 6, Training = 7, Watch = 8, Options = 9, Quit = 10 }

	enum EntityUpdateOrder { Character, Projectile, Explod };

	enum ProjectileState { Normal, Removing, Canceling, Kill }

	enum PlayerSelectType { Profile, Random }

	enum CursorDirection { Up, Down, Left, Right }

	enum ElementType { None, Static, Animation, Text }

	enum CombatMode { None, Versus, TeamArcade, TeamVersus, TeamCoop, Survival, SurvivalCoop, Training }
}