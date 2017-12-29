using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitDef")]
	internal class HitDef : StateController
	{
		public HitDef(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_hitattr = textsection.GetAttribute<Combat.HitAttribute>("attr", null);
			m_hitflag = textsection.GetAttribute("hitflag", Combat.HitFlag.Default);
			m_guardflag = textsection.GetAttribute("guardflag", Combat.HitFlag.Default);
			m_affectteam = textsection.GetAttribute("affectteam", AffectTeam.Enemy);
			m_hitanimtype = textsection.GetAttribute("animtype", HitAnimationType.Light);
			m_airhitanimtype = textsection.GetAttribute("air.animtype", HitAnimationType);
			m_fallhitanimtype = textsection.GetAttribute("fall.animtype", AirHitAnimationType == HitAnimationType.Up ? HitAnimationType.Up : HitAnimationType.Back);
			m_priority = textsection.GetAttribute("priority", Combat.HitPriority.Default);
			m_damage = textsection.GetAttribute<Evaluation.Expression>("damage", null);
			m_pausetime = textsection.GetAttribute<Evaluation.Expression>("pausetime", null);
			m_guardpausetime = textsection.GetAttribute<Evaluation.Expression>("guard.pausetime", null);
			m_sparknumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("sparkno", null);
			m_guardsparknumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("guard.sparkno", null);
			m_sparkposition = textsection.GetAttribute<Evaluation.Expression>("sparkxy", null);
			m_hitsound = textsection.GetAttribute<Evaluation.PrefixedExpression>("hitsound", null);
			m_guardhitsound = textsection.GetAttribute<Evaluation.PrefixedExpression>("guardsound", null);
			m_attackeffect = textsection.GetAttribute("ground.type", AttackEffect.High);
			m_aireffect = textsection.GetAttribute("air.type", GroundAttackEffect);
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

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (HitAttribute == null) return false;

			return true;
		}

		protected void SetHitDefinition(Combat.Character character, Combat.HitDefinition hitdef)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			var defaulthitspark = EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultSparkNumber, -1);
			var defaultplayerhitspark = !EvaluationHelper.IsCommon(character.BasePlayer.Constants.DefaultSparkNumber, true);

			var defaultguardspark = EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultGuardSparkNumber, -1);
			var defaultplayerguardspark = !EvaluationHelper.IsCommon(character.BasePlayer.Constants.DefaultGuardSparkNumber, true); 

			hitdef.HitAttribute = HitAttribute;
			hitdef.HitFlag = HitFlag;
			hitdef.GuardFlag = GuardFlag;
			hitdef.Targeting = Targetting;
			hitdef.GroundAnimationType = HitAnimationType;
			hitdef.AirAnimationType = AirHitAnimationType;
			hitdef.FallAnimationType = FallHitAnimationType;
			hitdef.HitPriority = Priority;

			var damage = EvaluationHelper.AsPoint(character, Damage, new Point(0, 0));
			hitdef.HitDamage = damage.X;
			hitdef.GuardDamage = damage.Y;

			var pauseshaketime = EvaluationHelper.AsPoint(character, PauseTime, new Point(0, 0));
			hitdef.PauseTime = pauseshaketime.X;
			hitdef.ShakeTime = pauseshaketime.Y;

			var guardpauseshaketime = EvaluationHelper.AsPoint(character, GuardPauseTime, pauseshaketime);
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
			hitdef.GroundCornerPush = EvaluationHelper.AsSingle(character, GroundCornerPushOff, hitdef.HitAttribute.HasHeight(AttackStateType.Air) ? 0.0f : hitdef.GroundGuardVelocity.X * 1.3f);
			hitdef.AirCornerPush = EvaluationHelper.AsSingle(character, AirCornerPushOff, hitdef.GroundCornerPush);
			hitdef.DownCornerPush = EvaluationHelper.AsSingle(character, DownCornerPushOff, hitdef.GroundCornerPush);
			hitdef.GuardCornerPush = EvaluationHelper.AsSingle(character, GuardCornerPushOff, hitdef.GroundCornerPush);
			hitdef.AirGuardCornerPush = EvaluationHelper.AsSingle(character, AirGuardCornerPushOff, hitdef.GroundCornerPush);
			hitdef.JugglePointsNeeded = EvaluationHelper.AsInt32(character, JugglePointsNeeded, 0);
			hitdef.MininumDistance = EvaluationHelper.AsPoint(character, MinimumDistance, null);
			hitdef.MaximumDistance = EvaluationHelper.AsPoint(character, MaximumDistance, null);
			hitdef.SnapLocation = EvaluationHelper.AsPoint(character, Snap, null);
			hitdef.P1SpritePriority = EvaluationHelper.AsInt32(character, P1SpritePriority, 1);
			hitdef.P2SpritePriority = EvaluationHelper.AsInt32(character, P2SpritePriority, 0);
			hitdef.P1Facing = EvaluationHelper.AsInt32(character, P1Facing, 0);
			hitdef.P1GetP2Facing = EvaluationHelper.AsInt32(character, P1GetP2Facing, 0);
			hitdef.P2Facing = EvaluationHelper.AsInt32(character, P2Facing, 0);
			hitdef.P1NewState = EvaluationHelper.AsInt32(character, P1StateNumber, null);
			hitdef.P2NewState = EvaluationHelper.AsInt32(character, P2StateNumber, null);
			hitdef.P2UseP1State = EvaluationHelper.AsBoolean(character, P2GetP1StateNumber, true);
			hitdef.ForceStand = EvaluationHelper.AsBoolean(character, ForceStand, hitdef.GroundVelocity.Y != 0 ? true : false);
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

			var nochainid = EvaluationHelper.AsPoint(character, NoChainId, new Point(-1, -1));
			hitdef.NoChainId1 = nochainid.X;
			hitdef.NoChainId2 = nochainid.Y;

			hitdef.HitOnce = EvaluationHelper.AsBoolean(character, HitOnce, hitdef.HitAttribute.HasData(new Combat.HitType(AttackClass.Throw, AttackPower.All)) ? true : false);
			hitdef.CanKill = EvaluationHelper.AsBoolean(character, CanKill, true);
			hitdef.CanGuardKill = EvaluationHelper.AsBoolean(character, CanGuardKill, true);
			hitdef.CanFallKill = EvaluationHelper.AsBoolean(character, CanFallKill, true);
			hitdef.NumberOfHits = EvaluationHelper.AsInt32(character, NumberOfHits, 1);

			if (P1PowerIncrease != null)
			{
				var statepower = P1PowerIncrease.Evaluate(character);

				if (statepower.Length > 0 && statepower[0].NumberType != NumberType.None)
				{
					hitdef.P1HitPowerAdjustment = statepower[0].IntValue;
				}
				else
				{
					hitdef.P1HitPowerAdjustment = (int)(hitdef.HitDamage * 0.7f);
				}

				if (statepower.Length > 1 && statepower[1].NumberType != NumberType.None)
				{
					hitdef.P1GuardPowerAdjustment = statepower[1].IntValue;
				}
				else
				{
					hitdef.P1GuardPowerAdjustment = (int)(hitdef.P1HitPowerAdjustment * 0.5f);
				}
			}

			if (P2PowerIncrease != null)
			{
				var p2power = P2PowerIncrease.Evaluate(character);

				if (p2power.Length > 0 && p2power[0].NumberType != NumberType.None)
				{
					hitdef.P2HitPowerAdjustment = p2power[0].IntValue;
				}
				else
				{
					hitdef.P2HitPowerAdjustment = (int)(hitdef.HitDamage * 0.6f);
				}

				if (p2power.Length > 1 && p2power[1].NumberType != NumberType.None)
				{
					hitdef.P2GuardPowerAdjustment = p2power[1].IntValue;
				}
				else
				{
					hitdef.P2GuardPowerAdjustment = (int)(hitdef.P2HitPowerAdjustment * 0.5f);
				}
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

		public Combat.HitAttribute HitAttribute => m_hitattr;

		public Combat.HitFlag HitFlag => m_hitflag;

		public Combat.HitFlag GuardFlag => m_guardflag;

		public AffectTeam Targetting => m_affectteam;

		public HitAnimationType HitAnimationType => m_hitanimtype;

		public HitAnimationType AirHitAnimationType => m_airhitanimtype;

		public HitAnimationType FallHitAnimationType => m_fallhitanimtype;

		public Combat.HitPriority Priority => m_priority;

		public Evaluation.Expression Damage => m_damage;

		public Evaluation.Expression PauseTime => m_pausetime;

		public Evaluation.Expression GuardPauseTime => m_guardpausetime;

		public Evaluation.PrefixedExpression SparkNumber => m_sparknumber;

		public Evaluation.PrefixedExpression GuardSparkNumber => m_guardsparknumber;

		public Evaluation.Expression SparkPosition => m_sparkposition;

		public Evaluation.PrefixedExpression HitSound => m_hitsound;

		public Evaluation.PrefixedExpression GuardHitSound => m_guardhitsound;

		public AttackEffect GroundAttackEffect => m_attackeffect;

		public AttackEffect AirAttackEffect => m_aireffect;

		public Evaluation.Expression GroundSlideTime => m_groundslidetime;

		public Evaluation.Expression GuardSlideTime => m_guardslidetime;

		public Evaluation.Expression GroundHitTime => m_groundhittime;

		public Evaluation.Expression GuardHitTime => m_guardhittime;

		public Evaluation.Expression AirHitTime => m_airhittime;

		public Evaluation.Expression GuardControlTime => m_guardctrltime;

		public Evaluation.Expression GuardDistance => m_guarddistance;

		public Evaluation.Expression VerticalAcceleration => m_yaccel;

		public Evaluation.Expression GroundVelocity => m_groundvelocity;

		public Evaluation.Expression GuardVelocity => m_guardvelocity;

		public Evaluation.Expression AirVelocity => m_airvelocity;

		public Evaluation.Expression AirGuardVelocity => m_airguardvelocity;

		public Evaluation.Expression GroundCornerPushOff => m_groundcornerpushoff;

		public Evaluation.Expression AirCornerPushOff => m_aircornerpushoff;

		public Evaluation.Expression DownCornerPushOff => m_downcornerpushoff;

		public Evaluation.Expression GuardCornerPushOff => m_guardcornerpushoff;

		public Evaluation.Expression AirGuardCornerPushOff => m_airguardcornerpushoff;

		public Evaluation.Expression AirGuardControlTime => m_airguardctrltime;

		public Evaluation.Expression JugglePointsNeeded => m_airjuggle;

		public Evaluation.Expression MinimumDistance => m_mindistance;

		public Evaluation.Expression MaximumDistance => m_maxdistance;

		public Evaluation.Expression Snap => m_snap;

		public Evaluation.Expression P1SpritePriority => m_p1spritepriority;

		public Evaluation.Expression P2SpritePriority => m_p2spritepriority;

		public Evaluation.Expression P1Facing => m_p1facing;

		public Evaluation.Expression P1GetP2Facing => m_p1getp2facing;

		public Evaluation.Expression P2Facing => m_p2facing;

		public Evaluation.Expression P1StateNumber => m_p1statenumber;

		public Evaluation.Expression P2StateNumber => m_p2statenumber;

		public Evaluation.Expression P2GetP1StateNumber => m_p2getp1state;

		public Evaluation.Expression ForceStand => m_forcestand;

		public Evaluation.Expression Fall => m_fall;

		public Evaluation.Expression FallXVelocity => m_fallxvelocity;

		public Evaluation.Expression FallYVelocity => m_fallyvelocity;

		public Evaluation.Expression FallCanRecover => m_fallrecover;

		public Evaluation.Expression FallRecoverTime => m_fallrecovertime;

		public Evaluation.Expression FallDamage => m_falldamage;

		public Evaluation.Expression AirFall => m_airfall;

		public Evaluation.Expression DownVelocity => m_downvelocity;

		public Evaluation.Expression DownHitTime => m_downhittime;

		public Evaluation.Expression DownBounce => m_downbounce;

		public Evaluation.Expression TargetId => m_targetid;

		public Evaluation.Expression ChainId => m_chainid;

		public Evaluation.Expression NoChainId => m_nochainid;

		public Evaluation.Expression HitOnce => m_hitonce;

		public Evaluation.Expression CanKill => m_kill;

		public Evaluation.Expression CanGuardKill => m_guardkill;

		public Evaluation.Expression CanFallKill => m_fallkill;

		public Evaluation.Expression NumberOfHits => m_numberofhits;

		public Evaluation.Expression P1PowerIncrease => m_p1powerincrease;

		public Evaluation.Expression P2PowerIncrease => m_p2powerincrease;

		public Evaluation.Expression PaletteColorTime => m_paltime;

		public Evaluation.Expression PaletteColorMultiply => m_palmul;

		public Evaluation.Expression PaletteColorAdd => m_paladd;

		public Evaluation.Expression PaletteColorSineAdd => m_palsinadd;

		public Evaluation.Expression PaletteColorInversion => m_palinvert;

		public Evaluation.Expression PaletteColor => m_palcolor;

		public Evaluation.Expression ShakeTime => m_shaketime;

		public Evaluation.Expression ShakeFrequency => m_shakefreq;

		public Evaluation.Expression ShakeAmplitude => m_shakeamplitude;

		public Evaluation.Expression ShakePhaseOffset => m_shakephaseoffset;

		public Evaluation.Expression FallShakeTime => m_fallshaketime;

		public Evaluation.Expression FallShakeFrequency => m_fallshakefreq;

		public Evaluation.Expression FallShakeAmplitude => m_fallshakeamplitude;

		public Evaluation.Expression FallShakePhaseOffset => m_fallshakephaseoffset;

		public Evaluation.Expression AttackWidth => m_attackwidth;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.HitAttribute m_hitattr;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.HitFlag m_hitflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.HitFlag m_guardflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AffectTeam m_affectteam;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly HitAnimationType m_hitanimtype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly HitAnimationType m_airhitanimtype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly HitAnimationType m_fallhitanimtype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.HitPriority m_priority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_damage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardpausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_sparknumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_guardsparknumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_sparkposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_hitsound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_guardhitsound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AttackEffect m_attackeffect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AttackEffect m_aireffect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_groundslidetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardslidetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_groundhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardctrltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guarddistance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_yaccel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_groundvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airguardvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_groundcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_aircornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_downcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airguardcornerpushoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airguardctrltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airjuggle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_mindistance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_maxdistance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_snap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p1spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p2spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p1facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p1getp2facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p2facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p1statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p2statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p2getp1state;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_forcestand;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fall;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallxvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallyvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallrecover;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallrecovertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_falldamage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airfall;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_downvelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_downhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_downbounce;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_chainid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_nochainid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_hitonce;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_kill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_guardkill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallkill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_numberofhits;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p1powerincrease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_p2powerincrease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_paltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_paladd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palsinadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_shaketime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_shakefreq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_shakeamplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_shakephaseoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallshaketime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallshakefreq;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallshakeamplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_fallshakephaseoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_attackwidth;

		#endregion
	}
}