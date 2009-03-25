using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("Explod Id #{Data.Id} - {Data.CommonAnimation}, {Data.AnimationNumber}")]
	class Explod : Entity
	{
		public Explod(FightEngine fightengine, ExplodData data)
			: base(fightengine)
		{
			if (data == null) throw new ArgumentNullException("data");

			m_data = data;
			m_tickcount = 0;
			m_forceremove = false;

			if (m_data.CommonAnimation == false)
			{
				m_spritemanager = BasePlayer.SpriteManager.Clone();
				m_animationmanager = BasePlayer.AnimationManager.Clone();
			}
			else
			{
				m_spritemanager = Engine.FxSprites.Clone();
				m_animationmanager = Engine.FxAnimations.Clone();
			}

			m_creationfacing = Data.Offseter.CurrentFacing;

			CurrentPalette = Creator.CurrentPalette;
			CurrentFacing = GetStartFacing();
			CurrentLocation = GetStartLocation();
			CurrentVelocity = m_data.Velocity;
			CurrentAcceleration = m_data.Acceleration;
			CurrentFlip = SpriteEffects.None;
			CurrentScale = m_data.Scale;
			DrawOrder = (m_data.DrawOnTop == true) ? 11 : m_data.SpritePriority;
			Transparency = Data.Transparency;

			Random rng = Engine.GetSubSystem<Random>();
			m_random = new Vector2();
			m_random.X += rng.NewInt(-m_data.Random.X, m_data.Random.X);
			m_random.Y += rng.NewInt(-m_data.Random.Y, m_data.Random.Y);

			m_palfx = (m_data.OwnPalFx == true) ? new PaletteFx() : Creator.PaletteFx;

			if (AnimationManager.HasAnimation(Data.AnimationNumber) == true)
			{
				SetLocalAnimation(Data.AnimationNumber, 0);
				m_valid = true;
			}
			else
			{
				m_valid = false;
			}
		}

		Vector2 GetStartLocation()
		{
			Vector2 offset = Data.Location + m_random;
			Vector2 location;
			Rectangle camerabounds = Engine.Camera.ScreenBounds;

			switch (Data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
					return Misc.GetOffset(Data.Offseter.CurrentLocation, m_creationfacing, offset);

				case PositionType.Left:
					return Misc.GetOffset(new Vector2(camerabounds.Left, 0), Facing.Right, offset);

				case PositionType.Right:
					return Misc.GetOffset(new Vector2(camerabounds.Right, 0), Facing.Right, offset);

				case PositionType.Back:
					location = Misc.GetOffset(Vector2.Zero, m_creationfacing, offset);
					location.X += (m_creationfacing == Facing.Right) ? camerabounds.Left : camerabounds.Right;
					return location;

				case PositionType.Front:
					location = Misc.GetOffset(Vector2.Zero, m_creationfacing, offset);
					location.X += (m_creationfacing == Facing.Left) ? camerabounds.Left : camerabounds.Right;
					return location;

				default:
					throw new ArgumentOutOfRangeException("postype");
			}
		}

		Facing GetStartFacing()
		{
			Facing facing = Facing.Left;

			switch (Data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
				case PositionType.Back:
					facing = Data.Offseter.CurrentFacing;
					break;

				case PositionType.Front:
				case PositionType.Left:
				case PositionType.Right:
					facing = Facing.Right;
					break;

				case PositionType.None:
				default:
					throw new ArgumentOutOfRangeException("Data.PositionType");
			}

			if ((Data.Flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) facing = Misc.FlipFacing(facing);

			return facing;
		}

		public override Boolean IsPaused(Pause pause)
		{
			if (pause == null) throw new ArgumentNullException("pause");

			if (pause.IsActive == false) return false;

            if (Data.IsHitSpark == true) return false;

			if (pause.IsSuperPause == true)
			{
				if (Data.SuperMove == true || pause.ElapsedTime <= Data.SuperMoveTime) return false;
			}
			else
			{
				if (pause.ElapsedTime <= Data.PauseTime) return false;
			}

			return true;
		}

		public override Vector2 Move(Vector2 p)
		{
			if (IsBound == true) return new Vector2();

			switch (m_data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
				case PositionType.Front:
				case PositionType.Back:
					return base.Move(p);

				case PositionType.Left:
					return base.Move(new Vector2(-p.X, p.Y));
				//return MoveRight(p);

				case PositionType.Right:
					return base.Move(new Vector2(p.X, p.Y));
				//return MoveLeft(p);

				default:
					return new Vector2();
			}
		}

		public override void UpdatePhsyics()
		{
			if (IsBound == false)
			{
				base.UpdatePhsyics();
			}
		}

		public override void UpdateAnimations()
		{
			if (m_tickcount > 0)
			{
				base.UpdateAnimations(Data.OwnPalFx);
			}
		}

		public override void UpdateState()
		{
			if (m_tickcount == 0)
			{
				CurrentLocation = GetStartLocation();
			}

			++m_tickcount;
		}

		public override Boolean RemoveCheck()
		{
			if (m_forceremove == true)
			{
				return true;
			}
			else if (m_valid == false)
			{
				return true;
			}
			else if (m_data.RemoveTime == -1)
			{
				return false;
			}
			else if (m_data.RemoveTime == -2)
			{
				return AnimationManager.IsAnimationFinished;
			}
			else if (m_data.RemoveTime >= 0)
			{
				return Ticks > m_data.RemoveTime;
			}
			else
			{
				return true;
			}
		}

		public override Vector2 GetDrawLocation()
		{
			Vector2 drawlocation = (IsBound == true) ? GetStartLocation() : CurrentLocation;
			Rectangle screenrect = Engine.Camera.ScreenBounds;

			switch (Data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
					drawlocation += new Vector2(Mugen.ScreenSize.X / 2, Engine.Stage.ZOffset);
					break;

				case PositionType.Front:
				case PositionType.Back:
				case PositionType.Left:
				case PositionType.Right:
					drawlocation += new Vector2(Mugen.ScreenSize.X / 2, screenrect.Top);
					break;

				default:
					throw new InvalidOperationException("Data.PositionType");
			}

			return drawlocation;
		}

		public void Kill()
		{
			m_forceremove = true;
			Engine.Entities.Remove(this);
		}

		public void Modify(ModifyExplodData modifydata)
		{
			if (modifydata == null) throw new ArgumentNullException("data");
			if (modifydata.Id != Data.Id) throw new ArgumentException("data");

			if (modifydata.Scale != null)
			{
				Data.Scale = modifydata.Scale.Value;
				CurrentScale = Data.Scale;
			}

			if (modifydata.SpritePriority != null)
			{
				Data.SpritePriority = modifydata.SpritePriority.Value;
				DrawOrder = Data.SpritePriority;
			}

			if (modifydata.DrawOnTop != null)
			{
				Data.DrawOnTop = modifydata.DrawOnTop.Value;
				if (Data.DrawOnTop == true) DrawOrder = 11;
			}

			if (modifydata.RemoveOnGetHit != null)
			{
				Data.RemoveOnGetHit = modifydata.RemoveOnGetHit.Value;
			}

			if (modifydata.SuperMove != null)
			{
				Data.SuperMove = modifydata.SuperMove.Value;
			}

			if (modifydata.SuperMoveTime != null)
			{
				Data.SuperMoveTime = modifydata.SuperMoveTime.Value;
			}

			if (modifydata.PauseTime != null)
			{
				Data.PauseTime = modifydata.PauseTime.Value;
			}

			if (modifydata.RemoveTime != null)
			{
				Data.RemoveTime = modifydata.RemoveTime.Value;
			}

			if (modifydata.BindTime != null)
			{
				Data.BindTime = modifydata.BindTime.Value;
			}

			if (modifydata.Acceleration != null)
			{
				Data.Acceleration = modifydata.Acceleration.Value;
				CurrentAcceleration = Data.Acceleration;
			}

			if (modifydata.Velocity != null)
			{
				Data.Velocity = modifydata.Velocity.Value;
				CurrentVelocity = Data.Velocity;
			}

			if (modifydata.IgnoreHitPause != null)
			{
				Data.IgnoreHitPause = modifydata.IgnoreHitPause.Value;
			}

			if (modifydata.Flip != null)
			{
				Data.Flip = modifydata.Flip.Value;
			}

			if (modifydata.PositionType != null)
			{
				Data.PositionType = modifydata.PositionType.Value;
			}

			if (modifydata.Location != null)
			{
				Data.Location = modifydata.Location.Value;
			}

			if (modifydata.Random != null)
			{
				Data.Random = modifydata.Random.Value;
			}

			if (modifydata.Transparency != null)
			{
				Data.Transparency = modifydata.Transparency.Value;
				Transparency = Data.Transparency;
			}

			if (modifydata.PositionType != null || modifydata.Location != null)
			{
				CurrentLocation = GetStartLocation();
			}

			if (modifydata.Flip != null)
			{
				CurrentFacing = GetStartFacing();
			}

			/*
			Data.CommonAnimation = data.CommonAnimation;
			Data.AnimationNumber = data.AnimationNumber;
			*/
		}

		public override EntityUpdateOrder UpdateOrder
		{
			get { return EntityUpdateOrder.Explod; }
		}

		public Entity Creator
		{
			get { return Data.Creator; }
		}

		public ExplodData Data
		{
			get { return m_data; }
		}

		public override Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public override Animations.AnimationManager AnimationManager
		{
			get { return m_animationmanager; }
		}

		public override Team Team
		{
			get { return BasePlayer.Team; }
		}

		public override Player BasePlayer
		{
			get { return Creator.BasePlayer; }
		}

		Int32 Ticks
		{
			get { return m_tickcount; }
		}

		Boolean IsBound
		{
			get { return Data.BindTime == -1 || m_tickcount <= Data.BindTime; }
		}

		public Boolean IsValid
		{
			get { return m_valid; }
		}

		public override PaletteFx PaletteFx
		{
			get { return m_palfx; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ExplodData m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_tickcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_valid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PaletteFx m_palfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_random;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_forceremove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Facing m_creationfacing;

		#endregion
	}
}