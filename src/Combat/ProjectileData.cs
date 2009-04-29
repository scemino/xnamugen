using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
	class ProjectileData
	{
		public ProjectileData()
		{
		}

		public HitDefinition HitDef { get; set; }
		public Int32 ProjectileId { get; set; }
		public Int32 AnimationNumber { get; set; }
		public Int32 HitAnimationNumber { get; set; }
		public Int32 RemoveAnimationNumber { get; set; }
		public Int32 CancelAnimationNumber { get; set; }
		public Vector2 Scale { get; set; }
		public Boolean RemoveOnHit { get; set; }
		public Int32 RemoveTimeout { get; set; }
		public Vector2 InitialVelocity { get; set; }
		public Vector2 RemoveVelocity { get; set; }
		public Vector2 Acceleration { get; set; }
		public Vector2 VelocityMultiplier { get; set; }
		public Int32 HitsBeforeRemoval { get; set; }
		public Int32 TimeBetweenHits { get; set; }
		public Int32 Priority { get; set; }
		public Int32 SpritePriority { get; set; }
		public Int32 ScreenEdgeBound { get; set; }
		public Int32 StageEdgeBound { get; set; }
		public Int32 HeightLowerBound { get; set; }
		public Int32 HeightUpperBound { get; set; }
		public Vector2 CreationOffset { get; set; }
		public PositionType PositionType { get; set; }
		public Vector4 ShadowColor { get; set; }
		public Int32 SuperPauseMoveTime { get; set; }
		public Int32 PauseMoveTime { get; set; }
		public Int32 AfterImageTime { get; set; }
		public Int32 AfterImageNumberOfFrames { get; set; }
		public Vector3 AfterImageBaseColor { get; set; }
		public Boolean AfterImageInvertColor { get; set; }
		public Vector3 AfterImagePreAddColor { get; set; }
		public Vector3 AfterImageConstrast { get; set; }
		public Vector3 AfterImagePostAddColor { get; set; }
		public Vector3 AfterImagePaletteColorAdd { get; set; }
		public Vector3 AfterImagePaletteColorMul { get; set; }
		public Int32 AfterImageTimeGap { get; set; }
		public Int32 AfterImageFrameGap { get; set; }
		public Blending AfterImageTransparency { get; set; }
	}
}