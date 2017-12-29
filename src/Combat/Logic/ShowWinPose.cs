using System;

namespace xnaMugen.Combat.Logic
{
	internal class ShowWinPose : Base
	{
		public ShowWinPose(FightEngine engine)
			: base(engine, RoundState.Over)
		{
		}

		private Team GetWinningTeam()
		{
			if (Engine.Team1.VictoryStatus.Win) return Engine.Team1;
			if (Engine.Team2.VictoryStatus.Win) return Engine.Team2;
			return null;
		}

		private void EnterWinPose(Player player)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));

			player.StateManager.ChangeState(StateMachine.StateNumber.WinPose);
		}

		private void EnterTimeLosePose(Player player)
		{
			if (player == null) throw new ArgumentNullException(nameof(player));

			player.StateManager.ChangeState(StateMachine.StateNumber.LoseTimeOverPose);
		}

		protected override void OnFirstTick()
		{
			base.OnFirstTick();

			var winningteam = GetWinningTeam();
			if (winningteam != null)
			{
				winningteam.DoAction(EnterWinPose);

				var win = new Win(Victory.Normal, winningteam.VictoryStatus.WinPerfect);
				winningteam.AddWin(win);

				if (winningteam.OtherTeam.VictoryStatus.LoseTime)
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
			var winningteam = GetWinningTeam();

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

		public override bool IsFinished()
		{
			if (Engine.Team1.MainPlayer != null && PlayeInputSkip(Engine.Team1.MainPlayer)) return true;
			if (Engine.Team1.TeamMate != null && PlayeInputSkip(Engine.Team1.TeamMate)) return true;

			if (Engine.Team2.MainPlayer != null && PlayeInputSkip(Engine.Team2.MainPlayer)) return true;
			if (Engine.Team2.TeamMate != null && PlayeInputSkip(Engine.Team2.TeamMate)) return true;

			return Engine.Assertions.WinPose == false && (TickCount > Engine.RoundInformation.OverTime || CurrentElement == null);
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
	}
}