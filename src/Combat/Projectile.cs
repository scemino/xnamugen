using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
    class Projectile : Entity
    {
        public Projectile(FightEngine fightengine, Character creator, ProjectileData data)
            : base(fightengine)
        {
            if (creator == null) throw new ArgumentNullException("creator");
            if (data == null) throw new ArgumentNullException("data");

            m_creator = creator;
            m_offsetcharacter = (data.PositionType == PositionType.P2) ? creator.GetOpponent() : creator;
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

            CurrentFacing = Creator.CurrentFacing;
            CurrentLocation = GetStartLocation();
            CurrentVelocity = Data.InitialVelocity;
            CurrentAcceleration = Data.Acceleration;
            CurrentFlip = SpriteEffects.None;
            CurrentScale = Data.Scale;
            DrawOrder = Data.SpritePriority;

            SetLocalAnimation(Data.AnimationNumber, 0);
        }

        Vector2 GetStartLocation()
        {
            Rectangle camerabounds = Engine.Camera.ScreenBounds;
            Facing facing = m_offsetcharacter.CurrentFacing;
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
                    location.X += (facing == Facing.Right) ? camerabounds.Left : camerabounds.Right;
                    return location;

                case PositionType.Front:
                    location = Misc.GetOffset(Vector2.Zero, facing, Data.CreationOffset);
                    location.X += (facing == Facing.Left) ? camerabounds.Left : camerabounds.Right;
                    return location;

                default:
                    throw new ArgumentOutOfRangeException("postype");
            }
        }

        public override Vector2 GetDrawLocation()
        {
            return CurrentLocation + new Vector2(Mugen.ScreenSize.X / 2, Engine.Stage.ZOffset);
        }

        public override Boolean RemoveCheck()
        {
            return m_state == ProjectileState.Kill;
        }

        public override void UpdatePhsyics()
        {
            if (m_hitpausecountdown == 0)
            {
                base.UpdatePhsyics();

                if (State == ProjectileState.Normal) CurrentVelocity *= Data.VelocityMultiplier;
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
                if (AnimationManager.IsAnimationFinished == true) m_state = ProjectileState.Kill;
                return;
            }

            if (Data.RemoveTimeout != -1 && m_gameticks != 1 && m_gameticks >= Data.RemoveTimeout) StartRemoval();

            if (BoundCheck() == false) StartRemoval();

            if (Data.RemoveOnHit == true && m_totalhits >= Data.HitsBeforeRemoval) StartHitRemoval();
        }

		public override Vector4 GetShadowColor()
		{
			if (Engine.Stage.ShadowFade == null)
			{
				return Data.ShadowColor;
			}
			else
			{
				if (CurrentLocation.Y <= Engine.Stage.ShadowFade.Value.X)
				{
					return Vector4.Zero;
				}
				else if (CurrentLocation.Y >= Engine.Stage.ShadowFade.Value.Y)
				{
					return Data.ShadowColor;
				}
				else
				{
					Single bottom = Engine.Stage.ShadowFade.Value.X;
					Single top = Engine.Stage.ShadowFade.Value.Y;
					Single percentage = (CurrentLocation.Y - bottom) / (top - bottom);

					return Data.ShadowColor * percentage;
				}
			}
		}

        Boolean BoundCheck()
        {
            Rectangle camera_rect = Engine.Camera.ScreenBounds;

            if (CurrentLocation.X < camera_rect.Left - Data.StageEdgeBound) return false;
            if (CurrentLocation.X > camera_rect.Right + Data.StageEdgeBound) return false;

            if (CurrentLocation.X < camera_rect.Left - Data.ScreenEdgeBound) return false;
            if (CurrentLocation.X > camera_rect.Right + Data.ScreenEdgeBound) return false;

            if (CurrentLocation.Y < Data.HeightLowerBound && CurrentVelocity.Y < 0) return false;
            if (CurrentLocation.Y > Data.HeightUpperBound && CurrentVelocity.Y > 0) return false;

            return true;
        }

        Facing GetStartFacing()
        {
            Facing facing = Facing.Left;

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

                case PositionType.None:
                default:
                    throw new ArgumentOutOfRangeException("Data.PositionType");
            }

            return facing;
        }

        public void StartHitRemoval()
        {
            if (m_state == ProjectileState.Removing) return;

            if (AnimationManager.HasAnimation(Data.HitAnimationNumber) == true && AnimationManager.CurrentAnimation.Number != Data.HitAnimationNumber)
            {
                m_state = ProjectileState.Removing;
                SetLocalAnimation(Data.HitAnimationNumber, 0);

                UpdatePhsyics();
                CurrentVelocity = Data.RemoveVelocity;
                CurrentAcceleration = Vector2.Zero;
            }
            else
            {
                m_state = ProjectileState.Kill;
            }
        }

        public void StartRemoval()
        {
            if (m_state == ProjectileState.Removing) return;

            if (AnimationManager.HasAnimation(Data.RemoveAnimationNumber) == true && AnimationManager.CurrentAnimation.Number != Data.RemoveAnimationNumber)
            {
                m_state = ProjectileState.Removing;
                SetLocalAnimation(Data.RemoveAnimationNumber, 0);

                UpdatePhsyics();
                CurrentVelocity = Data.RemoveVelocity;
                CurrentAcceleration = Vector2.Zero;
            }
            else
            {
                m_state = ProjectileState.Kill;
            }
        }

        public void StartCanceling()
        {
            if (m_state == ProjectileState.Canceling) return;

            if (AnimationManager.HasAnimation(Data.CancelAnimationNumber) == true && AnimationManager.CurrentAnimation.Number != Data.CancelAnimationNumber)
            {
                m_state = ProjectileState.Canceling;
                SetLocalAnimation(Data.CancelAnimationNumber, 0);
                BasePlayer.OffensiveInfo.ProjectileInfo.Set(Data.ProjectileId, ProjectileDataType.Cancel);

                UpdatePhsyics();
                CurrentVelocity = Data.RemoveVelocity;
                CurrentAcceleration = Vector2.Zero;
            }
            else
            {
                m_state = ProjectileState.Kill;
            }
        }

        public Boolean CanAttack()
        {
            if (State != ProjectileState.Normal) return false;
            if (Data.HitDef == null) return false;
            if (HitPauseCountdown > 0) return false;
            if (HitCountdown > 0) return false;
            if (TotalHits >= Data.HitsBeforeRemoval) return false;

            return true;
        }

        public override Drawing.SpriteManager SpriteManager
        {
            get { return m_spritemanager; }
        }

        public override Animations.AnimationManager AnimationManager
        {
            get { return m_animationmanager; }
        }

        public override EntityUpdateOrder UpdateOrder
        {
            get { return EntityUpdateOrder.Projectile; }
        }

        public Character Creator
        {
            get { return m_creator; }
        }

        public ProjectileData Data
        {
            get { return m_data; }
        }

        public override PaletteFx PaletteFx
        {
            get { return m_palfx; }
        }

        public override Team Team
        {
            get { return BasePlayer.Team; }
        }

        public override Player BasePlayer
        {
            get { return Creator.BasePlayer; }
        }

        public Int32 TotalHits
        {
            get { return m_totalhits; }

            set { m_totalhits = value; }
        }

        public Int32 HitCountdown
        {
            get { return m_hitcountdown; }

            set { m_hitcountdown = value; }
        }

        public Int32 Priority
        {
            get { return m_currentpriority; }

            set { m_currentpriority = value; }
        }

        public ProjectileState State
        {
            get { return m_state; }

            set { m_state = value; }
        }

        public Int32 HitPauseCountdown
        {
            get { return m_hitpausecountdown; }

            set { m_hitpausecountdown = value; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Character m_creator;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Character m_offsetcharacter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Drawing.SpriteManager m_spritemanager;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Animations.AnimationManager m_animationmanager;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ProjectileData m_data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_gameticks;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_hitcountdown;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_totalhits;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_currentpriority;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ProjectileState m_state;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly PaletteFx m_palfx;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_hitpausecountdown;

        #endregion
    }
}