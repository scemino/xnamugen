using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Combat
{
	abstract class Character : Entity
	{
		protected Character(FightEngine engine)
			: base(engine)
		{
			m_statetype = StateType.Standing;
			m_playercontrol = PlayerControl.InControl;
			m_movetype = MoveType.Idle;
			m_physics = Physics.Standing;
			m_life = 0;
			m_id = Engine.GenerateCharacterId();
			m_drawoffset = new Vector2(0, 0);
			m_positionfreeze = false;
			m_roundsexisted = 0;
			m_bind = new CharacterBind(this);
			m_assertions = new CharacterAssertions();
			m_jugglepoints = 0;
			m_variables = new CharacterVariables();
			m_clipboard = new StringBuilder();
			m_buttonarray = new Commands.ButtonArray();
			m_pushflag = false;
			m_drawscale = Vector2.One;
			m_offensiveinfo = new OffensiveInfo(this);
			m_defensiveinfo = new DefensiveInfo(this);
			m_updatedanimation = false;
		}

		public override void Reset()
		{
			base.Reset();

			m_statetype = StateType.Standing;
			m_playercontrol = PlayerControl.InControl;
			m_movetype = MoveType.Idle;
			m_physics = Physics.Standing;
			m_life = 0;
			m_drawoffset = new Vector2(0, 0);
			m_positionfreeze = false;
			m_roundsexisted = 0;
			m_bind.Reset();
			m_assertions.Reset();
			m_jugglepoints = 0;
			m_variables.Reset();
			m_clipboard.Length = 0;
			m_buttonarray = new Commands.ButtonArray();
			m_pushflag = false;
			m_drawscale = Vector2.One;
			m_offensiveinfo.Reset();
			m_defensiveinfo.Reset();
			m_updatedanimation = false;
		}

		public override void Draw()
		{
			if (Assertions.Invisible == false)
			{
				base.Draw();
			}
			else
			{
				AfterImages.Draw();
			}
		}

		public override void DebugDraw()
		{
			base.DebugDraw();

			Video.DrawState drawstate = SpriteManager.DrawState;

			Vector2 offset = new Vector2(Mugen.ScreenSize.X / 2.0f, Engine.Stage.ZOffset);

			drawstate.Reset();
			drawstate.Mode = DrawMode.Lines;

			drawstate.AddData(new Vector2(GetBackLocation(), CurrentLocation.Y) + offset, null, new Color(255, 0, 255, 255));
			drawstate.AddData(CurrentLocation + offset, null, new Color(255, 0, 255, 255));

			drawstate.AddData(new Vector2(GetFrontLocation(), CurrentLocation.Y) + offset, null, new Color(255, 255, 0, 255));
			drawstate.AddData(CurrentLocation + offset, null, new Color(255, 255, 0, 255));

			drawstate.Use();
		}

		public override void CleanUp()
		{
			if (OffensiveInfo.HitPauseTime > 1)
			{
				InHitPause = true;
				--OffensiveInfo.HitPauseTime;
			}
			else
			{
				InHitPause = false;
				OffensiveInfo.HitPauseTime = 0;
			}

			if (InHitPause == false)
			{
				base.CleanUp();

				Transparency = new Blending();
				CameraFollowX = true;
				CameraFollowY = true;
				ScreenBound = true;
				PositionFreeze = false;
				Assertions.Reset();
				Dimensions.ClearOverride();
				DrawOffset = Vector2.Zero;
				PushFlag = false;
				DrawScale = Vector2.One;
				m_updatedanimation = false;

				DoFriction();
			}
		}

		public override void UpdateAnimations()
		{
			if (InHitPause == false)
			{
				base.UpdateAnimations();
				m_updatedanimation = true;
			}
			else
			{
				m_updatedanimation = false;
			}
		}

        public override void UpdateAfterImages()
        {
            if (InHitPause == false)
            {
                base.UpdateAfterImages();
            }
        }

		public override void UpdateState()
		{
			Bind.Update();

			StateManager.Run(InHitPause);

			if (InHitPause == false)
			{
				OffensiveInfo.Update();
				DefensiveInfo.Update();
			}
		}

		public override void UpdatePhsyics()
		{
			if (InHitPause == true) return;

			CurrentVelocity += CurrentAcceleration;

			if (PositionFreeze == false)
			{
				Move(CurrentVelocity);
			}

			switch (Physics)
			{
				case Physics.Standing:
					if (CurrentLocation.Y >= 0) CurrentLocation = new Vector2(CurrentLocation.X, 0);
					break;

				case Physics.Crouching:
					if (CurrentLocation.Y >= 0) CurrentLocation = new Vector2(CurrentLocation.X, 0);
					break;
			}

			HandleTurning();
			HandlePushing();
		}

		public override void UpdateInput()
		{
			CommandManager.Update(m_buttonarray, CurrentFacing, InHitPause);
		}

		public virtual void RecieveInput(PlayerButton button, Boolean pressed)
		{
			m_buttonarray[button] = pressed;
		}

		void DoFriction()
		{
			switch (Physics)
			{
				case Physics.Standing:
					CurrentVelocity *= new Vector2(BasePlayer.Constants.Standfriction, 1);
					ZeroCheckVelocity();
					break;

				case Physics.Crouching:
					CurrentVelocity *= new Vector2(BasePlayer.Constants.Crouchfriction, 1);
					ZeroCheckVelocity();
					break;

				case Physics.Airborne:
					CurrentVelocity += new Vector2(0, BasePlayer.Constants.Vert_acceleration);
					break;
			}
		}

		void ZeroCheckVelocity()
		{
			Vector2 velocity = CurrentVelocity;

			velocity.X = (velocity.X > -1.0f && velocity.X < 1.0f) ? 0 : velocity.X;

			CurrentVelocity = velocity;
		}

		protected virtual Boolean HandleTurning()
		{
			if (PlayerControl == PlayerControl.NoControl) return false;
			//if (CurrentLocation.Y != 0 || PlayerControl == PlayerControl.NoControl) return false;

			Player closest = GetOpponent();
			if (closest == null) return false;
			if (CurrentFacing == Facing.Right && CurrentLocation.X <= closest.CurrentLocation.X) return false;
			if (CurrentFacing == Facing.Left && CurrentLocation.X >= closest.CurrentLocation.X) return false;

			if (StateManager.CurrentState.Number == StateMachine.StateNumber.Standing && AnimationManager.CurrentAnimation.Number != 5 && Assertions.NoAutoTurn == false)
			{
				CurrentFacing = Misc.FlipFacing(CurrentFacing);
				SetLocalAnimation(5, 0);
				return true;
			}

			if (StateManager.CurrentState.Number == StateMachine.StateNumber.Walking && AnimationManager.CurrentAnimation.Number != 5 && Assertions.NoAutoTurn == false)
			{
				CurrentFacing = Misc.FlipFacing(CurrentFacing);
				SetLocalAnimation(5, 0);
				return true;
			}

			if (StateManager.CurrentState.Number == StateMachine.StateNumber.Crouching && AnimationManager.CurrentAnimation.Number != 6 && Assertions.NoAutoTurn == false)
			{
				CurrentFacing = Misc.FlipFacing(CurrentFacing);
				SetLocalAnimation(6, 0);
				return true;
			}

			return false;
		}

		protected virtual void HandlePushing()
		{
			if (PushFlag == false) return;

			foreach (Entity entity in Engine.Entities)
			{
				Character c = FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (c == null) continue;

				if (c.PushFlag == false) continue;

				if (Collision.HasCollision(this, ClsnType.Type2Normal, c, ClsnType.Type2Normal) == false) continue;

				if (CurrentLocation.X >= c.CurrentLocation.X)
				{
					Single lhs_pos = GetLeftLocation();
					Single rhs_pos = c.GetRightLocation();

					Single overlap = rhs_pos - lhs_pos;

					if (overlap > 0)
					{
						Vector2 actualpush = c.MoveLeft(new Vector2(overlap, 0));

						if (actualpush.X != -overlap)
						{
							Single reversepush = overlap - actualpush.X;
							MoveRight(new Vector2(reversepush, 0));
						}
					}
					else if (overlap < 0)
					{
					}
				}
				else
				{
					Single lhs_pos = GetRightLocation();
					Single rhs_pos = c.GetLeftLocation();

					Single overlap = lhs_pos - rhs_pos;

					if (overlap > 0)
					{
						Vector2 actualpush = c.MoveRight(new Vector2(overlap, 0));

						if (actualpush.X != overlap)
						{
							Single reversepush = overlap - actualpush.X;
							MoveLeft(new Vector2(reversepush, 0));
						}
					}
				}
			}
		}

		public override Vector2 Move(Vector2 p)
		{
			return base.Move(p);

			/*
			if (PositionFreeze == false)
			{
				return base.Move(p);
			}

			return new Vector2();
			*/
		}

		protected override void Bounding()
		{
			base.Bounding();

			//CurrentLocation = FightEngine.Stage.PlayerBounds.Bound(CurrentLocation);

			if (ScreenBound == true)
			{
				Rectangle screenrect = Engine.Camera.ScreenBounds;

				Single newleft = screenrect.Left - GetLeftEdgePosition(true);
				Single newright = screenrect.Right - GetRightEdgePosition(true);

				if (GetLeftEdgePosition(true) < screenrect.Left)
				{
					CurrentLocation += new Vector2(newleft, 0);
				}

				if (GetRightEdgePosition(true) > screenrect.Right)
				{
					CurrentLocation += new Vector2(newright, 0);
				}
			}
		}

		public override Vector2 GetDrawLocation()
		{
			return CurrentLocation + Misc.GetOffset(Vector2.Zero, CurrentFacing, DrawOffset + BasePlayer.Constants.DrawOffset) + new Vector2(Mugen.ScreenSize.X / 2, Engine.Stage.ZOffset);
		}

		public Player GetOpponent()
		{
			foreach (Entity entity in Engine.Entities)
			{
				Player player = entity as Player;
				if (player == null) continue;

				if (Team != player.Team) return player;
			}

			return null;
		}

		public Single GetLeftLocation()
		{
			if (CurrentFacing == Facing.Right)
			{
				return GetBackLocation();
			}
			else
			{
				return GetFrontLocation();
			}
		}

		public Single GetRightLocation()
		{
			if (CurrentFacing == Facing.Right)
			{
				return GetFrontLocation();
			}
			else
			{
				return GetBackLocation();
			}
		}

		public Single GetFrontLocation()
		{
			if (CurrentFacing == Facing.Right)
			{
				return CurrentLocation.X + Dimensions.GetFrontWidth(StateType);
			}
			else
			{

				return CurrentLocation.X - Dimensions.GetFrontWidth(StateType);
			}
		}

		public Single GetBackLocation()
		{
			if (CurrentFacing == Facing.Right)
			{

				return CurrentLocation.X - Dimensions.GetBackWidth(StateType);
			}
			else
			{

				return CurrentLocation.X + Dimensions.GetBackWidth(StateType);
			}
		}

		public Int32 GetLeftEdgePosition(Boolean body)
		{
			if (CurrentFacing == Facing.Left)
			{
				Int32 position = (Int32)CurrentLocation.X - Engine.Stage.LeftEdgeDistance;
				if (body == true) position -= Dimensions.FrontEdgeWidth;

				return position;
			}

			if (CurrentFacing == Facing.Right)
			{
				Int32 position = (Int32)CurrentLocation.X - Engine.Stage.LeftEdgeDistance;
				if (body == true) position -= Dimensions.BackEdgeWidth;

				return position;
			}

			return 0;
		}

		public Int32 GetRightEdgePosition(Boolean body)
		{
			if (CurrentFacing == Facing.Left)
			{
				Int32 position = (Int32)CurrentLocation.X + Engine.Stage.RightEdgeDistance;
				if (body == true) position += Dimensions.BackEdgeWidth;

				return position;
			}

			if (CurrentFacing == Facing.Right)
			{
				Int32 position = (Int32)CurrentLocation.X + Engine.Stage.RightEdgeDistance;
				if (body == true) position += Dimensions.FrontEdgeWidth;

				return position;
			}

			return 0;
		}

		public Character FilterEntityAsCharacter(Entity entity, AffectTeam team)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			Character c = entity as Character;
			if (c == null || c == this) return null;

			if (((team & AffectTeam.Enemy) != AffectTeam.Enemy) && (Team != c.Team)) return null;
			if (((team & AffectTeam.Friendly) != AffectTeam.Friendly) && (Team == c.Team)) return null;

			return c;
		}

		public Character FilterEntityAsTarget(Entity entity, Int32? targetid)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			Character c = entity as Character;
			if (c == null || c == this) return null;

			//if (c.AttackInfo.HitTime == 0) return null; 

			//if (c.MoveType != MoveType.BeingHit) return null;
			//if (c.AttackInfo.Attacker != this) return null;

			if (OffensiveInfo.TargetList.Contains(c) == false) return null;
			if (targetid != null && targetid.Value >= 0 && c.DefensiveInfo.HitDef.TargetId != targetid.Value) return null;

			return c;
		}

		public Explod FilterEntityAsExplod(Entity entity, Int32? explodid)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			Explod explod = entity as Explod;
			if (explod == null || explod.Creator != this || (explodid != null && explodid.Value >= 0 && explod.Data.Id != explodid.Value)) return null;

			return explod;
		}

		public Helper FilterEntityAsHelper(Entity entity, Int32? helperid)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			Helper helper = entity as Helper;
			if (helper == null || helper.BasePlayer != this.BasePlayer || (helperid != null && helperid.Value > 0 && helper.Data.HelperId != helperid.Value)) return null;

			return helper;
		}

		public Projectile FilterEntityAsProjectile(Entity entity, Int32? projid)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			Projectile proj = entity as Projectile;
			if (proj == null || proj.BasePlayer != this || (projid != null && projid.Value > 0 && proj.Data.ProjectileId != projid.Value)) return null;

			return proj;
		}

		public Character FilterEntityAsPartner(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			if (entity == this) return null;
			if (entity == this.BasePlayer) return null;
			if (entity.Team != Team) return null;

			if (entity is Helper)
			{
				Helper helper = entity as Helper;
				return helper.Data.Type == HelperType.Player ? helper : null;
			}

			if (entity is Player)
			{
				return entity as Player;
			}

			return null;
		}

		void RemoveFromOthersTargetLists()
		{
			foreach (Entity entity in Engine.Entities)
			{
				Character character = entity as Character;
				if (character == null) continue;

				character.OffensiveInfo.TargetList.Remove(this);
			}
		}

		public abstract Audio.SoundManager SoundManager { get; }

		public abstract Commands.CommandManager CommandManager { get; }

		public abstract StateMachine.StateManager StateManager { get; }

		public abstract CharacterDimensions Dimensions { get; }

		public override EntityUpdateOrder UpdateOrder
		{
			get { return EntityUpdateOrder.Character; }
		}

		public Physics Physics
		{
			get { return m_physics; }

			set { m_physics = value; }
		}

		public MoveType MoveType
		{
			get { return m_movetype; }

			set
			{
				if (value == MoveType.Unchanged) throw new ArgumentOutOfRangeException("value");

				if (m_movetype == MoveType.BeingHit && value != MoveType.BeingHit) RemoveFromOthersTargetLists();

				m_movetype = value;
			}
		}

		public PlayerControl PlayerControl
		{
			get { return m_playercontrol; }

			set
			{

				if (value != PlayerControl.InControl && value != PlayerControl.NoControl) throw new ArgumentException("Value must be InControl or NoControl", "value");
				m_playercontrol = value;
			}
		}

		public StateType StateType
		{
			get { return m_statetype; }

			set 
			{
				if (value == StateType.None || value == StateType.Unchanged) throw new ArgumentOutOfRangeException("value");
				m_statetype = value; 
			}
		}

		public Int32 Life
		{
			get { return m_life; }

			[DebuggerStepThrough]
			set
			{
				m_life = Misc.Clamp(value, 0, BasePlayer.Constants.MaximumLife);
			}
		}

		public Int32 Id
		{
			get { return m_id; }

			set { m_id = value; }
		}

		public Vector2 DrawOffset
		{
			get { return m_drawoffset; }

			set { m_drawoffset = value; }
		}

		public Boolean PositionFreeze
		{
			get { return m_positionfreeze; }

			set { m_positionfreeze = value; }
		}

		public Int32 RoundsExisted
		{
			get { return m_roundsexisted; }

			set { m_roundsexisted = value; }
		}

		public Boolean CameraFollowX
		{
			get { return m_camerafollowx; }

			set { m_camerafollowx = value; }
		}

		public Boolean CameraFollowY
		{
			get { return m_camerafollowy; }

			set { m_camerafollowy = value; }
		}

		public Boolean ScreenBound
		{
			get { return m_screenbound; }

			set { m_screenbound = value; }
		}

		public Boolean InHitPause
		{
			get { return m_inhitpause; }

			set { m_inhitpause = value; }
		}

		public CharacterBind Bind
		{
			get { return m_bind; }
		}

		public CharacterAssertions Assertions
		{
			get { return m_assertions; }
		}

		public Int32 JugglePoints
		{
			get { return m_jugglepoints; }

			set { m_jugglepoints = value; }
		}

		public CharacterVariables Variables
		{
			get { return m_variables; }
		}

		public StringBuilder Clipboard
		{
			get { return m_clipboard; }
		}

		public Boolean PushFlag
		{
			get { return m_pushflag; }

			set { m_pushflag = value; }
		}

		public Vector2 DrawScale
		{
			get { return m_drawscale; }

			set { m_drawscale = value; }
		}

		public OffensiveInfo OffensiveInfo
		{
			get { return m_offensiveinfo; }
		}

		public DefensiveInfo DefensiveInfo
		{
			get { return m_defensiveinfo; }
		}

		public Boolean UpdatedAnimation
		{
			get { return m_updatedanimation; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Commands.ButtonArray m_buttonarray;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Physics m_physics;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		MoveType m_movetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		PlayerControl m_playercontrol;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		StateType m_statetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_drawoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_positionfreeze;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_roundsexisted;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_camerafollowx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_camerafollowy;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_screenbound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_inhitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CharacterBind m_bind;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CharacterAssertions m_assertions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_jugglepoints;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CharacterVariables m_variables;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StringBuilder m_clipboard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_pushflag;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_drawscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly OffensiveInfo m_offensiveinfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly DefensiveInfo m_defensiveinfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_updatedanimation;

		#endregion
	}
}