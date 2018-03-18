using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Combat
{
	internal abstract class Character : Entity
	{
		protected Character(FightEngine engine)
			: base(engine)
		{
			m_statetype = StateType.Standing;
			m_playercontrol = PlayerControl.InControl;
			m_movetype = MoveType.Idle;
			m_physics = Physics.Standing;
			m_life = 0;
			Id = Engine.GenerateCharacterId();
			DrawOffset = new Vector2(0, 0);
			PositionFreeze = false;
			RoundsExisted = 0;
			m_bind = new CharacterBind(this);
			m_assertions = new CharacterAssertions();
			JugglePoints = 0;
			m_variables = new CharacterVariables();
			m_clipboard = new StringBuilder();
			m_currentinput = PlayerButton.None;
			PushFlag = false;
			DrawScale = Vector2.One;
			m_offensiveinfo = new OffensiveInfo(this);
			m_defensiveinfo = new DefensiveInfo(this);
			m_updatedanimation = false;
			m_explods = new Dictionary<int, List<Explod>>();
		}

		public override void Reset()
		{
			base.Reset();

			m_statetype = StateType.Standing;
			m_playercontrol = PlayerControl.InControl;
			m_movetype = MoveType.Idle;
			m_physics = Physics.Standing;
			m_life = 0;
			DrawOffset = new Vector2(0, 0);
			PositionFreeze = false;
			RoundsExisted = 0;
			m_bind.Reset();
			m_assertions.Reset();
			JugglePoints = 0;
			m_variables.Reset();
			m_clipboard.Length = 0;
			m_currentinput = PlayerButton.None;
			PushFlag = false;
			DrawScale = Vector2.One;
			m_offensiveinfo.Reset();
			m_defensiveinfo.Reset();
			m_updatedanimation = false;
			m_explods.Clear();
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

			var drawstate = SpriteManager.DrawState;

			var offset = new Vector2(Mugen.ScreenSize.X / 2.0f, Engine.Stage.ZOffset);

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
			if (InHitPause || DefensiveInfo.HitShakeTime > 0) return;

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
			CommandManager.Update(m_currentinput, CurrentFacing, InHitPause);
		}

		public virtual void RecieveInput(PlayerButton button, bool pressed)
		{
			if (pressed)
			{
				m_currentinput |= button;
			}
			else
			{
				m_currentinput &= ~button;
			}
		}

		private void DoFriction()
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

		private void ZeroCheckVelocity()
		{
			var velocity = CurrentVelocity;

			velocity.X = velocity.X > -1.0f && velocity.X < 1.0f ? 0 : velocity.X;

			CurrentVelocity = velocity;
		}

		protected virtual bool HandleTurning()
		{
			if (PlayerControl == PlayerControl.NoControl) return false;
			//if (CurrentLocation.Y != 0 || PlayerControl == PlayerControl.NoControl) return false;

			var closest = GetOpponent();
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

			foreach (var entity in Engine.Entities)
			{
				var c = FilterEntityAsCharacter(entity, AffectTeam.Enemy);
				if (c == null) continue;

				if (c.PushFlag == false) continue;

				if (Collision.HasCollision(this, ClsnType.Type2Normal, c, ClsnType.Type2Normal) == false) continue;

				if (CurrentLocation.X >= c.CurrentLocation.X)
				{
					var lhs_pos = GetLeftLocation();
					var rhs_pos = c.GetRightLocation();

					var overlap = rhs_pos - lhs_pos;

					if (overlap > 0)
					{
						var actualpush = c.MoveLeft(new Vector2(overlap, 0));

						if (actualpush.X != -overlap)
						{
							var reversepush = overlap - actualpush.X;
							MoveRight(new Vector2(reversepush, 0));
						}
					}
					else if (overlap < 0)
					{
					}
				}
				else
				{
					var lhs_pos = GetRightLocation();
					var rhs_pos = c.GetLeftLocation();

					var overlap = lhs_pos - rhs_pos;

					if (overlap > 0)
					{
						var actualpush = c.MoveRight(new Vector2(overlap, 0));

						if (actualpush.X != overlap)
						{
							var reversepush = overlap - actualpush.X;
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

			if (ScreenBound)
			{
				var screenrect = Engine.Camera.ScreenBounds;

				float newleft = screenrect.Left - GetLeftEdgePosition(true);
				float newright = screenrect.Right - GetRightEdgePosition(true);

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
			return CurrentLocation + Misc.GetOffset(Vector2.Zero, CurrentFacing, DrawOffset) + new Vector2(Mugen.ScreenSize.X / 2, Engine.Stage.ZOffset);
		}

		public Player GetOpponent()
		{
			foreach (var entity in Engine.Entities)
			{
				if (!(entity is Player player)) continue;

				if (Team != player.Team) return player;
			}

			return null;
		}

		public float GetLeftLocation()
		{
			if (CurrentFacing == Facing.Right)
			{
				return GetBackLocation();
			}

			return GetFrontLocation();
		}

		public float GetRightLocation()
		{
			if (CurrentFacing == Facing.Right)
			{
				return GetFrontLocation();
			}

			return GetBackLocation();
		}

		public float GetFrontLocation()
		{
			if (CurrentFacing == Facing.Right)
			{
				return CurrentLocation.X + Dimensions.GetFrontWidth(StateType);
			}

			return CurrentLocation.X - Dimensions.GetFrontWidth(StateType);
		}

		public float GetBackLocation()
		{
			if (CurrentFacing == Facing.Right)
			{

				return CurrentLocation.X - Dimensions.GetBackWidth(StateType);
			}

			return CurrentLocation.X + Dimensions.GetBackWidth(StateType);
		}

		public int GetLeftEdgePosition(bool body)
		{
			if (CurrentFacing == Facing.Left)
			{
				var position = (int)CurrentLocation.X - Engine.Stage.LeftEdgeDistance;
				if (body) position -= Dimensions.FrontEdgeWidth;

				return position;
			}

			if (CurrentFacing == Facing.Right)
			{
				var position = (int)CurrentLocation.X - Engine.Stage.LeftEdgeDistance;
				if (body) position -= Dimensions.BackEdgeWidth;

				return position;
			}

			return 0;
		}

		public int GetRightEdgePosition(bool body)
		{
			if (CurrentFacing == Facing.Left)
			{
				var position = (int)CurrentLocation.X + Engine.Stage.RightEdgeDistance;
				if (body) position += Dimensions.BackEdgeWidth;

				return position;
			}

			if (CurrentFacing == Facing.Right)
			{
				var position = (int)CurrentLocation.X + Engine.Stage.RightEdgeDistance;
				if (body) position += Dimensions.FrontEdgeWidth;

				return position;
			}

			return 0;
		}

		public Character FilterEntityAsCharacter(Entity entity, AffectTeam team)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			var c = entity as Character;
			if (c == null || c == this) return null;

			if ((team & AffectTeam.Enemy) != AffectTeam.Enemy && Team != c.Team) return null;
			if ((team & AffectTeam.Friendly) != AffectTeam.Friendly && Team == c.Team) return null;

			return c;
		}

		public Projectile FilterEntityAsProjectile(Entity entity, int projid)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			var proj = entity as Projectile;
			if (proj == null || proj.BasePlayer != this || projid > 0 && proj.Data.ProjectileId != projid) return null;

			return proj;
		}

		public Character FilterEntityAsPartner(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			if (entity == this) return null;
			if (entity == BasePlayer) return null;
			if (entity.Team != Team) return null;

			if (entity is Helper)
			{
				var helper = entity as Helper;
				return helper.Data.Type == HelperType.Player ? helper : null;
			}

			if (entity is Player)
			{
				return entity as Player;
			}

			return null;
		}

		public IEnumerable<Character> GetTargets(int target_id)
		{
			if (target_id >= 0)
			{
				foreach (var character in OffensiveInfo.TargetList)
				{
					//if (character.AttackInfo.HitTime == 0) continue;
					//if (character.MoveType != MoveType.BeingHit) continue;
					//if (character.AttackInfo.Attacker != this) continue;

					if (character.DefensiveInfo.HitDef.TargetId == target_id) yield return character;
				}
			}
			else
			{
				foreach (var character in OffensiveInfo.TargetList) yield return character;
			}
		}

		public IEnumerable<Explod> GetExplods(int id)
		{
			if (id >= 0)
			{
				List<Explod> explods;
				if (Explods.TryGetValue(id, out explods))
				{
					foreach (var explod in explods) yield return explod;
				}

			}
			else
			{
				foreach (var data in Explods)
				{
					foreach (var explod in data.Value) yield return explod;
				}
			}
		}

		private void RemoveFromOthersTargetLists()
		{
			foreach (var entity in Engine.Entities)
			{
				var character = entity as Character;
				if (character == null) continue;

				character.OffensiveInfo.TargetList.Remove(this);
			}
		}

		public abstract Audio.SoundManager SoundManager { get; }

		public abstract Commands.CommandManager CommandManager { get; }

		public abstract StateMachine.StateManager StateManager { get; }

		public abstract CharacterDimensions Dimensions { get; }

		public override EntityUpdateOrder UpdateOrder => EntityUpdateOrder.Character;

		public Physics Physics
		{
			get => m_physics;

			set { m_physics = value; }
		}

		public MoveType MoveType
		{
			get => m_movetype;

			set
			{
				if (value == MoveType.Unchanged) throw new ArgumentOutOfRangeException(nameof(value));

				if (m_movetype == MoveType.BeingHit && value != MoveType.BeingHit) RemoveFromOthersTargetLists();

				m_movetype = value;
			}
		}

		public PlayerControl PlayerControl
		{
			get => m_playercontrol;

			set
			{

				if (value != PlayerControl.InControl && value != PlayerControl.NoControl) throw new ArgumentException("Value must be InControl or NoControl", nameof(value));
				m_playercontrol = value;
			}
		}

		public StateType StateType
		{
			get => m_statetype;

			set
			{
				if (value == StateType.None || value == StateType.Unchanged) throw new ArgumentOutOfRangeException(nameof(value));
				m_statetype = value;
			}
		}

		public int Life
		{
			get { return m_life; }

			[DebuggerStepThrough]
			set
			{
				m_life = Misc.Clamp(value, 0, BasePlayer.Constants.MaximumLife);
			}
		}

		public int Id { get; set; }

		public Vector2 DrawOffset { get; set; }

		public bool PositionFreeze { get; set; }

		public int RoundsExisted { get; set; }

		public bool CameraFollowX { get; set; }

		public bool CameraFollowY { get; set; }

		public bool ScreenBound { get; set; }

		public bool InHitPause { get; set; }

		public CharacterBind Bind => m_bind;

		public CharacterAssertions Assertions => m_assertions;

		public int JugglePoints { get; set; }

		public CharacterVariables Variables => m_variables;

		public StringBuilder Clipboard => m_clipboard;

		public bool PushFlag { get; set; }

		public Vector2 DrawScale { get; set; }

		public OffensiveInfo OffensiveInfo => m_offensiveinfo;

		public DefensiveInfo DefensiveInfo => m_defensiveinfo;

		public bool UpdatedAnimation => m_updatedanimation;

		public Dictionary<int, List<Explod>> Explods => m_explods;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerButton m_currentinput;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Physics m_physics;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MoveType m_movetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerControl m_playercontrol;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private StateType m_statetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CharacterBind m_bind;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CharacterAssertions m_assertions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CharacterVariables m_variables;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StringBuilder m_clipboard;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly OffensiveInfo m_offensiveinfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly DefensiveInfo m_defensiveinfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_updatedanimation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<int, List<Explod>> m_explods;

		#endregion
	}
}