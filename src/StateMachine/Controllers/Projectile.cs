using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Projectile")]
	class Projectile : HitDef
	{
		public Projectile(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_priority = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_priority = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_projectileid = textsection.GetAttribute<Evaluation.Expression>("ProjID", null);
			m_animation = textsection.GetAttribute<Evaluation.Expression>("projanim", null);
			m_hitanimation = textsection.GetAttribute<Evaluation.Expression>("projhitanim", null);
			m_removeanimation = textsection.GetAttribute<Evaluation.Expression>("projremanim", null);
			m_cancelanimation = textsection.GetAttribute<Evaluation.Expression>("projcancelanim", null);
			m_scale = textsection.GetAttribute<Evaluation.Expression>("projscale", null);
			m_removeonhit = textsection.GetAttribute<Evaluation.Expression>("projremove", null);
			m_removetime = textsection.GetAttribute<Evaluation.Expression>("projremovetime", null);
			m_velocity = textsection.GetAttribute<Evaluation.Expression>("velocity", null);
			m_removevelocity = textsection.GetAttribute<Evaluation.Expression>("remvelocity", null);
			m_acceleration = textsection.GetAttribute<Evaluation.Expression>("accel", null);
			m_velocitymultiplier = textsection.GetAttribute<Evaluation.Expression>("velmul", null);
			m_hits = textsection.GetAttribute<Evaluation.Expression>("projhits", null);
			m_misstime = textsection.GetAttribute<Evaluation.Expression>("projmisstime", null);
			m_priority = textsection.GetAttribute<Evaluation.Expression>("projpriority", null);
			m_spritepriority = textsection.GetAttribute<Evaluation.Expression>("projsprpriority", null);
			m_edgebound = textsection.GetAttribute<Evaluation.Expression>("projedgebound", null);
			m_stagebound = textsection.GetAttribute<Evaluation.Expression>("projstagebound", null);
			m_heightbound = textsection.GetAttribute<Evaluation.Expression>("projheightbound", null);
			m_offset = textsection.GetAttribute<Evaluation.Expression>("offset", null);
			m_postype = textsection.GetAttribute<PositionType>("postype", PositionType.P1);
			m_shadow = textsection.GetAttribute<Evaluation.Expression>("projshadow", null);
			m_supermovetime = textsection.GetAttribute<Evaluation.Expression>("supermovetime", null);
			m_pausetime = textsection.GetAttribute<Evaluation.Expression>("pausemovetime", null);
			m_afterimagetime = textsection.GetAttribute<Evaluation.Expression>("afterimage.Time", null);
			m_afterimageframes = textsection.GetAttribute<Evaluation.Expression>("afterimage.length", null);
			m_afterimagecolor = textsection.GetAttribute<Evaluation.Expression>("afterimage.palcolor", null);
			m_afterimageinversion = textsection.GetAttribute<Evaluation.Expression>("afterimage.palinvertall", null);
			m_afterimageprebrightness = textsection.GetAttribute<Evaluation.Expression>("afterimage.palbright", null);
			m_afterimagecontrast = textsection.GetAttribute<Evaluation.Expression>("afterimage.palcontrast", null);
			m_afterimagepostbrightness = textsection.GetAttribute<Evaluation.Expression>("afterimage.palpostbright", null);
			m_afterimagecoloradd = textsection.GetAttribute<Evaluation.Expression>("afterimage.paladd", null);
			m_afterimagecolormul = textsection.GetAttribute<Evaluation.Expression>("afterimage.palmul", null);
			m_afterimagetimegap = textsection.GetAttribute<Evaluation.Expression>("afterimage.TimeGap", null);
			m_afterimageframegap = textsection.GetAttribute<Evaluation.Expression>("afterimage.FrameGap", null);
			m_afterimageblending = textsection.GetAttribute<Blending>("afterimage.trans", new Blending());
		}

		public override void Run(Combat.Character character)
		{
			Combat.ProjectileData data = new Combat.ProjectileData();

			if (HitAttribute != null)
			{
				data.HitDef = new Combat.HitDefinition();
				SetHitDefinition(character, data.HitDef);
			}

			data.ProjectileId = EvaluationHelper.AsInt32(character, ProjectileId, 0);
			data.AnimationNumber = EvaluationHelper.AsInt32(character, ProjectileAnimationNumber, 0);
			data.HitAnimationNumber = EvaluationHelper.AsInt32(character, ProjectileHitAnimationNumber, -1);
			data.RemoveAnimationNumber = EvaluationHelper.AsInt32(character, ProjectileRemoveAnimationNumber, data.HitAnimationNumber);
			data.CancelAnimationNumber = EvaluationHelper.AsInt32(character, ProjectileRemoveAnimationNumber, data.RemoveAnimationNumber);
			data.Scale = EvaluationHelper.AsVector2(character, ProjectileScale, Vector2.One);
			data.RemoveOnHit = EvaluationHelper.AsBoolean(character, ProjectileRemoveOnHit, true);
			data.RemoveTimeout = EvaluationHelper.AsInt32(character, ProjectileRemoveTime, -1);
			data.InitialVelocity = EvaluationHelper.AsVector2(character, ProjectileVelocity, Vector2.Zero);
			data.RemoveVelocity = EvaluationHelper.AsVector2(character, ProjectileRemoveVelocity, Vector2.Zero);
			data.Acceleration = EvaluationHelper.AsVector2(character, ProjectileAcceleration, Vector2.Zero);
			data.VelocityMultiplier = EvaluationHelper.AsVector2(character, ProjectileVelocityMultiplier, Vector2.One);
			data.HitsBeforeRemoval = EvaluationHelper.AsInt32(character, ProjectileHits, 1);
			data.TimeBetweenHits = EvaluationHelper.AsInt32(character, ProjectileMissTime, 0);
			data.Priority = EvaluationHelper.AsInt32(character, ProjectilePriority, 1);
			data.SpritePriority = EvaluationHelper.AsInt32(character, ProjectileSpritePriority, 3);
			data.ScreenEdgeBound = EvaluationHelper.AsInt32(character, ProjectileScreenBound, 40);
			data.StageEdgeBound = EvaluationHelper.AsInt32(character, ProjectileStageBound, 40);

			Point heightbounds = EvaluationHelper.AsPoint(character, ProjectileHeightBound, new Point(-240, 1));
			data.HeightLowerBound = heightbounds.X;
			data.HeightUpperBound = heightbounds.Y;

			data.CreationOffset = (Vector2)EvaluationHelper.AsPoint(character, ProjectileCreationOffset, new Point(0, 0));
			data.PositionType = ProjectileCreationPositionType;

			data.ShadowColor = EvaluationHelper.AsVector4(character, ProjectileShadow, Vector4.Zero);
			if (data.ShadowColor.X == -1) data.ShadowColor = character.Engine.Stage.ShadowColor;

			data.SuperPauseMoveTime = EvaluationHelper.AsInt32(character, ProjectileSuperPauseTime, 0);
			data.PauseMoveTime = EvaluationHelper.AsInt32(character, ProjectilePauseTime, 0);
			data.AfterImageTime = EvaluationHelper.AsInt32(character, ProjectileAfterImageTime, 1);
			data.AfterImageNumberOfFrames = EvaluationHelper.AsInt32(character, ProjectileAfterImageNumberOfFrames, 20);

			Int32 basecolor = EvaluationHelper.AsInt32(character, ProjectileAfterImagePaletteColor, 255);
			data.AfterImageBaseColor = Vector3.Clamp(new Vector3(basecolor) / 255.0f, Vector3.Zero, Vector3.One);

			data.AfterImageInvertColor = EvaluationHelper.AsBoolean(character, ProjectileAfterImagePaletteColorInversion, false);

			Vector3 brightness = EvaluationHelper.AsVector3(character, ProjectileAfterImagePaletteColorBrightness, new Vector3(30, 30, 30));
			data.AfterImagePreAddColor = Vector3.Clamp(brightness / 255.0f, Vector3.Zero, Vector3.One);

			Vector3 contrast = EvaluationHelper.AsVector3(character, ProjectileAfterImagePaletteColorContrast, new Vector3(255, 255, 255));
			data.AfterImageConstrast = Vector3.Clamp(contrast / 255.0f, Vector3.Zero, new Vector3(Single.MaxValue));

			Vector3 postcolor = EvaluationHelper.AsVector3(character, ProjectileAfterImagePalettePostBrightness, new Vector3(0, 0, 0));
			data.AfterImagePostAddColor = Vector3.Clamp(postcolor / 255.0f, Vector3.Zero, Vector3.One);

			Vector3 coloradd = EvaluationHelper.AsVector3(character, ProjectileAfterImagePaletteColorAdd, new Vector3(10, 10, 25));
			data.AfterImagePaletteColorAdd = Vector3.Clamp(coloradd / 255.0f, Vector3.Zero, Vector3.One);

			Vector3 colormul = EvaluationHelper.AsVector3(character, ProjectileAfterImagePaletteColorMultiply, new Vector3(.65f, .65f, .75f));
			data.AfterImagePaletteColorMul = Vector3.Clamp(colormul, Vector3.Zero, new Vector3(Single.MaxValue));

			data.AfterImageTimeGap = EvaluationHelper.AsInt32(character, ProjectileAfterImageTimeGap, 1);
			data.AfterImageFrameGap = EvaluationHelper.AsInt32(character, ProjectileAfterImageFrameGap, 4);
			data.AfterImageTransparency = ProjectileAfterImageTransparency;

			Combat.Projectile projectile = new Combat.Projectile(character.Engine, character, data);
			projectile.Engine.Entities.Add(projectile);
		}

		public override Boolean IsValid()
		{
			return Triggers.IsValid;
		}

		public Evaluation.Expression ProjectileId
		{
			get { return m_projectileid; }
		}

		public Evaluation.Expression ProjectileAnimationNumber
		{
			get { return m_animation; }
		}

		public Evaluation.Expression ProjectileHitAnimationNumber
		{
			get { return m_hitanimation; }
		}

		public Evaluation.Expression ProjectileRemoveAnimationNumber
		{
			get { return m_removeanimation; }
		}

		public Evaluation.Expression ProjectileCancelAnimationNumber
		{
			get { return m_cancelanimation; }
		}

		public Evaluation.Expression ProjectileScale
		{
			get { return m_scale; }
		}

		public Evaluation.Expression ProjectileRemoveOnHit
		{
			get { return m_removeonhit; }
		}

		public Evaluation.Expression ProjectileRemoveTime
		{
			get { return m_removetime; }
		}

		public Evaluation.Expression ProjectileVelocity
		{
			get { return m_velocity; }
		}

		public Evaluation.Expression ProjectileRemoveVelocity
		{
			get { return m_removevelocity; }
		}

		public Evaluation.Expression ProjectileAcceleration
		{
			get { return m_acceleration; }
		}

		public Evaluation.Expression ProjectileVelocityMultiplier
		{
			get { return m_velocitymultiplier; }
		}

		public Evaluation.Expression ProjectileHits
		{
			get { return m_hits; }
		}

		public Evaluation.Expression ProjectileMissTime
		{
			get { return m_misstime; }
		}

		public Evaluation.Expression ProjectilePriority
		{
			get { return m_priority; }
		}

		public Evaluation.Expression ProjectileSpritePriority
		{
			get { return m_spritepriority; }
		}

		public Evaluation.Expression ProjectileScreenBound
		{
			get { return m_edgebound; }
		}

		public Evaluation.Expression ProjectileStageBound
		{
			get { return m_stagebound; }
		}

		public Evaluation.Expression ProjectileHeightBound
		{
			get { return m_heightbound; }
		}

		public Evaluation.Expression ProjectileCreationOffset
		{
			get { return m_offset; }
		}

		public PositionType ProjectileCreationPositionType
		{
			get { return m_postype; }
		}

		public Evaluation.Expression ProjectileShadow
		{
			get { return m_shadow; }
		}

		public Evaluation.Expression ProjectileSuperPauseTime
		{
			get { return m_supermovetime; }
		}

		public Evaluation.Expression ProjectilePauseTime
		{
			get { return m_pausetime; }
		}

		public Evaluation.Expression ProjectileAfterImageTime
		{
			get { return m_afterimagetime; }
		}

		public Evaluation.Expression ProjectileAfterImageNumberOfFrames
		{
			get { return m_afterimageframes; }
		}

		public Evaluation.Expression ProjectileAfterImagePaletteColor
		{
			get { return m_afterimagecolor; }
		}

		public Evaluation.Expression ProjectileAfterImagePaletteColorInversion
		{
			get { return m_afterimageinversion; }
		}

		public Evaluation.Expression ProjectileAfterImagePaletteColorBrightness
		{
			get { return m_afterimageprebrightness; }
		}

		public Evaluation.Expression ProjectileAfterImagePaletteColorContrast
		{
			get { return m_afterimagecontrast; }
		}

		public Evaluation.Expression ProjectileAfterImagePalettePostBrightness
		{
			get { return m_afterimagepostbrightness; }
		}

		public Evaluation.Expression ProjectileAfterImagePaletteColorAdd
		{
			get { return m_afterimagecoloradd; }
		}

		public Evaluation.Expression ProjectileAfterImagePaletteColorMultiply
		{
			get { return m_afterimagecolormul; }
		}

		public Evaluation.Expression ProjectileAfterImageTimeGap
		{
			get { return m_afterimagetimegap; }
		}

		public Evaluation.Expression ProjectileAfterImageFrameGap
		{
			get { return m_afterimageframegap; }
		}

		public Blending ProjectileAfterImageTransparency
		{
			get { return m_afterimageblending; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_projectileid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_animation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_hitanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_removeanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_cancelanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_removeonhit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_removetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_removevelocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_acceleration;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_velocitymultiplier;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_hits;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_misstime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_priority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_edgebound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_stagebound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_heightbound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PositionType m_postype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_supermovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimageframes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimageinversion;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimageprebrightness;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagecontrast;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagepostbrightness;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagecoloradd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagecolormul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimagetimegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_afterimageframegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending m_afterimageblending;

		#endregion
	}
}