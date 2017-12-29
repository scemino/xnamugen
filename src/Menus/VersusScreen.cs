using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	internal class VersusScreen : NonCombatScreen
	{
		public VersusScreen(MenuSystem screensystem, TextSection textsection, string spritepath, string animationpath, string soundpath)
			: base(screensystem, textsection, spritepath, animationpath, soundpath)
		{
			m_visibletime = textsection.GetAttribute<int>("time");
			m_p1 = new VersusData("p1.", textsection);
			m_p2 = new VersusData("p2.", textsection);
			m_timer = new CountdownTimer(TimeSpan.FromSeconds(m_visibletime / 60.0f), ShowTimeComplete);
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
				var b = button;

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

		public override void Draw(bool debugdraw)
		{
			base.Draw(debugdraw);

			P1.Profile.SpriteManager.Draw(SpriteId.LargePortrait, (Vector2)P1.PortraitLocation, Vector2.Zero, P1.PortraitScale, P1.PortraitFlip);
			P2.Profile.SpriteManager.Draw(SpriteId.LargePortrait, (Vector2)P2.PortraitLocation, Vector2.Zero, P2.PortraitScale, P2.PortraitFlip);

			Print(P1.NameFont, (Vector2)P1.NameLocation, P1.Profile.DisplayName, null);
			Print(P2.NameFont, (Vector2)P2.NameLocation, P2.Profile.DisplayName, null);
		}

		private void SwitchToCombat(bool pressed)
		{
			if (pressed)
			{
				m_timer.Reset();
				m_timer.IsRunning = false;

				ShowTimeComplete(this, EventArgs.Empty);
			}
		}

		private void ShowTimeComplete(object sender, EventArgs args)
		{
			MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Combat));
		}

		public int VisibleTime => m_visibletime;

		public VersusData P1 => m_p1;

		public VersusData P2 => m_p2;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_visibletime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly VersusData m_p1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly VersusData m_p2;

		private CountdownTimer m_timer;

		#endregion
	}
}