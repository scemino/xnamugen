using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

namespace xnaMugen.Diagnostics
{
	class GeneralPanel : Panel
	{
		public GeneralPanel()
		{
			m_stringbuilder = new StringBuilder();

			m_text = new Label();
			m_text.AutoSize = true;

			this.Controls.Add(m_text);
		}

		public void Set(Combat.FightEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");

			m_stringbuilder.Length = 0;

			BuildEntityText(engine.Entities);
			BuildStageText(engine.Stage);
			BuildPlayerText(engine.Team1.MainPlayer, 1);
			BuildPlayerText(engine.Team2.MainPlayer, 2);			

			m_text.Text = m_stringbuilder.ToString();
		}

		void BuildEntityText(Combat.EntityCollection collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");

			Int32 players, helpers, explods, projectiles;
			collection.CountEntities(out players, out helpers, out explods, out projectiles);

			m_stringbuilder.AppendFormat("Entity Count: {0}{1}", players + helpers + explods + projectiles, Environment.NewLine);
			m_stringbuilder.AppendFormat("Players: {0}    Helpers: {1}{2}", players, helpers, Environment.NewLine);
			m_stringbuilder.AppendFormat("Explods: {0}    Projectiles: {1}{2}", explods, projectiles, Environment.NewLine);
			m_stringbuilder.AppendLine();
		}

		void BuildStageText(Combat.Stage stage)
		{
			if (stage != null)
			{
				m_stringbuilder.AppendFormat("Stage: {0}\r\n", stage.Name);
				m_stringbuilder.AppendFormat("Definition File: {0}\r\n", stage.Profile.Filepath);
			}
			else
			{
				m_stringbuilder.AppendLine("NoStage\r\n");
			}

			m_stringbuilder.AppendLine();
		}

		void BuildPlayerText(Combat.Player player, Int32 playernumber)
		{
			if (player != null)
			{
				m_stringbuilder.AppendFormat("Player {0} - {1}{2}", playernumber, player.Profile.PlayerName, Environment.NewLine);
				m_stringbuilder.AppendFormat("Definition File: {0}{1}", player.Profile.DefinitionPath, Environment.NewLine);
				m_stringbuilder.AppendFormat("Life: {0} / {1}    Power: {2} / {3}{4}", player.Life, player.Constants.MaximumLife, player.Power, player.Constants.MaximumPower, Environment.NewLine);
				m_stringbuilder.AppendFormat("Anim: {0}   Spr: {1}   Elem: {2} / {3}   Time: {4} / {5}\r\n", player.AnimationManager.CurrentAnimation.Number, player.AnimationManager.CurrentElement.SpriteId, player.AnimationManager.CurrentElement.Id + 1, player.AnimationManager.CurrentAnimation.Elements.Count, player.AnimationManager.TimeInAnimation, player.AnimationManager.CurrentAnimation.TotalTime);
				m_stringbuilder.AppendFormat("State: {0}    StateTime: {1}    Foreign States: {2}\r\n", player.StateManager.CurrentState.Number, player.StateManager.StateTime, player.StateManager.ForeignManager != null);
				m_stringbuilder.AppendFormat("StateType: {0}    MoveType: {1}\r\nPhsyics: {2}    ControlFlag: {3}\r\n", player.StateType, player.MoveType, player.Physics, player.PlayerControl);
				m_stringbuilder.AppendFormat("Active Hitdef: {0}    Juggle Points: {1}\r\n", player.OffensiveInfo.ActiveHitDef, player.JugglePoints);
				m_stringbuilder.AppendFormat("-------------{0}", Environment.NewLine);
				m_stringbuilder.AppendLine(player.Clipboard.ToString());
			}
			else
			{
				m_stringbuilder.AppendFormat("Player {0} does not exist\r\n", playernumber);
			}

			m_stringbuilder.AppendLine();
		}

		readonly Label m_text;

		readonly StringBuilder m_stringbuilder;
	}
}