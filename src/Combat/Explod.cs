using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("Explod Id #{Data.Id} - {Data.CommonAnimation}, {Data.AnimationNumber}")]
	internal class Explod : Entity
	{
		public Explod(FightEngine fightengine, ExplodData data)
			: base(fightengine)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_data = data;
			m_tickcount = 0;
			m_forceremove = false;
			m_creator = data.Creator;

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
			DrawOrder = m_data.DrawOnTop ? 11 : m_data.SpritePriority;
			Transparency = Data.Transparency;

			var rng = Engine.GetSubSystem<Random>();
			m_random = new Vector2();
			m_random.X += rng.NewInt(-m_data.Random.X, m_data.Random.X);
			m_random.Y += rng.NewInt(-m_data.Random.Y, m_data.Random.Y);

			m_palfx = m_data.OwnPalFx ? new PaletteFx() : Creator.PaletteFx;

			if (AnimationManager.HasAnimation(Data.AnimationNumber))
			{
				SetLocalAnimation(Data.AnimationNumber, 0);
				m_valid = true;
			}
			else
			{
				m_valid = false;
			}
		}

		private Vector2 GetStartLocation()
		{
			var offset = Data.Location + m_random;
			Vector2 location;
			var camerabounds = Engine.Camera.ScreenBounds;

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
					location.X += m_creationfacing == Facing.Right ? camerabounds.Left : camerabounds.Right;
					return location;

				case PositionType.Front:
					location = Misc.GetOffset(Vector2.Zero, m_creationfacing, offset);
					location.X += m_creationfacing == Facing.Left ? camerabounds.Left : camerabounds.Right;
					return location;

				default:
					throw new ArgumentOutOfRangeException("postype");
			}
		}

		private Facing GetStartFacing()
		{
			var facing = Facing.Left;

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

				default:
					throw new ArgumentOutOfRangeException("Data.PositionType");
			}

			if ((Data.Flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) facing = Misc.FlipFacing(facing);

			return facing;
		}

		public override bool IsPaused(Pause pause)
		{
			if (pause == null) throw new ArgumentNullException(nameof(pause));

			if (pause.IsActive == false) return false;

            if (Data.IsHitSpark) return false;

			if (pause.IsSuperPause)
			{
				if (Data.SuperMove || pause.ElapsedTime <= Data.SuperMoveTime) return false;
			}
			else
			{
				if (pause.ElapsedTime <= Data.PauseTime) return false;
			}

			return true;
		}

		public override Vector2 Move(Vector2 p)
		{
			if (IsBound) return new Vector2();

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

		public override bool RemoveCheck()
		{
			if (m_forceremove)
			{
				return true;
			}

			if (m_valid == false)
			{
				return true;
			}

			if (m_data.RemoveTime == -1)
			{
				return false;
			}

			if (m_data.RemoveTime == -2)
			{
				return AnimationManager.IsAnimationFinished;
			}

			if (m_data.RemoveTime >= 0)
			{
				return Ticks > m_data.RemoveTime;
			}

			return true;
		}

		public override Vector2 GetDrawLocation()
		{
			var drawlocation = IsBound ? GetStartLocation() : CurrentLocation;
			var screenrect = Engine.Camera.ScreenBounds;

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
				if (Data.DrawOnTop) DrawOrder = 11;
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

		public override EntityUpdateOrder UpdateOrder => EntityUpdateOrder.Explod;

		public Character Creator => m_creator;

		public ExplodData Data => m_data;

		public override Drawing.SpriteManager SpriteManager => m_spritemanager;

		public override Animations.AnimationManager AnimationManager => m_animationmanager;

		public override Team Team => BasePlayer.Team;

		public override Player BasePlayer => Creator.BasePlayer;

		private int Ticks => m_tickcount;

		private bool IsBound => Data.BindTime == -1 || m_tickcount <= Data.BindTime;

		public bool IsValid => m_valid;

		public override PaletteFx PaletteFx => m_palfx;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ExplodData m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_tickcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_valid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PaletteFx m_palfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_random;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_forceremove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Facing m_creationfacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_creator;

		#endregion
	}
}