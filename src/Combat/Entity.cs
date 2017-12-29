using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	internal abstract class Entity : EngineObject
	{
		protected Entity(FightEngine engine)
			: base(engine)
		{
			m_draworder = 0;
			m_location = new Vector2(0, 0);
			m_velocity = new Vector2(0, 0);
			m_acceleration = new Vector2(0, 0);
			m_facing = Facing.Right;
			m_flip = SpriteEffects.None;
			m_scale = new Vector2(1, 1);
			m_blending = new Blending();
			m_afterimages = new AfterImage(this);
			m_drawingangle = 0;
		}

		public virtual void Reset()
		{
			m_draworder = 0;
			m_location = new Vector2(0, 0);
			m_velocity = new Vector2(0, 0);
			m_acceleration = new Vector2(0, 0);
			m_facing = Facing.Right;
			m_flip = SpriteEffects.None;
			m_scale = new Vector2(1, 1);
			m_blending = new Blending();
			m_drawingangle = 0;
			m_angledraw = false;

			m_afterimages.Reset();
		}

		public virtual void CleanUp()
		{
			m_angledraw = false;
		}

		public void SetLocalAnimation(int animnumber, int elementnumber)
		{
			if (AnimationManager.SetLocalAnimation(animnumber, elementnumber))
			{
				SpriteManager.LoadSprites(AnimationManager.CurrentAnimation);
			}
		}

		public void SetForeignAnimation(Animations.AnimationManager animationmanager, int animationnumber, int elementnumber)
		{
			if (AnimationManager.SetForeignAnimation(animationmanager, animationnumber, elementnumber))
			{
				SpriteManager.LoadSprites(AnimationManager.CurrentAnimation);
			}
		}

		public virtual void UpdateAnimations()
		{
			UpdateAnimations(true);
		}

		public virtual void UpdateAnimations(bool updatepalfx)
		{
			AnimationManager.Update();

			if (updatepalfx) PaletteFx.Update();
		}

        public virtual void UpdateAfterImages()
        {
            AfterImages.Update();
        }

		public virtual void UpdatePhsyics()
		{
			Move(CurrentVelocity);

			CurrentVelocity += CurrentAcceleration;
		}

		public virtual void UpdateInput()
		{
		}

		public virtual void UpdateState()
		{
		}

		public virtual Vector2 Move(Vector2 p)
		{
			var oldlocation = CurrentLocation;

			if (CurrentFacing == Facing.Right) CurrentLocation += p;

			if (CurrentFacing == Facing.Left) CurrentLocation += new Vector2(-p.X, p.Y);

			Bounding();

			var movement = CurrentLocation - oldlocation;
			return movement;
		}

		protected virtual void Bounding()
		{
		}

		public virtual bool IsPaused(Pause pause)
		{
			if (pause == null) throw new ArgumentNullException(nameof(pause));

			if (pause.IsActive == false) return false;
			if (this == pause.Creator && pause.ElapsedTime <= pause.MoveTime) return false;

			return true;
		}

		public Vector2 MoveLeft(Vector2 p)
		{
			if (CurrentFacing == Facing.Right) return Move(new Vector2(-p.X, p.Y));

			if (CurrentFacing == Facing.Left) return Move(p);

			return new Vector2();
		}

		public Vector2 MoveRight(Vector2 p)
		{
			if (CurrentFacing == Facing.Right) return Move(p);

			if (CurrentFacing == Facing.Left) return Move(new Vector2(-p.X, p.Y));

			return new Vector2();
		}

		public virtual void Draw()
		{
			AfterImages.Draw();

			var currentelement = AnimationManager.CurrentElement;
			if (currentelement == null) return;

			var sprite = SpriteManager.GetSprite(currentelement.SpriteId);
			if (sprite == null) return;

			var drawlocation = GetDrawLocation();
			var drawoffset = Misc.GetOffset(Vector2.Zero, CurrentFacing, currentelement.Offset);

			SpriteManager.OverridePalette = CurrentPalette;

			var drawscale = CurrentScale;
			if (this is Character) drawscale *= (this as Character).DrawScale;

			var drawstate = SpriteManager.SetupDrawing(currentelement.SpriteId, drawlocation, drawoffset, drawscale, GetDrawFlip());

			drawstate.Blending = Transparency == new Blending() ? currentelement.Blending : Transparency;
			drawstate.Rotation = AngleDraw ? Misc.FaceScalar(CurrentFacing, -DrawingAngle) : 0;

			PaletteFx.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		public virtual void DebugDraw()
		{
			var currentelement = AnimationManager.CurrentElement;
			if (currentelement == null) return;

			var location = GetDrawLocation();

			//DrawSpriteBox(location, currentelement);
			DrawClsnBoxes(location, currentelement);
			DrawOriginCross(location, 3);
		}

		private void DrawSpriteBox(Vector2 location, Animations.AnimationElement element)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));

			var sprite = SpriteManager.GetSprite(element.SpriteId);
			if (sprite == null) return;

			var drawoffset = Vector2.Zero;
			var flip = GetDrawFlip();

			switch (CurrentFacing)
			{
				case Facing.Right:
					drawoffset = element.Offset;
					break;

				case Facing.Left:
					drawoffset = new Vector2(-element.Offset.X, element.Offset.Y);
					break;
			}

			if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) drawoffset.X = -drawoffset.X;
			if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) drawoffset.Y = -drawoffset.Y;

			var drawstate = SpriteManager.DrawState;

			var spritelocation = Video.Renderer.GetDrawLocation(sprite.Size, location, (Vector2)sprite.Axis - drawoffset, CurrentScale, flip);
			drawstate.Reset();
			drawstate.Mode = DrawMode.OutlinedRectangle;
			drawstate.AddData(spritelocation, new Rectangle(0, 0, (int)(sprite.Size.X * CurrentScale.X), (int)(sprite.Size.Y * CurrentScale.Y)), Color.Gray);
			drawstate.Use();
		}

		private void DrawClsnBoxes(Vector2 location, Animations.AnimationElement element)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));

			var drawstate = SpriteManager.DrawState;
			drawstate.Reset();

			drawstate.Mode = DrawMode.OutlinedRectangle;

			foreach (var clsn in element)
			{
				var rect = clsn.MakeRect(location, CurrentScale, CurrentFacing);

				switch (clsn.ClsnType)
				{
					case ClsnType.Type1Attack:
						drawstate.AddData(new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.Red);
						break;

					case ClsnType.Type2Normal:
						drawstate.AddData(new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.Blue);
						break;
				}
			}

			drawstate.Use();
		}

		private void DrawOriginCross(Vector2 location, int segmentlength)
		{
			var drawstate = SpriteManager.DrawState;
			drawstate.Reset();

			drawstate.Mode = DrawMode.Lines;
			drawstate.AddData(location - new Vector2(segmentlength, 0), null, Color.Black);
			drawstate.AddData(location + new Vector2(segmentlength, 0), null, Color.Black);
			drawstate.AddData(location - new Vector2(0, segmentlength), null, Color.Black);
			drawstate.AddData(location + new Vector2(0, segmentlength), null, Color.Black);
			drawstate.Use();
		}

		public abstract bool RemoveCheck();

		public abstract Vector2 GetDrawLocation();

		public SpriteEffects GetDrawFlip()
		{
			var currentelement = AnimationManager.CurrentElement;

			var flip = CurrentFlip ^ currentelement.Flip;

			if (CurrentFacing == Facing.Left) flip ^= SpriteEffects.FlipHorizontally;

			return flip;
		}

		public abstract Drawing.SpriteManager SpriteManager { get; }

		public abstract Animations.AnimationManager AnimationManager { get; }

		public abstract EntityUpdateOrder UpdateOrder { get; }

		public int DrawOrder
		{
			get => m_draworder;

			set { m_draworder = value; }
		}

		public Vector2 CurrentLocation
		{
			get => m_location;

			set { m_location = value; }
		}

		public Vector2 CurrentVelocity
		{
			get => m_velocity;

			set { m_velocity = value; }
		}

		public Vector2 CurrentAcceleration
		{
			get => m_acceleration;

			set { m_acceleration = value; }
		}

		public Facing CurrentFacing
		{
			get => m_facing;

			set
			{
				if (m_facing != value)
				{
					CurrentVelocity *= new Vector2(-1, 1);
					CurrentAcceleration *= new Vector2(-1, 1);
				}

				m_facing = value;
			}
		}

		public SpriteEffects CurrentFlip
		{
			get => m_flip;

			set { m_flip = value; }
		}

		public Vector2 CurrentScale
		{
			get => m_scale;

			set { m_scale = value; }
		}

		public Texture2D CurrentPalette
		{
			get => m_currentpalette;

			set { m_currentpalette = value; }
		}

		public Blending Transparency
		{
			get => m_blending;

			set { m_blending = value; }
		}

		public AfterImage AfterImages => m_afterimages;

		public float DrawingAngle
		{
			get => m_drawingangle;

			set { m_drawingangle = value; }
		}

		public bool AngleDraw
		{
			get => m_angledraw;

			set { m_angledraw = value; }
		}

		public abstract PaletteFx PaletteFx { get; }

		public abstract Team Team { get; }

		public abstract Player BasePlayer { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_acceleration;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Facing m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Texture2D m_currentpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AfterImage m_afterimages;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_draworder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_drawingangle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_angledraw;

		#endregion
	}
}