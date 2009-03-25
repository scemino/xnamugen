using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	class Team : EngineObject
	{
		public Team(FightEngine engine, TeamSide side)
			: base(engine)
		{
			if (side != TeamSide.Left && side != TeamSide.Right) throw new ArgumentException("Side must be either Left or Right", "side");

			m_side = side;
			m_victorystatus = new VictoryStatus(this);
			m_display = new TeamDisplay(this);
			m_winhistory = new List<Win>(9);
			m_p1 = null;
			m_p2 = null;
		}

		public void Clear()
		{
			m_winhistory.Clear();
			m_p1 = null;
			m_p2 = null;
		}

		public void DoAction(Action<Player> action)
		{
			if (action == null) throw new ArgumentNullException("action");

			action(MainPlayer);
			if (TeamMate != null) action(TeamMate);
		}

		public void ResetPlayers()
		{
			MainPlayer.StateManager.ChangeState(0);
			MainPlayer.SetLocalAnimation(0, 0);
			MainPlayer.PlayerControl = PlayerControl.NoControl;
			MainPlayer.Life = MainPlayer.Constants.MaximumLife;
			MainPlayer.Power = 0;
			MainPlayer.SoundManager.Stop();
			MainPlayer.JugglePoints = MainPlayer.Constants.AirJuggle;

			if (Side == TeamSide.Left)
			{
				MainPlayer.CurrentLocation = Engine.Stage.P1Start;
				MainPlayer.CurrentFacing = Engine.Stage.P1Facing;
			}
			else
			{
				MainPlayer.CurrentLocation = Engine.Stage.P2Start;
				MainPlayer.CurrentFacing = Engine.Stage.P2Facing;
			}

			if (TeamMate != null)
			{
				TeamMate.StateManager.ChangeState(0);
				TeamMate.SetLocalAnimation(0, 0);
				TeamMate.PlayerControl = PlayerControl.NoControl;
				TeamMate.Life = TeamMate.Constants.MaximumLife;
				TeamMate.Power = 0;
				TeamMate.SoundManager.Stop();
				TeamMate.JugglePoints = TeamMate.Constants.AirJuggle;

				if (Side == TeamSide.Left)
				{
					MainPlayer.CurrentLocation = Engine.Stage.P1Start;
					MainPlayer.CurrentFacing = Engine.Stage.P1Facing;
				}
				else
				{
					MainPlayer.CurrentLocation = Engine.Stage.P2Start;
					MainPlayer.CurrentFacing = Engine.Stage.P2Facing;
				}
			}
		}

		public void AddWin(Win win)
		{
			m_winhistory.Add(win);
		}

		public void CreatePlayers(PlayerCreation p1, PlayerCreation p2)
		{
			if (p1 == null) throw new ArgumentNullException("p1");

			Clear();

			m_p1 = new Player(Engine, p1.Profile, this);
			m_p1.PaletteNumber = p1.PaletteIndex;

			if (p2 != null)
			{
				m_p2 = new Player(Engine, p2.Profile, this);
				m_p2.PaletteNumber = p2.PaletteIndex;
			}

			ResetPlayers();
		}

		public TeamSide Side
		{
			get { return m_side; }
		}

		public Player MainPlayer
		{
			get { return m_p1; }
		}

		public Player TeamMate
		{
			get { return m_p2; }
		}

		public Team OtherTeam
		{
			get { return (Side == TeamSide.Left) ? Engine.Team2 : Engine.Team1; }
		}

		public VictoryStatus VictoryStatus
		{
			get { return m_victorystatus; }
		}

		public TeamDisplay Display
		{
			get { return m_display; }
		}

		public ListIterator<Win> Wins
		{
			get { return new ListIterator<Win>(m_winhistory); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TeamSide m_side;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly VictoryStatus m_victorystatus;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TeamDisplay m_display;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Win> m_winhistory;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Player m_p1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Player m_p2;

		#endregion
	}
}