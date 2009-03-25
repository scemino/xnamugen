using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	abstract class NonCombatScreen : Screen
	{
		protected NonCombatScreen(MenuSystem menusystem, TextSection textsection, String spritepath, String animationpath, String soundpath)
			: base(menusystem)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");
			if (spritepath == null) throw new ArgumentNullException("spritepath");
			if (animationpath == null) throw new ArgumentNullException("animationpath");
			if (soundpath == null) throw new ArgumentNullException("soundpath");

			m_soundmanager = MenuSystem.GetSubSystem<Audio.SoundSystem>().CreateManager(soundpath);
			m_spritemanager = MenuSystem.GetSubSystem<Drawing.SpriteSystem>().CreateManager(spritepath);
			m_animationmanager = MenuSystem.GetSubSystem<Animations.AnimationSystem>().CreateManager(animationpath);
			m_backgrounds = new Backgrounds.Collection(m_spritemanager.Clone(), m_animationmanager.Clone());
			m_fadeintime = textsection.GetAttribute<Int32>("fadein.time");
			m_fadeouttime = textsection.GetAttribute<Int32>("fadeout.time");

			SpriteManager.LoadAllSprites();
		}

		public void LoadBackgrounds(String prefix, TextFile textfile)
		{
			if (prefix == null) throw new ArgumentNullException("prefix");
			if (textfile == null) throw new ArgumentNullException("textfile");

			Regex regex = new Regex("^" + prefix + "BG (.*)$", RegexOptions.IgnoreCase);

			foreach (TextSection section in textfile)
			{
				Match match = regex.Match(section.Title);
				if (match.Success == true)
				{
					m_backgrounds.CreateBackground(section);
				}
			}

			m_backgrounds.Reset();
		}

		public override void Reset()
		{
			base.Reset();

			Backgrounds.Reset();
		}

		public override void Update(GameTime gametime)
		{
			base.Update(gametime);

			Backgrounds.Update();
		}

		public override void Draw(Boolean debugdraw)
		{
			base.Draw(debugdraw);

			Point shift = new Point(Mugen.ScreenSize.X / 2, 0);

			MenuSystem.GetSubSystem<Video.VideoSystem>().CameraShift += shift;
			m_backgrounds.Draw();
			MenuSystem.GetSubSystem<Video.VideoSystem>().CameraShift -= shift;
		}

		public Backgrounds.Collection Backgrounds
		{
			get { return m_backgrounds; }
		}

		public Audio.SoundManager SoundManager
		{
			get { return m_soundmanager; }
		}

		public Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public Animations.AnimationManager AnimationManager
		{
			get { return m_animationmanager; }
		}

		public override Int32 FadeInTime
		{
			get { return m_fadeintime; }
		}

		public override Int32 FadeOutTime
		{
			get { return m_fadeouttime; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Backgrounds.Collection m_backgrounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_fadeintime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_fadeouttime;

		#endregion
	}
}