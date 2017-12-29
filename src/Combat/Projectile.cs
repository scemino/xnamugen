using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	internal class Projectile : Entity
	{
		public Projectile(FightEngine fightengine, Character creator, ProjectileData data)
			: base(fightengine)
		{
			if (creator == null) throw new ArgumentNullException(nameof(creator));
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_creator = creator;
			m_offsetcharacter = data.PositionType == PositionType.P2 ? creator.GetOpponent() : creator;
			m_data = data;
			m_animationmanager = Creator.AnimationManager.Clone();
			m_spritemanager = Creator.SpriteManager.Clone();
			m_gameticks = 0;
			m_hitcountdown = 0;
			m_state = ProjectileState.Normal;
			m_totalhits = 0;
			m_hitpausecountdown = 0;
			m_currentpriority = Data.Priority;
			m_palfx = new PaletteFx();

			CurrentPalette = Creator.CurrentPalette;
			CurrentFacing = Creator.CurrentFacing;
			CurrentLocation = GetStartLocation();
			CurrentVelocity = Data.InitialVelocity;
			CurrentAcceleration = Data.Acceleration;
			CurrentFlip = SpriteEffects.None;
			CurrentScale = Data.Scale;
			DrawOrder = Data.SpritePriority;

			SetLocalAnimation(Data.AnimationNumber, 0);
		}

		private Vector2 GetStartLocation()
		{
			var camerabounds = Engine.Camera.ScreenBounds;
			var facing = m_offsetcharacter.CurrentFacing;
			Vector2 location;

			switch (m_data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
					return Misc.GetOffset(m_offsetcharacter.CurrentLocation, facing, Data.CreationOffset);

				case PositionType.Left:
					return Misc.GetOffset(new Vector2(camerabounds.Left, 0), Facing.Right, Data.CreationOffset);

				case PositionType.Right:
					return Misc.GetOffset(new Vector2(camerabounds.Right, 0), Facing.Right, Data.CreationOffset);

				case PositionType.Back:
					location = Misc.GetOffset(Vector2.Zero, facing, Data.CreationOffset);
					location.X += facing == Facing.Right ? camerabounds.Left : camerabounds.Right;
					return location;

				case PositionType.Front:
					location = Misc.GetOffset(Vector2.Zero, facing, Data.CreationOffset);
					location.X += facing == Facing.Left ? camerabounds.Left : camerabounds.Right;
					return location;

				default:
					throw new ArgumentOutOfRangeException("postype");
			}
		}

		public override Vector2 GetDrawLocation()
		{
			return CurrentLocation + new Vector2(Mugen.ScreenSize.X / 2, Engine.Stage.ZOffset);
		}

		public override bool RemoveCheck()
		{
			return m_state == ProjectileState.Kill;
		}

		public override void UpdatePhsyics()
		{
			if (m_hitpausecountdown == 0)
			{
				base.UpdatePhsyics();

				CurrentVelocity *= Data.VelocityMultiplier;
			}
		}

		public override void UpdateState()
		{
			base.UpdateState();

			if (m_gameticks == 0)
			{
				CurrentLocation = GetStartLocation();
			}

			++m_gameticks;

			if (m_hitpausecountdown > 0)
			{
				--m_hitpausecountdown;
			}
			else if (m_hitcountdown > 0)
			{
				--m_hitcountdown;
			}

			if (m_state == ProjectileState.Canceling || m_state == ProjectileState.Removing)
			{
				if (AnimationManager.IsAnimationFinished) m_state = ProjectileState.Kill;
				return;
			}

			var camera_rect = Engine.Camera.ScreenBounds;
			var drawlocation = GetDrawLocation();
			var stage = Engine.Stage;

			if (Data.RemoveTimeout != -1 && m_gameticks != 1 && m_gameticks >= Data.RemoveTimeout) StartRemoval();

			if (CurrentLocation.X < camera_rect.Left - Data.StageEdgeBound) StartRemoval();
			if (CurrentLocation.X > camera_rect.Right + Data.StageEdgeBound) StartRemoval();

			if (CurrentLocation.X < camera_rect.Left - Data.ScreenEdgeBound) StartRemoval();
			if (CurrentLocation.X > camera_rect.Right + Data.ScreenEdgeBound) StartRemoval();

#warning This could be an issue.
			//if (CurrentLocation.Y < Data.HeightLowerBound) StartRemoval();
			//if (CurrentLocation.Y > Data.HeightUpperBound) StartRemoval();

			if (Data.RemoveOnHit && m_totalhits >= Data.HitsBeforeRemoval) StartHitRemoval();
		}

		private Facing GetStartFacing()
		{
			var facing = Facing.Left;

			switch (Data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
				case PositionType.Back:
					facing = m_offsetcharacter.CurrentFacing;
					break;

				case PositionType.Front:
				case PositionType.Left:
				case PositionType.Right:
					facing = Facing.Right;
					break;

				default:
					throw new ArgumentOutOfRangeException("Data.PositionType");
			}

			return facing;
		}

		public void StartHitRemoval()
		{
			if (m_state == ProjectileState.Removing) return;

			if (AnimationManager.HasAnimation(Data.HitAnimationNumber) && AnimationManager.CurrentAnimation.Number != Data.HitAnimationNumber)
			{
				m_state = ProjectileState.Removing;
				SetLocalAnimation(Data.HitAnimationNumber, 0);
				CurrentVelocity = Data.RemoveVelocity;
			}
			else
			{
				m_state = ProjectileState.Kill;
			}
		}

		public void StartRemoval()
		{
			if (m_state == ProjectileState.Removing) return;

			if (AnimationManager.HasAnimation(Data.RemoveAnimationNumber) && AnimationManager.CurrentAnimation.Number != Data.RemoveAnimationNumber)
			{
				m_state = ProjectileState.Removing;
				SetLocalAnimation(Data.RemoveAnimationNumber, 0);
				CurrentVelocity = Data.RemoveVelocity;
			}
			else
			{
				m_state = ProjectileState.Kill;
			}
		}

		public void StartCanceling()
		{
			if (m_state == ProjectileState.Canceling) return;

			if (AnimationManager.HasAnimation(Data.CancelAnimationNumber) && AnimationManager.CurrentAnimation.Number != Data.CancelAnimationNumber)
			{
				m_state = ProjectileState.Canceling;
				SetLocalAnimation(Data.CancelAnimationNumber, 0);
				BasePlayer.OffensiveInfo.ProjectileInfo.Set(Data.ProjectileId, ProjectileDataType.Cancel);
			}
			else
			{
				m_state = ProjectileState.Kill;
			}
		}

		public bool CanAttack()
		{
			if (State != ProjectileState.Normal) return false;
			if (Data.HitDef == null) return false;
			if (HitPauseCountdown > 0) return false;
			if (HitCountdown > 0) return false;
			if (TotalHits >= Data.HitsBeforeRemoval) return false;

			return true;
		}

		public override Drawing.SpriteManager SpriteManager => m_spritemanager;

		public override Animations.AnimationManager AnimationManager => m_animationmanager;

		public override EntityUpdateOrder UpdateOrder => EntityUpdateOrder.Projectile;

		public Character Creator => m_creator;

		public ProjectileData Data => m_data;

		public override PaletteFx PaletteFx => m_palfx;

		public override Team Team => BasePlayer.Team;

		public override Player BasePlayer => Creator.BasePlayer;

		public int TotalHits
		{
			get => m_totalhits;

			set { m_totalhits = value; }
		}

		public int HitCountdown
		{
			get => m_hitcountdown;

			set { m_hitcountdown = value; }
		}

		public int Priority
		{
			get => m_currentpriority;

			set { m_currentpriority = value; }
		}

		public ProjectileState State
		{
			get => m_state;

			set { m_state = value; }
		}

		public int HitPauseCountdown
		{
			get => m_hitpausecountdown;

			set { m_hitpausecountdown = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_creator;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_offsetcharacter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ProjectileData m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_gameticks;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_hitcountdown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_totalhits;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_currentpriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ProjectileState m_state;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PaletteFx m_palfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_hitpausecountdown;

		#endregion
	}
}