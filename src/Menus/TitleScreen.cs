using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	internal class TitleScreen : NonCombatScreen
	{
		public TitleScreen(MenuSystem screensystem, TextSection textsection, string spritepath, string animationpath, string soundpath) :
			base(screensystem, textsection, spritepath, animationpath, soundpath)
		{
			m_menuposition = textsection.GetAttribute<Point>("menu.pos");
			m_mainfont = textsection.GetAttribute<PrintData>("menu.item.font");
			m_activefont = textsection.GetAttribute<PrintData>("menu.item.active.font");
			m_spacing = textsection.GetAttribute<Point>("menu.item.spacing");
			m_visiblemenuitems = textsection.GetAttribute<int>("menu.window.visibleitems");
			m_cursorvisible = textsection.GetAttribute<bool>("menu.boxcursor.visible");
			m_soundcursormove = textsection.GetAttribute<SoundId>("cursor.move.snd");
			m_soundselect = textsection.GetAttribute<SoundId>("cursor.done.snd");
			m_soundcancel = textsection.GetAttribute<SoundId>("cancel.snd");

			m_menutext = BuildMenuText(textsection);

			var margins = textsection.GetAttribute<Point>("menu.window.margins.y");
			m_marginytop = margins.X;
			m_marginybottom = margins.Y;

			m_currentmenuitem = 0;
			m_verticalmenudrawoffset = 0;
			m_quitselected = false;
		}

		private static ReadOnlyDictionary<int, string> BuildMenuText(TextSection textsection)
		{
			var menutext = new Dictionary<int, string>();

			menutext[(int)MainMenuOption.Arcade] = textsection.GetAttribute<string>("menu.itemname.arcade");
			menutext[(int)MainMenuOption.Versus] = textsection.GetAttribute<string>("menu.itemname.versus");
			menutext[(int)MainMenuOption.TeamArcade] = textsection.GetAttribute<string>("menu.itemname.teamarcade");
			menutext[(int)MainMenuOption.TeamVersus] = textsection.GetAttribute<string>("menu.itemname.teamversus");
			menutext[(int)MainMenuOption.TeamCoop] = textsection.GetAttribute<string>("menu.itemname.teamcoop");
			menutext[(int)MainMenuOption.Survival] = textsection.GetAttribute<string>("menu.itemname.survival");
			menutext[(int)MainMenuOption.SurvivalCoop] = textsection.GetAttribute<string>("menu.itemname.survivalcoop");
			menutext[(int)MainMenuOption.Training] = textsection.GetAttribute<string>("menu.itemname.training");
			menutext[(int)MainMenuOption.Watch] = textsection.GetAttribute<string>("menu.itemname.watch");
			menutext[(int)MainMenuOption.Options] = textsection.GetAttribute<string>("menu.itemname.options");
			menutext[(int)MainMenuOption.Quit] = textsection.GetAttribute<string>("menu.itemname.exit");

#warning Some menu items aren't implemented yet
			menutext[(int)MainMenuOption.Arcade] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.TeamArcade] = "NOT IMPLEMENTED";
			//menutext[(int)MainMenuOption.TeamVersus] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.TeamCoop] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.Survival] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.SurvivalCoop] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.Training] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.Watch] = "NOT IMPLEMENTED";
			menutext[(int)MainMenuOption.Options] = "NOT IMPLEMENTED";

			return new ReadOnlyDictionary<int, string>(menutext);
		}

		public override void SetInput(Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			inputstate[0].Add(SystemButton.Quit, QuitGame);

			inputstate[1].Add(PlayerButton.Up, DecreaseActiveMenuItem);
			inputstate[1].Add(PlayerButton.Down, IncreaseActiveMenuItem);
			inputstate[1].Add(PlayerButton.A, SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.B, SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.C, SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.X, SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.Y, SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.Z, SelectActiveMenuItem);
		}

		public override void Reset()
		{
			base.Reset();

			m_currentmenuitem = 0;
			m_verticalmenudrawoffset = 0;
			m_quitselected = false;
		}

		public override void Draw(bool debugdraw)
		{
			base.Draw(debugdraw);

			var height = m_spacing.Y * (m_visiblemenuitems - 1) + m_marginytop + m_marginybottom;
			var scissorrect = new Rectangle(0, m_menuposition.Y - m_spacing.Y, Mugen.ScreenSize.X, height);

			var offset = 0;
			for (var i = 0; i != 11; ++i) DrawMenuItem(i, ref offset, scissorrect);
		}

		public override void FadeOutComplete()
		{
			base.FadeOutComplete();

			if (m_quitselected)
			{
				MenuSystem.SubSystems.Game.Exit();
			}
		}

		private void DrawMenuItem(int i, ref int offset, Rectangle? scissorrect)
		{
			if (m_menutext.ContainsKey(i) == false) return;
			var text = m_menutext[i];

			var data = i == m_currentmenuitem ? m_activefont : m_mainfont;

			var location = (Vector2)m_menuposition;
			location.X += m_spacing.X * offset;
			location.Y += m_spacing.Y * offset;
			location.Y -= m_verticalmenudrawoffset;

			++offset;

			Print(data, location, text, scissorrect);
		}

		private void IncreaseActiveMenuItem(bool pressed)
		{
			if (pressed)
			{
				if (m_currentmenuitem == 10)
				{
					m_currentmenuitem = 0;

					m_verticalmenudrawoffset = 0;
				}
				else
				{
					++m_currentmenuitem;

					var menuoffset = m_verticalmenudrawoffset / m_spacing.Y;
					if (m_currentmenuitem >= menuoffset + m_visiblemenuitems) m_verticalmenudrawoffset += m_spacing.Y;
				}

				SoundManager.Play(m_soundcursormove);
			}
		}

		private void DecreaseActiveMenuItem(bool pressed)
		{
			if (pressed)
			{
				if (m_currentmenuitem == 0)
				{
					m_currentmenuitem = 10;

					m_verticalmenudrawoffset = m_spacing.Y * (11 - m_visiblemenuitems);
				}
				else
				{
					--m_currentmenuitem;

					var menuoffset = m_verticalmenudrawoffset / m_spacing.Y;
					if (m_currentmenuitem < menuoffset) m_verticalmenudrawoffset -= m_spacing.Y;
				}

				SoundManager.Play(m_soundcursormove);
			}
		}

		private void SelectActiveMenuItem(bool pressed)
		{
			if (pressed)
			{
				SoundManager.Play(m_soundselect);
                switch (m_currentmenuitem)
                {
                    case (int)MainMenuOption.Versus:
                        MenuSystem.PostEvent(new Events.SetupCombatMode(CombatMode.Versus));
                        MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Select));
                        break;
                    case (int)MainMenuOption.TeamVersus:
                        MenuSystem.PostEvent(new Events.SetupCombatMode(CombatMode.TeamVersus));
                        MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Select));
                        break;
                    case (int)MainMenuOption.Quit:
                        QuitGame(true);
                        break;
                }
            }
		}

		private void QuitGame(bool pressed)
		{
			if (pressed)
			{
				m_quitselected = true;
				SoundManager.Play(m_soundcancel);
				MenuSystem.PostEvent(new Events.FadeScreen(FadeDirection.Out));
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyDictionary<int, string> m_menutext;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_menuposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_mainfont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_activefont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_spacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_marginytop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_marginybottom;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_visiblemenuitems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_cursorvisible;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundcursormove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundselect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundcancel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_currentmenuitem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_verticalmenudrawoffset;

		private bool m_quitselected;

		#endregion
	}
}