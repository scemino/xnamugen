using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class ShowCharacterIntro : Base
	{
		public ShowCharacterIntro(FightEngine engine)
			: base(engine, RoundState.Intro)
		{
			m_finishearly = false;
		}

		public override void Reset()
		{
			base.Reset();

			m_finishearly = false;
		}

		public override void Update()
		{
			base.Update();

			CheckForEarlyEnd();
		}

		void CheckForEarlyEnd()
		{
			if (IsFinished() == true) return;

			if (Engine.Team1.MainPlayer != null && PlayeInputSkip(Engine.Team1.MainPlayer) == true) m_finishearly = true;
			if (Engine.Team1.TeamMate != null && PlayeInputSkip(Engine.Team1.TeamMate) == true) m_finishearly = true;

			if (Engine.Team2.MainPlayer != null && PlayeInputSkip(Engine.Team2.MainPlayer) == true) m_finishearly = true;
			if (Engine.Team2.TeamMate != null && PlayeInputSkip(Engine.Team2.TeamMate) == true) m_finishearly = true;

			if (m_finishearly == false) return;

			Engine.Camera.Location = new Point(0, 0);

			Engine.Entities.Clear();
			Engine.Team1.DoAction(SetPlayer);
			Engine.Team2.DoAction(SetPlayer);
		}

		void SetPlayer(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

			Engine.Entities.Add(player);

			player.StateManager.ChangeState(0);
			player.SetLocalAnimation(0, 0);
			player.PlayerControl = PlayerControl.NoControl;
			player.Life = player.Constants.MaximumLife;
			player.SoundManager.Stop();
			player.JugglePoints = player.Constants.AirJuggle;
			player.StateManager.ChangeState(StateMachine.StateNumber.Standing);

			if (player.Team.Side == TeamSide.Left)
			{
				player.CurrentLocation = Engine.Stage.P1Start;
				player.CurrentFacing = Engine.Stage.P1Facing;
			}
			else
			{
				player.CurrentLocation = Engine.Stage.P2Start;
				player.CurrentFacing = Engine.Stage.P2Facing;
			}
		}

		protected override Elements.Base GetElement()
		{
			return null;
		}

		public override Boolean IsFinished()
		{
			return m_finishearly == true || Engine.Assertions.Intro == false;
		}

		Boolean PlayeInputSkip(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

			if (player.CommandManager.IsActive("x") == true) return true;
			if (player.CommandManager.IsActive("y") == true) return true;
			if (player.CommandManager.IsActive("z") == true) return true;

			if (player.CommandManager.IsActive("a") == true) return true;
			if (player.CommandManager.IsActive("b") == true) return true;
			if (player.CommandManager.IsActive("c") == true) return true;

			if (player.CommandManager.IsActive("taunt") == true) return true;

			return false;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_finishearly;

		#endregion
	}
}