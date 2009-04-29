using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	abstract class Entity : EngineObject
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

		public void SetLocalAnimation(Int32 animnumber, Int32 elementnumber)
		{
			if (AnimationManager.SetLocalAnimation(animnumber, elementnumber) == true)
			{
				SpriteManager.LoadSprites(AnimationManager.CurrentAnimation);
			}
			else
			{
			}
		}

		public void SetForeignAnimation(Animations.AnimationManager animationmanager, Int32 animationnumber, Int32 elementnumber)
		{
			if (AnimationManager.SetForeignAnimation(animationmanager, animationnumber, elementnumber) == true)
			{
				SpriteManager.LoadSprites(AnimationManager.CurrentAnimation);
			}
			else
			{
			}

		}

		public virtual void UpdateAnimations()
		{
			UpdateAnimations(true);
		}

		public virtual void UpdateAnimations(Boolean updatepalfx)
		{
			AnimationManager.Update();

			if (updatepalfx == true) PaletteFx.Update();
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
			Vector2 oldlocation = CurrentLocation;

			if (CurrentFacing == Facing.Right) CurrentLocation += p;

			if (CurrentFacing == Facing.Left) CurrentLocation += new Vector2(-p.X, p.Y);

			Bounding();

			Vector2 movement = CurrentLocation - oldlocation;
			return movement;
		}

		protected virtual void Bounding()
		{
		}

		public virtual Boolean IsPaused(Pause pause)
		{
			if (pause == null) throw new ArgumentNullException("pause");

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

		public virtual void ShadowDraw()
		{
		}

		public virtual void ReflectionDraw()
		{
		}

		public virtual void Draw()
		{
			AfterImages.Draw();

			Animations.AnimationElement currentelement = AnimationManager.CurrentElement;
			if (currentelement == null) return;

			Drawing.Sprite sprite = SpriteManager.GetSprite(currentelement.SpriteId);
			if (sprite == null) return;

			Vector2 drawlocation = GetDrawLocation();
			Vector2 drawoffset = Misc.GetOffset(Vector2.Zero, CurrentFacing, currentelement.Offset);

			Vector2 drawscale = CurrentScale;
			if (this is Character) drawscale *= (this as Character).DrawScale;

			Video.DrawState drawstate = SpriteManager.SetupDrawing(currentelement.SpriteId, drawlocation, drawoffset, drawscale, GetDrawFlip());
			drawstate.Blending = Transparency == new Blending() ? currentelement.Blending : Transparency;
			drawstate.Rotation = (AngleDraw == true) ? Misc.FaceScalar(CurrentFacing, -DrawingAngle) : 0;

			PaletteFx.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		public virtual void DebugDraw()
		{
			Animations.AnimationElement currentelement = AnimationManager.CurrentElement;
			if (currentelement == null) return;

			Vector2 location = GetDrawLocation();

			//DrawSpriteBox(location, currentelement);
			DrawClsnBoxes(location, currentelement);
			DrawOriginCross(location, 3);
		}

		void DrawSpriteBox(Vector2 location, Animations.AnimationElement element)
		{
			if (element == null) throw new ArgumentNullException("element");

			Drawing.Sprite sprite = SpriteManager.GetSprite(element.SpriteId);
			if (sprite == null) return;

			Vector2 drawoffset = Vector2.Zero;
			SpriteEffects flip = GetDrawFlip();

			switch (CurrentFacing)
			{
				case Facing.Right:
					drawoffset = (Vector2)element.Offset;
					break;

				case Facing.Left:
					drawoffset = new Vector2(-element.Offset.X, element.Offset.Y);
					break;
			}

			if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) drawoffset.X = -drawoffset.X;
			if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) drawoffset.Y = -drawoffset.Y;

			Video.DrawState drawstate = SpriteManager.DrawState;

			Vector2 spritelocation = Video.Renderer.GetDrawLocation(sprite.Size, location, (Vector2)sprite.Axis - drawoffset, CurrentScale, Vector2.One, flip);
			drawstate.Reset();
			drawstate.Mode = DrawMode.OutlinedRectangle;
			drawstate.AddData(spritelocation, new Rectangle(0, 0, (Int32)(sprite.Size.X * CurrentScale.X), (Int32)(sprite.Size.Y * CurrentScale.Y)), Color.Gray);
			drawstate.Use();
		}

		void DrawClsnBoxes(Vector2 location, Animations.AnimationElement element)
		{
			if (element == null) throw new ArgumentNullException("element");

			Video.DrawState drawstate = SpriteManager.DrawState;
			drawstate.Reset();

			drawstate.Mode = DrawMode.OutlinedRectangle;

			foreach (Animations.Clsn clsn in element)
			{
				Rectangle rect = clsn.MakeRect(location, CurrentScale, CurrentFacing);

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

		void DrawOriginCross(Vector2 location, Int32 segmentlength)
		{
			Video.DrawState drawstate = SpriteManager.DrawState;
			drawstate.Reset();

			drawstate.Mode = DrawMode.Lines;
			drawstate.AddData(location - new Vector2(segmentlength, 0), null, Color.Black);
			drawstate.AddData(location + new Vector2(segmentlength, 0), null, Color.Black);
			drawstate.AddData(location - new Vector2(0, segmentlength), null, Color.Black);
			drawstate.AddData(location + new Vector2(0, segmentlength), null, Color.Black);
			drawstate.Use();
		}

		public abstract Boolean RemoveCheck();

		public abstract Vector2 GetDrawLocation();

		public virtual SpriteEffects GetDrawFlip()
		{
			Animations.AnimationElement currentelement = AnimationManager.CurrentElement;

			SpriteEffects flip = CurrentFlip ^ currentelement.Flip;

			if (CurrentFacing == Facing.Left) flip ^= SpriteEffects.FlipHorizontally;

			return flip;
		}

		public abstract Drawing.SpriteManager SpriteManager { get; }

		public abstract Animations.AnimationManager AnimationManager { get; }

		public abstract EntityUpdateOrder UpdateOrder { get; }

		public Int32 DrawOrder
		{
			get { return m_draworder; }

			set { m_draworder = value; }
		}

		public Vector2 CurrentLocation
		{
			get { return m_location; }

			set { m_location = value; }
		}

		public Vector2 CurrentVelocity
		{
			get { return m_velocity; }

			set { m_velocity = value; }
		}

		public Vector2 CurrentAcceleration
		{
			get { return m_acceleration; }

			set { m_acceleration = value; }
		}

		public Facing CurrentFacing
		{
			get { return m_facing; }

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
			get { return m_flip; }

			set { m_flip = value; }
		}

		public Vector2 CurrentScale
		{
			get { return m_scale; }

			set { m_scale = value; }
		}

		public Blending Transparency
		{
			get { return m_blending; }

			set { m_blending = value; }
		}

		public AfterImage AfterImages
		{
			get { return m_afterimages; }
		}

		public Single DrawingAngle
		{
			get { return m_drawingangle; }

			set { m_drawingangle = value; }
		}

		public Boolean AngleDraw
		{
			get { return m_angledraw; }

			set { m_angledraw = value; }
		}

		public abstract PaletteFx PaletteFx { get; }

		public abstract Team Team { get; }

		public abstract Player BasePlayer { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_acceleration;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Facing m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AfterImage m_afterimages;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_draworder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_drawingangle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_angledraw;

		#endregion
	}
}