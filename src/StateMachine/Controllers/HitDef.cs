using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitDef")]
	class HitDef : StateController
	{
		public HitDef(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_hitattr = textsection.GetAttribute<Combat.HitAttribute>("attr", null);
			m_hitflag = textsection.GetAttribute<Combat.HitFlag>("hitflag", Combat.HitFlag.Default);
			m_guardflag = textsection.GetAttribute<Combat.HitFlag>("guardflag", Combat.HitFlag.Default);
			m_affectteam = textsection.GetAttribute<AffectTeam>("affectteam", AffectTeam.Enemy);
			m_hitanimtype = textsection.GetAttribute<HitAnimationType>("animtype", HitAnimationType.Light);
			m_airhitanimtype = textsection.GetAttribute<HitAnimationType>("air.animtype", HitAnimationType);
			m_fallhitanimtype = textsection.GetAttribute<HitAnimationType>("fall.animtype", (AirHitAnimationType == HitAnimationType.Up) ? HitAnimationType.Up : HitAnimationType.Back);
			m_priority = textsection.GetAttribute<Combat.HitPriority>("priority", Combat.HitPriority.Default);
			m_damage = textsection.GetAttribute<Evaluation.Expression>("damage", null);
			m_pausetime = textsection.GetAttribute<Evaluation.Expression>("pausetime", null);
			m_guardpausetime = textsection.GetAttribute<Evaluation.Expression>("guard.pausetime", null);
			m_sparknumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("sparkno", null);
			m_guardsparknumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("guard.sparkno", null);
			m_sparkposition = textsection.GetAttribute<Evaluation.Expression>("sparkxy", null);
			m_hitsound = textsection.GetAttribute<Evaluation.PrefixedExpression>("hitsound", null);
			m_guardhitsound = textsection.GetAttribute<Evaluation.PrefixedExpression>("guardsound", null);
			m_attackeffect = textsection.GetAttribute<AttackEffect>("ground.type", AttackEffect.High);
			m_aireffect = textsection.GetAttribute<AttackEffect>("air.type", GroundAttackEffect);
			m_groundslidetime = textsection.GetAttribute<Evaluation.Expression>("ground.slidetime", null);
			m_guardslidetime = textsection.GetAttribute<Evaluation.Expression>("guard.slidetime", null);
			m_groundhittime = textsection.GetAttribute<Evaluation.Expression>("ground.hittime", null);
			m_guardhittime = textsection.GetAttribute<Evaluation.Expression>("guard.hittime", null);
			m_airhittime = textsection.GetAttribute<Evaluation.Expression>("air.hittime", null);
			m_guardctrltime = textsection.GetAttribute<Evaluation.Expression>("guard.ctrltime", null);
			m_guarddistance = textsection.GetAttribute<Evaluation.Expression>("guard.dist", null);
			m_yaccel = textsection.GetAttribute<Evaluation.Expression>("yaccel", null);
			m_groundvelocity = textsection.GetAttribute<Evaluation.Expression>("ground.velocity", null);
			m_guardvelocity = textsection.GetAttribute<Evaluation.Expression>("guard.velocity", null);
			m_airvelocity = textsection.GetAttribute<Evaluation.Expression>("air.velocity", null);
			m_airguardvelocity = textsection.GetAttribute<Evaluation.Expression>("airguard.velocity", null);
			m_groundcornerpushoff = textsection.GetAttribute<Evaluation.Expression>("ground.cornerpush.veloff", null);
			m_aircornerpushoff = textsection.GetAttribute<Evaluation.Expression>("air.cornerpush.veloff", null);
			m_downcornerpushoff = textsection.GetAttribute<Evaluation.Expression>("down.cornerpush.veloff", null);
			m_guardcornerpushoff = textsection.GetAttribute<Evaluation.Expression>("guard.cornerpush.veloff", null);
			m_airguardcornerpushoff = textsection.GetAttribute<Evaluation.Expression>("airguard.cornerpush.veloff", null);
			m_airguardctrltime = textsection.GetAttribute<Evaluation.Expression>("airguard.ctrltime", null);
			m_airjuggle = textsection.GetAttribute<Evaluation.Expression>("air.juggle", null);
			m_mindistance = textsection.GetAttribute<Evaluation.Expression>("mindist", null);
			m_maxdistance = textsection.GetAttribute<Evaluation.Expression>("maxdist", null);
			m_snap = textsection.GetAttribute<Evaluation.Expression>("snap", null);
			m_p1spritepriority = textsection.GetAttribute<Evaluation.Expression>("p1sprpriority", null);
			m_p2spritepriority = textsection.GetAttribute<Evaluation.Expression>("p2sprpriority", null);
			m_p1facing = textsection.GetAttribute<Evaluation.Expression>("p1facing", null);
			m_p1getp2facing = textsection.GetAttribute<Evaluation.Expression>("p1getp2facing", null);
			m_p2facing = textsection.GetAttribute<Evaluation.Expression>("p2facing", null);
			m_p1statenumber = textsection.GetAttribute<Evaluation.Expression>("p1stateno", null);
			m_p2statenumber = textsection.GetAttribute<Evaluation.Expression>("p2stateno", null);
			m_p2getp1state = textsection.GetAttribute<Evaluation.Expression>("p2getp1state", null);
			m_forcestand = textsection.GetAttribute<Evaluation.Expression>("forcestand", null);
			m_fall = textsection.GetAttribute<Evaluation.Expression>("fall", null);
			m_fallxvelocity = textsection.GetAttribute<Evaluation.Expression>("fall.xvelocity", null);
			m_fallyvelocity = textsection.GetAttribute<Evaluation.Expression>("fall.yvelocity", null);
			m_fallrecover = textsection.GetAttribute<Evaluation.Expression>("fall.recover", null);
			m_fallrecovertime = textsection.GetAttribute<Evaluation.Expression>("fall.recovertime", null);
			m_falldamage = textsection.GetAttribute<Evaluation.Expression>("fall.damage", null);
			m_airfall = textsection.GetAttribute<Evaluation.Expression>("air.fall", null);
			m_downvelocity = textsection.GetAttribute<Evaluation.Expression>("down.velocity", null);
			m_downhittime = textsection.GetAttribute<Evaluation.Expression>("down.hittime", null);
			m_downbounce = textsection.GetAttribute<Evaluation.Expression>("down.bounce", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_chainid = textsection.GetAttribute<Evaluation.Expression>("chainid", null);
			m_nochainid = textsection.GetAttribute<Evaluation.Expression>("nochainid", null);
			m_hitonce = textsection.GetAttribute<Evaluation.Expression>("hitonce", null);
			m_kill = textsection.GetAttribute<Evaluation.Expression>("kill", null);
			m_guardkill = textsection.GetAttribute<Evaluation.Expression>("guard.kill", null);
			m_fallkill = textsection.GetAttribute<Evaluation.Expression>("fall.kill", null);
			m_numberofhits = textsection.GetAttribute<Evaluation.Expression>("numhits", null);
			m_p1powerincrease = textsection.GetAttribute<Evaluation.Expression>("getpower", null);
			m_p2powerincrease = textsection.GetAttribute<Evaluation.Expression>("givepower", null);
			m_paltime = textsection.GetAttribute<Evaluation.Expression>("palfx.time", null);
			m_palmul = textsection.GetAttribute<Evaluation.Expression>("palfx.mul", null);
			m_paladd = textsection.GetAttribute<Evaluation.Expression>("palfx.add", null);
			m_palsinadd = textsection.GetAttribute<Evaluation.Expression>("palfx.sinadd", null);
			m_palinvert = textsection.GetAttribute<Evaluation.Expression>("palfx.invertall", null);
			m_palcolor = textsection.GetAttribute<Evaluation.Expression>("palfx.color", null);
			m_shaketime = textsection.GetAttribute<Evaluation.Expression>("envshake.time", null);
			m_shakefreq = textsection.GetAttribute<Evaluation.Expression>("envshake.freq", null);
			m_shakeamplitude = textsection.GetAttribute<Evaluation.Expression>("envshake.ampl", null);
			m_shakephaseoffset = textsection.GetAttribute<Evaluation.Expression>("envshake.phase", null);
			m_fallshaketime = textsection.GetAttribute<Evaluation.Expression>("fall.envshake.time", null);
			m_fallshakefreq = textsection.GetAttribute<Evaluation.Expression>("fall.envshake.freq", null);
			m_fallshakeamplitude = textsection.GetAttribute<Evaluation.Expression>("fall.envshake.ampl", null);
			m_fallshakephaseoffset = textsection.GetAttribute<Evaluation.Expression>("fall.envshake.phase", null);
			m_attackwidth = textsection.GetAttribute<Evaluation.Expression>("attack.width", null);

#warning Note this.
#if EMULATE_ENGINE_BUG
			if (m_hitanimtype == HitAnimationType.Back) m_hitanimtype = HitAnimationType.Hard;

			m_palcolor = null;
#endif
		}

		public override void Run(Combat.Character character)
		{
			SetHitDefinition(character, character.OffensiveInfo.HitDef);

			character.OffensiveInfo.ActiveHitDef = true;
			character.OffensiveInfo.HitPauseTime = 0;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (HitAttribute == null) return false;

			return true;
		}

		protected void SetHitDefinition(Combat.Character character, Combat.HitDefinition hitdef)
		{
			if (character == null) throw new ArgumentNullException("character");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			Int32 defaulthitspark = EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultSparkNumber, -1);
			Boolean defaultplayerhitspark = !EvaluationHelper.IsCommon(character.BasePlayer.Constants.DefaultSparkNumber, true);

			Int32 defaultguardspark = EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultGuardSparkNumber, -1);
			Boolean defaultplayerguardspark = !EvaluationHelper.IsCommon(character.BasePlayer.Constants.DefaultGuardSparkNumber, true); 

			hitdef.HitAttribute = HitAttribute;
			hitdef.HitFlag = HitFlag;
			hitdef.GuardFlag = GuardFlag;
			hitdef.Targeting = Targetting;
			hitdef.GroundAnimationType = HitAnimationType;
			hitdef.AirAnimationType = AirHitAnimationType;
			hitdef.FallAnimationType = FallHitAnimationType;
			hitdef.HitPriority = Priority;

			Point damage = EvaluationHelper.AsPoint(character, Damage, new Point(0, 0));
			hitdef.HitDamage = damage.X;
			hitdef.GuardDamage = damage.Y;

			Point pauseshaketime = EvaluationHelper.AsPoint(character, PauseTime, new Point(0, 0));
			hitdef.PauseTime = pauseshaketime.X;
			hitdef.ShakeTime = pauseshaketime.Y;

			Point guardpauseshaketime = EvaluationHelper.AsPoint(character, GuardPauseTime, pauseshaketime);
			hitdef.GuardPauseTime = guardpauseshaketime.X;
			hitdef.GuardShakeTime = guardpauseshaketime.Y;

			hitdef.PlayerSpark = !EvaluationHelper.IsCommon(SparkNumber, !defaultplayerhitspark);
			hitdef.SparkAnimation = EvaluationHelper.AsInt32(character, SparkNumber, defaulthitspark);

			hitdef.GuardPlayerSpark = !EvaluationHelper.IsCommon(GuardSparkNumber, !defaultplayerguardspark);
			hitdef.GuardSparkAnimation = EvaluationHelper.AsInt32(character, GuardSparkNumber, defaultguardspark);

			hitdef.SparkStartPosition = (Vector2)EvaluationHelper.AsPoint(character, SparkPosition, new Point(0, 0));

			hitdef.PlayerSound = !EvaluationHelper.IsCommon(HitSound, true);
			hitdef.HitSoundId = EvaluationHelper.AsSoundId(character, HitSound, SoundId.Invalid);

			hitdef.GuardPlayerSound = !EvaluationHelper.IsCommon(GuardHitSound, true);
			hitdef.GuardSoundId = EvaluationHelper.AsSoundId(character, GuardHitSound, SoundId.Invalid);

			hitdef.GroundAttackEffect = GroundAttackEffect;
			hitdef.AirAttackEffect = AirAttackEffect;

			hitdef.GroundHitTime = EvaluationHelper.AsInt32(character, GroundHitTime, 0);
			hitdef.DownHitTime = EvaluationHelper.AsInt32(character, DownHitTime, 0);
			hitdef.GuardHitTime = EvaluationHelper.AsInt32(character, GuardHitTime, hitdef.GroundHitTime);
			hitdef.AirHitTime = EvaluationHelper.AsInt32(character, AirHitTime, 20);
			hitdef.GroundSlideTime = EvaluationHelper.AsInt32(character, GroundSlideTime, 0);
			hitdef.GuardSlideTime = EvaluationHelper.AsInt32(character, GroundSlideTime, hitdef.GuardHitTime);
			hitdef.GuardControlTime = EvaluationHelper.AsInt32(character, GuardControlTime, hitdef.GuardSlideTime);
			hitdef.AirGuardControlTime = EvaluationHelper.AsInt32(character, AirGuardControlTime, hitdef.GuardControlTime);

			hitdef.GuardDistance = EvaluationHelper.AsInt32(character, GuardDistance, character.BasePlayer.Constants.Attackdistance);
			hitdef.YAcceleration = EvaluationHelper.AsSingle(character, VerticalAcceleration, 0.35f);
			hitdef.GroundVelocity = EvaluationHelper.AsVector2(character, GroundVelocity, Vector2.Zero);
			hitdef.GroundGuardVelocity = new Vector2(EvaluationHelper.AsSingle(character, GuardVelocity, hitdef.GroundVelocity.X), 0);
			hitdef.AirVelocity = EvaluationHelper.AsVector2(character, AirVelocity, Vector2.Zero);
			hitdef.DownVelocity = EvaluationHelper.AsVector2(character, DownVelocity, hitdef.AirVelocity);
			hitdef.AirGuardVelocity = EvaluationHelper.AsVector2(character, AirGuardVelocity, hitdef.AirVelocity * new Vector2(1.5f, 0.5f));
			hitdef.GroundCornerPush = EvaluationHelper.AsSingle(character, GroundCornerPushOff, (hitdef.HitAttribute.HasHeight(AttackStateType.Air) == true) ? 0.0f : hitdef.GroundGuardVelocity.X * 1.3f);
			hitdef.AirCornerPush = EvaluationHelper.AsSingle(character, AirCornerPushOff, hitdef.GroundCornerPush);
			hitdef.DownCornerPush = EvaluationHelper.AsSingle(character, DownCornerPushOff, hitdef.GroundCornerPush);
			hitdef.GuardCornerPush = EvaluationHelper.AsSingle(character, GuardCornerPushOff, hitdef.GroundCornerPush);
			hitdef.AirGuardCornerPush = EvaluationHelper.AsSingle(character, AirGuardCornerPushOff, hitdef.GroundCornerPush);
			hitdef.JugglePointsNeeded = EvaluationHelper.AsInt32(character, JugglePointsNeeded, 0);
			hitdef.MininumDistance = EvaluationHelper.AsPoint(character, MinimumDistance, (Point?)null);
			hitdef.MaximumDistance = EvaluationHelper.AsPoint(character, MaximumDistance, (Point?)null);
			hitdef.SnapLocation = EvaluationHelper.AsPoint(character, Snap, (Point?)null);
			hitdef.P1SpritePriority = EvaluationHelper.AsInt32(character, P1SpritePriority, 1);
			hitdef.P2SpritePriority = EvaluationHelper.AsInt32(character, P2SpritePriority, 0);
			hitdef.P1Facing = EvaluationHelper.AsInt32(character, P1Facing, 0);
			hitdef.P1GetP2Facing = EvaluationHelper.AsInt32(character, P1GetP2Facing, 0);
			hitdef.P2Facing = EvaluationHelper.AsInt32(character, P2Facing, 0);
			hitdef.P1NewState = EvaluationHelper.AsInt32(character, P1StateNumber, (Int32?)null);
			hitdef.P2NewState = EvaluationHelper.AsInt32(character, P2StateNumber, (Int32?)null);
			hitdef.P2UseP1State = EvaluationHelper.AsBoolean(character, P2GetP1StateNumber, true);
			hitdef.ForceStand = EvaluationHelper.AsBoolean(character, ForceStand, (hitdef.GroundVelocity.Y != 0) ? true : false);
			hitdef.Fall = EvaluationHelper.AsBoolean(character, Fall, false);
			hitdef.AirFall = EvaluationHelper.AsBoolean(character, AirFall, hitdef.Fall);

			hitdef.FallVelocityX = EvaluationHelper.AsSingle(character, FallXVelocity, null);
			hitdef.FallVelocityY = EvaluationHelper.AsSingle(character, FallYVelocity, -4.5f);

			hitdef.FallCanRecover = EvaluationHelper.AsBoolean(character, FallCanRecover, true);
			hitdef.FallRecoverTime = EvaluationHelper.AsInt32(character, FallRecoverTime, 4);
			hitdef.FallDamage = EvaluationHelper.AsInt32(character, FallDamage, 0);
			hitdef.DownBounce = EvaluationHelper.AsBoolean(character, DownBounce, false);
			hitdef.TargetId = EvaluationHelper.AsInt32(character, TargetId, 0);
			hitdef.ChainId = EvaluationHelper.AsInt32(character, ChainId, -1);

			Point nochainid = EvaluationHelper.AsPoint(character, NoChainId, new Point(-1, -1));
			hitdef.NoChainId1 = nochainid.X;
			hitdef.NoChainId2 = nochainid.Y;

			hitdef.HitOnce = EvaluationHelper.AsBoolean(character, HitOnce, (hitdef.HitAttribute.HasData(new Combat.HitType(AttackClass.Throw, AttackPower.All)) == true) ? true : false);
			hitdef.CanKill = EvaluationHelper.AsBoolean(character, CanKill, true);
			hitdef.CanGuardKill = EvaluationHelper.AsBoolean(character, CanGuardKill, true);
			hitdef.CanFallKill = EvaluationHelper.AsBoolean(character, CanFallKill, true);
			hitdef.NumberOfHits = EvaluationHelper.AsInt32(character, NumberOfHits, 1);

			if (P1PowerIncrease != null)
			{
				Evaluation.Result statepower = P1PowerIncrease.Evaluate(character);
				hitdef.P1HitPowerAdjustment = (statepower.IsValid(0) == true) ? statepower[0].IntValue : (Int32)(hitdef.HitDamage * 0.7f);
				hitdef.P1GuardPowerAdjustment = (statepower.IsValid(1) == true) ? statepower[1].IntValue : (Int32)(hitdef.P1HitPowerAdjustment * 0.5f);
			}

			if (P2PowerIncrease != null)
			{
				Evaluation.Result p2power = P2PowerIncrease.Evaluate(character);
				hitdef.P2HitPowerAdjustment = (p2power.IsValid(0) == true) ? p2power[0].IntValue : (Int32)(hitdef.HitDamage * 0.6f);
				hitdef.P2GuardPowerAdjustment = (p2power.IsValid(1) == true) ? p2power[1].IntValue : (Int32)(hitdef.P2HitPowerAdjustment * 0.5f);
			}

			hitdef.PalFxTime = EvaluationHelper.AsInt32(character, PaletteColorTime, 0);
			hitdef.PalFxAdd = EvaluationHelper.AsVector3(character, PaletteColorAdd, new Vector3(0, 0, 0));
			hitdef.PalFxMul = EvaluationHelper.AsVector3(character, PaletteColorMultiply, new Vector3(255, 255, 255));
			hitdef.PalFxBaseColor = EvaluationHelper.AsInt32(character, PaletteColor, 255) / 255.0f;
			hitdef.PalFxInvert = EvaluationHelper.AsBoolean(character, PaletteColorInversion, false);
			hitdef.PalFxSinAdd = EvaluationHelper.AsVector4(character, PaletteColorSineAdd, new Vector4(0, 0, 0, 1));

			hitdef.EnvShakeTime = EvaluationHelper.AsInt32(character, ShakeTime, 0);
			hitdef.EnvShakeFrequency = Misc.Clamp(EvaluationHelper.AsSingle(character, ShakeFrequency, 60), 0, 180);
			hitdef.EnvShakeAmplitude = EvaluationHelper.AsInt32(character, ShakeAmplitude, -4);
			hitdef.EnvShakePhase = EvaluationHelper.AsSingle(character, ShakePhaseOffset, hitdef.EnvShakeFrequency >= 90 ? 0 : 90);

			hitdef.EnvShakeFallTime = EvaluationHelper.AsInt32(character, FallShakeTime, 0);
			hitdef.EnvShakeFallFrequency = Misc.Clamp(EvaluationHelper.AsSingle(character, FallShakeFrequency, 60), 0, 180);
			hitdef.EnvShakeFallAmplitude = EvaluationHelper.AsInt32(character, FallShakeAmplitude, -4);
			hitdef.EnvShakeFallPhase = EvaluationHelper.AsSingle(character, FallShakePhaseOffset, hitdef.EnvShakeFallFrequency >= 90 ? 0 : 90);
		}

		public Combat.HitAttribute HitAttribute
		{
			get { return m_hitattr; }
		}

		public Combat.HitFlag HitFlag
		{
			get { return m_hitflag; }
		}

		public Combat.HitFlag GuardFlag
		{
			get { return m_guardflag; }
		}

		public AffectTeam Targetting
		{
			get { return m_affectteam; }
		}

		public HitAnimationType HitAnimationType
		{
			get { return m_hitanimtype; }
		}

		public HitAnimationType AirHitAnimationType
		{
			get { return m_airhitanimtype; }
		}

		public HitAnimationType FallHitAnimationType
		{
			get { return m_fallhitanimtype; }
		}

		public Combat.HitPriority Priority
		{
			get { return m_priority; }
		}

		public Evaluation.Expression Damage
		{
			get { return m_damage; }
		}

		public Evaluation.Expression PauseTime
		{
			get { return m_pausetime; }
		}

		public Evaluation.Expression GuardPauseTime
		{
			get { return m_guardpausetime; }
		}

		public Evaluation.PrefixedExpression SparkNumber
		{
			get { return m_sparknumber; }
		}

		public Evaluation.PrefixedExpression GuardSparkNumber
		{
			get { return m_guardsparknumber; }
		}

		public Evaluation.Expression SparkPosition
		{
			get { return m_sparkposition; }
		}

		public Evaluation.PrefixedExpression HitSound
		{
			get { return m_hitsound; }
		}

		public Evaluation.PrefixedExpression GuardHitSound
		{
			get { return m_guardhitsound; }
		}

		public AttackEffect GroundAttackEffect
		{
			get { return m_attackeffect; }
		}

		public AttackEffect AirAttackEffect
		{
			get { return m_aireffect; }
		}

		public Evaluation.Expression GroundSlideTime
		{
			get { return m_groundslidetime; }
		}

		public Evaluation.Expression GuardSlideTime
		{
			get { return m_guardslidetime; }
		}

		public Evaluation.Expression GroundHitTime
		{
			get { return m_groundhittime; }
		}

		public Evaluation.Expression GuardHitTime
		{
			get { return m_guardhittime; }
		}

		public Evaluation.Expression AirHitTime
		{
			get { return m_airhittime; }
		}

		public Evaluation.Expression GuardControlTime
		{
			get { return m_guardctrltime; }
		}

		public Evaluation.Expression GuardDistance
		{
			get { return m_guarddistance; }
		}

		public Evaluation.Expression VerticalAcceleration
		{
			get { return m_yaccel; }
		}

		public Evaluation.Expression GroundVelocity
		{
			get { return m_groundvelocity; }
		}

		public Evaluation.Expression GuardVelocity
		{
			get { return m_guardvelocity; }
		}

		public Evaluation.Expression AirVelocity
		{
			get { return m_airvelocity; }
		}

		public Evaluation.Expression AirGuardVelocity
		{
			get { return m_airguardvelocity; }
		}

		public Evaluation.Expression GroundCornerPushOff
		{
			get { return m_groundcornerpushoff; }
		}

		public Evaluation.Expression AirCornerPushOff
		{
			get { return m_aircornerpushoff; }
		}

		public Evaluation.Expression DownCornerPushOff
		{
			get { return m_downcornerpushoff; }
		}

		public Evaluation.Expression GuardCornerPushOff
		{
			get { return m_guardcornerpushoff; }
		}

		public Evaluation.Expression AirGuardCornerPushOff
		{
			get { return m_airguardcornerpushoff; }
		}

		public Evaluation.Expression AirGuardControlTime
		{
			get { return m_airguardctrltime; }
		}

		public Evaluation.Expression JugglePointsNeeded
		{
			get { return m_airjuggle; }
		}

		public Evaluation.Expression MinimumDistance
		{
			get { return m_mindistance; }
		}

		public Evaluation.Expression MaximumDistance
		{
			get { return m_maxdistance; }
		}

		public Evaluation.Expression Snap
		{
			get { return m_snap; }
		}

		public Evaluation.Expression P1SpritePriority
		{
			get { return m_p1spritepriority; }
		}

		public Evaluation.Expression P2SpritePriority
		{
			get { return m_p2spritepriority; }
		}

		public Evaluation.Expression P1Facing
		{
			get { return m_p1facing; }
		}

		public Evaluation.Expression P1GetP2Facing
		{
			get { return m_p1getp2facing; }
		}

		public Evaluation.Expression P2Facing
		{
			get { return m_p2facing; }
		}

		public Evaluation.Expression P1StateNumber
		{
			get { return m_p1statenumber; }
		}

		public Evaluation.Expression P2StateNumber
		{
			get { return m_p2statenumber; }
		}

		public Evaluation.Expression P2GetP1StateNumber
		{
			get { return m_p2getp1state; }
		}

		public Evaluation.Expression ForceStand
		{
			get { return m_forcestand; }
		}

		public Evaluation.Expression Fall
		{
			get { return m_fall; }
		}

		public Evaluation.Expression FallXVelocity
		{
			get { return m_fallxvelocity; }
		}

		public Evaluation.Expression FallYVelocity
		{
			get { return m_fallyvelocity; }
		}

		public Evaluation.Expression FallCanRecover
		{
			get { return m_fallrecover; }
		}

		public Evaluation.Expression FallRecoverTime
		{
			get { return m_fallrecovertime; }
		}

		public Evaluation.Expression FallDamage
		{
			get { return m_falldamage; }
		}

		public Evaluation.Expression AirFall
		{
			get { return m_airfall; }
		}

		public Evaluation.Expression DownVelocity
		{
			get { return m_downvelocity; }
		}

		public Evaluation.Expression DownHitTime
		{
			get { return m_downhittime; }
		}

		public Evaluation.Expression DownBounce
		{
			get { return m_downbounce; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		public Evaluation.Expression ChainId
		{
			get { return m_chainid; }
		}

		public Evaluation.Expression NoChainId
		{
			get { return m_nochainid; }
		}

		public Evaluation.Expression HitOnce
		{
			get { return m_hitonce; }
		}

		public Evaluation.Expression CanKill
		{
			get { return m_kill; }
		}

		public Evaluation.Expression CanGuardKill
		{
			get { return m_guardkill; }
		}

		public Evaluation.Expression CanFallKill
		{
			get { return m_fallkill; }
		}

		public Evaluation.Expression NumberOfHits
		{
			get { return m_numberofhits; }
		}

		public Evaluation.Expression P1PowerIncrease
		{
			get { return m_p1powerincrease; }
		}

		public Evaluation.Expression P2PowerIncrease
		{
			get { return m_p2powerincrease; }
		}

		public Evaluation.Expression PaletteColorTime
		{
			get { return m_paltime; }
		}

		public Evaluation.Expression PaletteColorMultiply
		{
			get { return m_palmul; }
		}

		public Evaluation.Expression PaletteColorAdd
		{
			get { return m_paladd; }
		}

		public Evaluation.Expression PaletteColorSineAdd
		{
			get { return m_palsinadd; }
		}

		public Evaluation.Expression PaletteColorInversion
		{
			get { return m_palinvert; }
		}

		public Evaluation.Expression PaletteColor
		{
			get { return m_palcolor; }
		}

		public Evaluation.Expression ShakeTime
		{
			get { return m_shaketime; }
		}

		public Evaluation.Expression ShakeFrequency
		{
			get { return m_shakefreq; }
		}

		public Evaluation.Expression ShakeAmplitude
		{
			get { return m_shakeamplitude; }
		}

		public Evaluation.Expression ShakePhaseOffset
		{
			get { return m_shakephaseoffset; }
		}

		public Evaluation.Expression FallShakeTime
		{
			get { return m_fallshaketime; }
		}

		public Evaluation.Expression FallShakeFrequency
		{
			get { return m_fallshakefreq; }
		}

		public Evaluation.Expression FallShakeAmplitude
		{
			get { return m_fallshakeamplitude; }
		}

		public Evaluation.Expression FallShakePhaseOffset
		{
			get { return m_fallshakephaseoffset; }
		}

		public Evaluation.Expression AttackWidth
		{
			get { return m_attackwidth; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitAttribute m_hitattr;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitFlag m_hitflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitFlag m_guardflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AffectTeam m_affectteam;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitAnimationType m_hitanimtype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitAnimationType m_airhitanimtype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitAnimationType m_fallhitanimtype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitPriority m_priority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_damage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardpausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_sparknumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_guardsparknumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_sparkposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_hitsound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_guardhitsound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AttackEffect m_attackeffect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AttackEffect m_aireffect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_groundslidetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardslidetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_groundhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardctrltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guarddistance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_yaccel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_groundvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airguardvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_groundcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_aircornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_downcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airguardcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airguardctrltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airjuggle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_mindistance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_maxdistance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_snap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p1spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p2spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p1facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p1getp2facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p2facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p1statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p2statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p2getp1state;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_forcestand;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fall;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallxvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallyvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallrecover;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallrecovertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_falldamage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airfall;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_downvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_downhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_downbounce;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_chainid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_nochainid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_hitonce;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_kill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_guardkill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallkill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_numberofhits;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p1powerincrease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p2powerincrease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_paltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_paladd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palsinadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shaketime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shakefreq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shakeamplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shakephaseoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallshaketime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallshakefreq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallshakeamplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallshakephaseoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_attackwidth;

		#endregion
	}
}