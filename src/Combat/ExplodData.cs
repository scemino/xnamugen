using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("Id #{Id} - {CommonAnimation}, {AnimationNumber}")]
	class ExplodData
	{
		public ExplodData()
		{
			Type = ExplodType.None;

			BindTime = 1;
		}

		public ExplodType Type { get; set; }

		public Boolean CommonAnimation { get; set; }
		public Int32 AnimationNumber { get; set; }
		public Int32? Id { get; set; }
		public PositionType PositionType { get; set; }
		public Vector2 Location { get; set; }
		public Vector2 Velocity { get; set; }
		public Vector2 Acceleration { get; set; }
		public SpriteEffects Flip { get; set; }
		public Int32 BindTime { get; set; }
		public Int32 RemoveTime { get; set; }
		public Point Random { get; set; }
		public Boolean SuperMove { get; set; }
		public Int32 SuperMoveTime { get; set; }
		public Int32 PauseTime { get; set; }
		public Vector2 Scale { get; set; }
		public Int32 SpritePriority { get; set; }
		public Boolean DrawOnTop { get; set; }
		public Boolean OwnPalFx { get; set; }
		public Boolean RemoveOnGetHit { get; set; }
		public Boolean IgnoreHitPause { get; set; }
		public Blending Transparency { get; set; }
		public Vector4 ShadowColor { get; set; }

		public Entity Creator { get; set; }
		public Entity Offseter { get; set; }
	}

	[DebuggerDisplay("Id #{Id} - {CommonAnimation}, {AnimationNumber}")]
	class ModifyExplodData
	{
		public Boolean? CommonAnimation { get; set; }
		public Int32? AnimationNumber { get; set; }
		public Int32 Id { get; set; }
		public PositionType? PositionType { get; set; }
		public Vector2? Location { get; set; }
		public Vector2? Velocity { get; set; }
		public Vector2? Acceleration { get; set; }
		public SpriteEffects? Flip { get; set; }
		public Int32? BindTime { get; set; }
		public Int32? RemoveTime { get; set; }
		public Point? Random { get; set; }
		public Boolean? SuperMove { get; set; }
		public Int32? SuperMoveTime { get; set; }
		public Int32? PauseTime { get; set; }
		public Vector2? Scale { get; set; }
		public Int32? SpritePriority { get; set; }
		public Boolean? DrawOnTop { get; set; }
		public Boolean? OwnPalFx { get; set; }
		public Boolean? RemoveOnGetHit { get; set; }
		public Boolean? IgnoreHitPause { get; set; }
		public Blending? Transparency { get; set; }
		public Vector4? ShadowColor { get; set; }
	}
}