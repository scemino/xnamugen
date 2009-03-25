using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
	class TitleScreen : NonCombatScreen
	{
		public TitleScreen(MenuSystem screensystem, TextSection textsection, String spritepath, String animationpath, String soundpath) :
			base(screensystem, textsection, spritepath, animationpath, soundpath)
		{
			m_menuposition = textsection.GetAttribute<Point>("menu.pos");
			m_mainfont = textsection.GetAttribute<PrintData>("menu.item.font");
			m_activefont = textsection.GetAttribute<PrintData>("menu.item.active.font");
			m_spacing = textsection.GetAttribute<Point>("menu.item.spacing");
			m_visiblemenuitems = textsection.GetAttribute<Int32>("menu.window.visibleitems");
			m_cursorvisible = textsection.GetAttribute<Boolean>("menu.boxcursor.visible");
			m_soundcursormove = textsection.GetAttribute<SoundId>("cursor.move.snd");
			m_soundselect = textsection.GetAttribute<SoundId>("cursor.done.snd");
			m_soundcancel = textsection.GetAttribute<SoundId>("cancel.snd");

			m_menutext = BuildMenuText(textsection);

			Point margins = textsection.GetAttribute<Point>("menu.window.margins.y");
			m_marginytop = margins.X;
			m_marginybottom = margins.Y;

			m_currentmenuitem = 0;
			m_verticalmenudrawoffset = 0;
			m_quitselected = false;
		}

		static ReadOnlyDictionary<Int32, String> BuildMenuText(TextSection textsection)
		{
			Dictionary<Int32, String> menutext = new Dictionary<Int32, String>();

			menutext[(Int32)MainMenuOption.Arcade] = textsection.GetAttribute<String>("menu.itemname.arcade");
			menutext[(Int32)MainMenuOption.Versus] = textsection.GetAttribute<String>("menu.itemname.versus");
			menutext[(Int32)MainMenuOption.TeamArcade] = textsection.GetAttribute<String>("menu.itemname.teamarcade");
			menutext[(Int32)MainMenuOption.TeamVersus] = textsection.GetAttribute<String>("menu.itemname.teamversus");
			menutext[(Int32)MainMenuOption.TeamCoop] = textsection.GetAttribute<String>("menu.itemname.teamcoop");
			menutext[(Int32)MainMenuOption.Survival] = textsection.GetAttribute<String>("menu.itemname.survival");
			menutext[(Int32)MainMenuOption.SurvivalCoop] = textsection.GetAttribute<String>("menu.itemname.survivalcoop");
			menutext[(Int32)MainMenuOption.Training] = textsection.GetAttribute<String>("menu.itemname.training");
			menutext[(Int32)MainMenuOption.Watch] = textsection.GetAttribute<String>("menu.itemname.watch");
			menutext[(Int32)MainMenuOption.Options] = textsection.GetAttribute<String>("menu.itemname.options");
			menutext[(Int32)MainMenuOption.Quit] = textsection.GetAttribute<String>("menu.itemname.exit");

#warning Some menu items aren't implemented yet
			menutext[(Int32)MainMenuOption.Arcade] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.TeamArcade] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.TeamVersus] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.TeamCoop] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.Survival] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.SurvivalCoop] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.Training] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.Watch] = "NOT IMPLEMENTED";
			menutext[(Int32)MainMenuOption.Options] = "NOT IMPLEMENTED";

			return new ReadOnlyDictionary<Int32, String>(menutext);
		}

		public override void SetInput(Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			inputstate[0].Add(SystemButton.Quit, this.QuitGame);

			inputstate[1].Add(PlayerButton.Up, this.DecreaseActiveMenuItem);
			inputstate[1].Add(PlayerButton.Down, this.IncreaseActiveMenuItem);
			inputstate[1].Add(PlayerButton.A, this.SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.B, this.SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.C, this.SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.X, this.SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.Y, this.SelectActiveMenuItem);
			inputstate[1].Add(PlayerButton.Z, this.SelectActiveMenuItem);
		}

		public override void Reset()
		{
			base.Reset();

			m_currentmenuitem = 0;
			m_verticalmenudrawoffset = 0;
			m_quitselected = false;
		}

		public override void Draw(Boolean debugdraw)
		{
			base.Draw(debugdraw);

			Int32 height = (m_spacing.Y * (m_visiblemenuitems - 1)) + m_marginytop + m_marginybottom;
			Rectangle scissorrect = new Rectangle(0, m_menuposition.Y - m_spacing.Y, Mugen.ScreenSize.X, height);

			Int32 offset = 0;
			for (Int32 i = 0; i != 11; ++i) DrawMenuItem(i, ref offset, scissorrect);
		}

		public override void FadeOutComplete()
		{
			base.FadeOutComplete();

			if (m_quitselected == true)
			{
				MenuSystem.SubSystems.Game.Exit();
			}
		}

		void DrawMenuItem(Int32 i, ref Int32 offset, Rectangle? scissorrect)
		{
			if (m_menutext.ContainsKey(i) == false) return;
			String text = m_menutext[i];

			PrintData data = (i == m_currentmenuitem) ? m_activefont : m_mainfont;

			Vector2 location = (Vector2)m_menuposition;
			location.X += m_spacing.X * offset;
			location.Y += m_spacing.Y * offset;
			location.Y -= m_verticalmenudrawoffset;

			++offset;

			Print(data, location, text, scissorrect);
		}

		void IncreaseActiveMenuItem(Boolean pressed)
		{
			if (pressed == true)
			{
				if (m_currentmenuitem == 10)
				{
					m_currentmenuitem = 0;

					m_verticalmenudrawoffset = 0;
				}
				else
				{
					++m_currentmenuitem;

					Int32 menuoffset = m_verticalmenudrawoffset / m_spacing.Y;
					if (m_currentmenuitem >= menuoffset + m_visiblemenuitems) m_verticalmenudrawoffset += m_spacing.Y;
				}

				SoundManager.Play(m_soundcursormove);
			}
		}

		void DecreaseActiveMenuItem(Boolean pressed)
		{
			if (pressed == true)
			{
				if (m_currentmenuitem == 0)
				{
					m_currentmenuitem = 10;

					m_verticalmenudrawoffset = m_spacing.Y * (11 - m_visiblemenuitems);
				}
				else
				{
					--m_currentmenuitem;

					Int32 menuoffset = m_verticalmenudrawoffset / m_spacing.Y;
					if (m_currentmenuitem < menuoffset) m_verticalmenudrawoffset -= m_spacing.Y;
				}

				SoundManager.Play(m_soundcursormove);
			}
		}

		void SelectActiveMenuItem(Boolean pressed)
		{
			if (pressed == true)
			{
				SoundManager.Play(m_soundselect);

				if (m_currentmenuitem == (Int32)MainMenuOption.Versus)
				{
					MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Select));
				}
				else if (m_currentmenuitem == (Int32)MainMenuOption.Quit)
				{
					QuitGame(true);
				}
			}
		}

		void QuitGame(Boolean pressed)
		{
			if (pressed == true)
			{
				m_quitselected = true;
				SoundManager.Play(m_soundcancel);
				MenuSystem.PostEvent(new Events.FadeScreen(FadeDirection.Out));
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyDictionary<Int32, String> m_menutext;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_menuposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PrintData m_mainfont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PrintData m_activefont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_spacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_marginytop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_marginybottom;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_visiblemenuitems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_cursorvisible;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundId m_soundcursormove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundId m_soundselect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundId m_soundcancel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_currentmenuitem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_verticalmenudrawoffset;

		Boolean m_quitselected;

		#endregion
	}
}