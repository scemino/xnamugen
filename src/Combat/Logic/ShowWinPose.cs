using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class ShowWinPose : Base
	{
		public ShowWinPose(FightEngine engine)
			: base(engine, RoundState.Over)
		{
		}

		Team GetWinningTeam()
		{
			if (Engine.Team1.VictoryStatus.Win == true) return Engine.Team1;
			else if (Engine.Team2.VictoryStatus.Win == true) return Engine.Team2;
			else return null;
		}

		void EnterWinPose(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

			player.StateManager.ChangeState(StateMachine.StateNumber.WinPose);
		}

		void EnterTimeLosePose(Player player)
		{
			if (player == null) throw new ArgumentNullException("player");

			player.StateManager.ChangeState(StateMachine.StateNumber.LoseTimeOverPose);
		}

		protected override void OnFirstTick()
		{
			base.OnFirstTick();

			Team winningteam = GetWinningTeam();
			if (winningteam != null)
			{
				winningteam.DoAction(EnterWinPose);

				Win win = new Win(Victory.Normal, winningteam.VictoryStatus.WinPerfect == true);
				winningteam.AddWin(win);

				if (winningteam.OtherTeam.VictoryStatus.LoseTime == true)
				{
					winningteam.OtherTeam.DoAction(EnterTimeLosePose);
				}

				if (winningteam.TeamMate != null && CurrentElement != null)
				{
					DisplayString = Engine.GetSubSystem<StringFormatter>().BuildString(CurrentElement.DataMap.Text, winningteam.MainPlayer.Profile.DisplayName, winningteam.TeamMate.Profile.DisplayName);
				}
				else
				{
					DisplayString = Engine.GetSubSystem<StringFormatter>().BuildString(CurrentElement.DataMap.Text, winningteam.MainPlayer.Profile.DisplayName);
				}
			}
			else
			{
				Engine.Team1.DoAction(EnterTimeLosePose);
				Engine.Team2.DoAction(EnterTimeLosePose);
			}
		}

		protected override Elements.Base GetElement()
		{
			Team winningteam = GetWinningTeam();

			if (winningteam == null)
			{
                return Engine.Elements.GetElement("draw");
			}

			if (winningteam.TeamMate != null)
			{
                return Engine.Elements.GetElement("win2");
			}

            return Engine.Elements.GetElement("win");
		}

		public override Boolean IsFinished()
		{
			if (Engine.Team1.MainPlayer != null && PlayeInputSkip(Engine.Team1.MainPlayer) == true) return true;
			if (Engine.Team1.TeamMate != null && PlayeInputSkip(Engine.Team1.TeamMate) == true) return true;

			if (Engine.Team2.MainPlayer != null && PlayeInputSkip(Engine.Team2.MainPlayer) == true) return true;
			if (Engine.Team2.TeamMate != null && PlayeInputSkip(Engine.Team2.TeamMate) == true) return true;

			return Engine.Assertions.WinPose == false && (TickCount > Engine.RoundInformation.OverTime || CurrentElement == null);
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
	}
}