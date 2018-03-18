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
			DrawOrder = 0;
			CurrentLocation = new Vector2(0, 0);
			CurrentVelocity = new Vector2(0, 0);
			CurrentAcceleration = new Vector2(0, 0);
			m_facing = Facing.Right;
			CurrentFlip = SpriteEffects.None;
			CurrentScale = new Vector2(1, 1);
			Transparency = new Blending();
			m_afterimages = new AfterImage(this);
			DrawingAngle = 0;
		}

		public virtual void Reset()
		{
			DrawOrder = 0;
			CurrentLocation = new Vector2(0, 0);
			CurrentVelocity = new Vector2(0, 0);
			CurrentAcceleration = new Vector2(0, 0);
			m_facing = Facing.Right;
			CurrentFlip = SpriteEffects.None;
			CurrentScale = new Vector2(1, 1);
			Transparency = new Blending();
			DrawingAngle = 0;
			AngleDraw = false;

			m_afterimages.Reset();
		}

		public virtual void CleanUp()
		{
			AngleDraw = false;
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

		public int DrawOrder { get; set; }

		public Vector2 CurrentLocation { get; set; }

		public Vector2 CurrentVelocity { get; set; }

		public Vector2 CurrentAcceleration { get; set; }

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

		public SpriteEffects CurrentFlip { get; set; }

		public Vector2 CurrentScale { get; set; }

		public Texture2D CurrentPalette { get; set; }

		public Blending Transparency { get; set; }

		public AfterImage AfterImages => m_afterimages;

		public float DrawingAngle { get; set; }

		public bool AngleDraw { get; set; }

		public abstract PaletteFx PaletteFx { get; }

		public abstract Team Team { get; }

		public abstract Player BasePlayer { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Facing m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly AfterImage m_afterimages;

		#endregion
	}
}