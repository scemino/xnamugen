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
	[DebuggerDisplay("Helper - #{Data.HelperId} - {Data.Name}")]
	class Helper : Character
	{
		public Helper(FightEngine engine, Character parent, HelperData data)
			: base(engine)
		{
			if (parent == null) throw new ArgumentNullException("parent");
			if (data == null) throw new ArgumentNullException("data");

			m_parent = parent;
			m_baseplayer = m_parent.BasePlayer;
			m_team = m_baseplayer.Team;
			m_offsetcharacter = (data.PositionType == PositionType.P2) ? parent.GetOpponent() : parent;
			m_remove = false;
			m_data = data;
			m_firsttick = true;
			m_statemanager = Parent.StateManager.Clone(this);
			m_spritemanager = Parent.SpriteManager.Clone();
			m_animationmanager = Parent.AnimationManager.Clone();
			m_commandmanager = Parent.CommandManager.Clone();
			m_soundmanager = Parent.SoundManager.Clone();
			m_dimensions = new CharacterDimensions(Data.GroundFront, Data.GroundBack, Data.AirFront, Data.AirBack, Data.Height);
			m_palfx = (Data.OwnPaletteFx == true) ? new PaletteFx() : Parent.PaletteFx;

			CurrentPalette = Parent.CurrentPalette;
			CurrentFacing = GetFacing(Data.PositionType, m_offsetcharacter.CurrentFacing, Data.FacingFlag < 0);
			CurrentLocation = GetStartLocation();
			CurrentScale = Data.Scale;

			SetLocalAnimation(0, 0);

			StateManager.ChangeState(Data.InitialStateNumber);
		}

		Vector2 GetStartLocation()
		{
			Rectangle camerabounds = Engine.Camera.ScreenBounds;
			Facing facing = m_offsetcharacter.CurrentFacing;
			Vector2 location;

			switch (Data.PositionType)
			{
				case PositionType.P1:
				case PositionType.P2:
					return Misc.GetOffset(m_offsetcharacter.CurrentLocation, facing, Data.CreationOffset);

				case PositionType.Left:
					return Misc.GetOffset(new Vector2(camerabounds.Left, m_offsetcharacter.CurrentLocation.Y), Facing.Right, Data.CreationOffset);

				case PositionType.Right:
					return Misc.GetOffset(new Vector2(camerabounds.Right, m_offsetcharacter.CurrentLocation.Y), Facing.Right, Data.CreationOffset);

				case PositionType.Back:
					location = Misc.GetOffset(new Vector2(0, 0), facing, Data.CreationOffset);
					location.X += (facing == Facing.Right) ? camerabounds.Left : camerabounds.Right;
					return location;

				case PositionType.Front:
					location = Misc.GetOffset(new Vector2(0, 0), m_offsetcharacter.CurrentFacing, Data.CreationOffset);
					location.X += (facing == Facing.Left) ? camerabounds.Left : camerabounds.Right;
					return location;

				default:
					throw new ArgumentOutOfRangeException("postype");
			}
		}

		public override void UpdateState()
		{
			if (m_firsttick == true)
			{
				m_firsttick = false;

				CurrentLocation = GetStartLocation();
			}

			base.UpdateState();
		}

		public override void UpdateInput()
		{
			if (Data.KeyControl == true)
			{
				base.UpdateInput();
			}
		}

		public override void RecieveInput(PlayerButton button, Boolean pressed)
		{
			if (Data.KeyControl == true)
			{
				base.RecieveInput(button, pressed);
			}
		}

		public override Boolean IsPaused(Pause pause)
		{
			if (base.IsPaused(pause) == false) return false;

			if (pause.IsSuperPause == true)
			{
				if (pause.ElapsedTime <= Data.SuperPauseTime) return false;
			}
			else
			{
				if (pause.ElapsedTime <= Data.PauseTime) return false;
			}

			return true;
		}

		static Facing GetFacing(PositionType ptype, Facing characterfacing, Boolean facingflag)
		{
			switch (ptype)
			{
				case PositionType.P1:
				case PositionType.Front:
				case PositionType.Back:
				case PositionType.P2:
					return (facingflag == true) ? Misc.FlipFacing(characterfacing) : characterfacing;

				case PositionType.Left:
				case PositionType.Right:
					return (facingflag == true) ? Facing.Left : Facing.Right;

				case PositionType.None:
				default:
					throw new ArgumentNullException("ptype");
			}
		}

		public void Remove()
		{
			m_remove = true;
			Engine.Entities.Remove(this);
		}

		public override Boolean RemoveCheck()
		{
			return m_remove;
		}

		protected override void Bounding()
		{
			if (Data.Type == HelperType.Player)
			{
				base.Bounding();
			}
		}

		public Character Parent
		{
			get { return m_parent; }
		}

		public override Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public override Animations.AnimationManager AnimationManager
		{
			get { return m_animationmanager; }
		}

		public override Audio.SoundManager SoundManager
		{
			get { return m_soundmanager; }
		}

		public override Commands.CommandManager CommandManager
		{
			get { return m_commandmanager; }
		}

		public override StateMachine.StateManager StateManager
		{
			get { return m_statemanager; }
		}

		public override CharacterDimensions Dimensions
		{
			get { return m_dimensions; }
		}

		public HelperData Data
		{
			get { return m_data; }
		}

		public override PaletteFx PaletteFx
		{
			get { return m_palfx; }
		}

		public override Team Team
		{
			get { return m_team; }
		}

		public override Player BasePlayer
		{
			get { return m_baseplayer; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_parent;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Player m_baseplayer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Team m_team;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_offsetcharacter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Commands.CommandManager m_commandmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CharacterDimensions m_dimensions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StateMachine.StateManager m_statemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_remove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HelperData m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PaletteFx m_palfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_firsttick;

		#endregion
	}
}