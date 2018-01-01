using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	internal class ShowCharacterIntro : Base
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

		private void CheckForEarlyEnd()
		{
			if (IsFinished()) return;

			if (Engine.Team1.MainPlayer != null && PlayeInputSkip(Engine.Team1.MainPlayer)) m_finishearly = true;
			if (Engine.Team1.TeamMate != null && PlayeInputSkip(Engine.Team1.TeamMate)) m_finishearly = true;

			if (Engine.Team2.MainPlayer != null && PlayeInputSkip(Engine.Team2.MainPlayer)) m_finishearly = true;
			if (Engine.Team2.TeamMate != null && PlayeInputSkip(Engine.Team2.TeamMate)) m_finishearly = true;

			if (m_finishearly == false) return;

			Engine.Camera.Location = new Point(0, 0);

			Engine.Entities.Clear();
			Engine.Team1.DoAction(SetPlayer);
			Engine.Team2.DoAction(SetPlayer);
		}

		private void SetPlayer(Player player)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));

			Engine.Entities.Add(player);

			player.StateManager.ChangeState(0);
			player.SetLocalAnimation(0, 0);
			player.PlayerControl = PlayerControl.NoControl;
			player.Life = player.Constants.MaximumLife;
			player.SoundManager.Stop();
			player.JugglePoints = player.Constants.AirJuggle;
			player.StateManager.ChangeState(StateMachine.StateNumber.Standing);

			player.Explods.Clear();
			player.Helpers.Clear();

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

		public override bool IsFinished()
		{
			return m_finishearly || Engine.Assertions.Intro == false;
		}

		private bool PlayeInputSkip(Player player)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));

			if (player.CommandManager.IsActive("x")) return true;
			if (player.CommandManager.IsActive("y")) return true;
			if (player.CommandManager.IsActive("z")) return true;

			if (player.CommandManager.IsActive("a")) return true;
			if (player.CommandManager.IsActive("b")) return true;
			if (player.CommandManager.IsActive("c")) return true;

			if (player.CommandManager.IsActive("taunt")) return true;

			return false;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_finishearly;

		#endregion
	}
}