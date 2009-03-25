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
	class VersusScreen : NonCombatScreen
	{
		public VersusScreen(MenuSystem screensystem, TextSection textsection, String spritepath, String animationpath, String soundpath)
			: base(screensystem, textsection, spritepath, animationpath, soundpath)
		{
			m_visibletime = textsection.GetAttribute<Int32>("time");
			m_p1 = new VersusData("p1.", textsection);
			m_p2 = new VersusData("p2.", textsection);
			m_timer = new CountdownTimer(TimeSpan.FromSeconds(m_visibletime / 60.0f), this.ShowTimeComplete);
		}

		public override void Reset()
		{
			base.Reset();

			m_timer.Reset();
		}

		public override void SetInput(Input.InputState inputstate)
		{
			base.SetInput(inputstate);

			foreach (PlayerButton button in Enum.GetValues(typeof(PlayerButton)))
			{
				PlayerButton b = button;

				inputstate[1].Add(b, SwitchToCombat);
				inputstate[2].Add(b, SwitchToCombat);
			}
		}

		public override void FadeInComplete()
		{
			base.FadeInComplete();

			m_timer.Reset();
			m_timer.IsRunning = true;
		}

		public override void Update(GameTime gametime)
		{
			base.Update(gametime);

			m_timer.Update(gametime);
		}

		public override void Draw(Boolean debugdraw)
		{
			base.Draw(debugdraw);

			P1.Profile.SpriteManager.Draw(SpriteId.LargePortrait, (Vector2)P1.PortraitLocation, Vector2.Zero, P1.PortraitScale, P1.PortraitFlip);
			P2.Profile.SpriteManager.Draw(SpriteId.LargePortrait, (Vector2)P2.PortraitLocation, Vector2.Zero, P2.PortraitScale, P2.PortraitFlip);

			Print(P1.NameFont, (Vector2)P1.NameLocation, P1.Profile.DisplayName, null);
			Print(P2.NameFont, (Vector2)P2.NameLocation, P2.Profile.DisplayName, null);
		}

		void SwitchToCombat(Boolean pressed)
		{
			if (pressed == true)
			{
				m_timer.Reset();
				m_timer.IsRunning = false;

				ShowTimeComplete(this, EventArgs.Empty);
			}
		}

		void ShowTimeComplete(Object sender, EventArgs args)
		{
			MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Combat));
		}

		public Int32 VisibleTime
		{
			get { return m_visibletime; }
		}

		public VersusData P1
		{
			get { return m_p1; }
		}

		public VersusData P2
		{
			get { return m_p2; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_visibletime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly VersusData m_p1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly VersusData m_p2;

		CountdownTimer m_timer;

		#endregion
	}
}