using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
	internal abstract class NonCombatScreen : Screen
	{
		protected NonCombatScreen(MenuSystem menusystem, TextSection textsection, string spritepath, string animationpath, string soundpath)
			: base(menusystem)
		{
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));
			if (spritepath == null) throw new ArgumentNullException(nameof(spritepath));
			if (animationpath == null) throw new ArgumentNullException(nameof(animationpath));
			if (soundpath == null) throw new ArgumentNullException(nameof(soundpath));

			//m_soundmanager = MenuSystem.GetSubSystem<Audio.SoundSystem>().CreateManager(soundpath);
			m_spritemanager = MenuSystem.GetSubSystem<Drawing.SpriteSystem>().CreateManager(spritepath);
			m_animationmanager = MenuSystem.GetSubSystem<Animations.AnimationSystem>().CreateManager(animationpath);
			m_backgrounds = new Backgrounds.Collection(m_spritemanager.Clone(), m_animationmanager.Clone());
			m_fadeintime = textsection.GetAttribute<int>("fadein.time");
			m_fadeouttime = textsection.GetAttribute<int>("fadeout.time");

			SpriteManager.LoadAllSprites();
		}

		public void LoadBackgrounds(string prefix, TextFile textfile)
		{
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));
			if (textfile == null) throw new ArgumentNullException(nameof(textfile));

			var regex = new Regex("^" + prefix + "BG (.*)$", RegexOptions.IgnoreCase);

			foreach (var section in textfile)
			{
				var match = regex.Match(section.Title);
				if (match.Success)
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

		public override void Draw(bool debugdraw)
		{
			base.Draw(debugdraw);

			var shift = new Point(Mugen.ScreenSize.X / 2, 0);

			MenuSystem.GetSubSystem<Video.VideoSystem>().CameraShift += shift;
			m_backgrounds.Draw();
			MenuSystem.GetSubSystem<Video.VideoSystem>().CameraShift -= shift;
		}

		public Backgrounds.Collection Backgrounds => m_backgrounds;

		//public Audio.SoundManager SoundManager
		//{
		//	get { return m_soundmanager; }
		//}

		public Drawing.SpriteManager SpriteManager => m_spritemanager;

		public Animations.AnimationManager AnimationManager => m_animationmanager;

		public override int FadeInTime => m_fadeintime;

		public override int FadeOutTime => m_fadeouttime;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Backgrounds.Collection m_backgrounds;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		//readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_fadeintime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_fadeouttime;

		#endregion
	}
}