using System;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	internal class Collection
	{
		public Collection(Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds, Drawing.FontMap fontmap)
		{
			if (sprites == null) throw new ArgumentNullException(nameof(sprites));
			if (animations == null) throw new ArgumentNullException(nameof(animations));
			if (sounds == null) throw new ArgumentNullException("sounds");
			if (fontmap == null) throw new ArgumentNullException(nameof(fontmap));

			m_elements = new KeyedCollection<string, Base>(x => x.Name, StringComparer.OrdinalIgnoreCase);
			m_spritemanager = sprites;
			m_animationmanager = animations;
			m_soundmanager = sounds;
			m_fontmap = fontmap;
		}

		public Base Build(IO.TextSection section, string prefix)
		{
			if (section == null) throw new ArgumentNullException(nameof(section));
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));

			return Build(prefix, section, prefix);
		}

		public Base Build(string name, IO.TextSection section, string prefix)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (section == null) throw new ArgumentNullException(nameof(section));
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));

			var datamap = new DataMap(section, prefix);

			Base element;

			switch (datamap.Type)
			{
				case ElementType.Animation:
					element = new AnimatedImage(this, name, datamap, SpriteManager, AnimationManager, SoundManager);
					break;

				case ElementType.Static:
					element = new StaticImage(this, name, datamap, SpriteManager, AnimationManager, SoundManager);
					break;

				case ElementType.Text:
					element = new Text(this, name, datamap, SpriteManager, AnimationManager, SoundManager);
					break;

				default:
					element = new Base(this, name, datamap, SpriteManager, AnimationManager, SoundManager);
					break;

			}

			m_elements.Add(element);
			return element;
		}

		public Base GetElement(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (m_elements.Contains(name) == false) return null;

			return m_elements[name];
		}

		public Audio.Channel PlaySound(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			var element = GetElement(name);
			if (element != null) return element.PlaySound();

			return null;
		}

		public void Update()
		{
			foreach (var element in m_elements)
			{
				element.Update();
			}
		}

		public void Reset()
		{
			foreach (var element in m_elements) element.Reset();
		}

		public Audio.SoundManager SoundManager
		{
			get { return m_soundmanager; }
		}

		public Drawing.SpriteManager SpriteManager => m_spritemanager;

		public Animations.AnimationManager AnimationManager => m_animationmanager;

		public Drawing.FontMap Fonts => m_fontmap;

		#region Fields

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[DebuggerDisplay(null, Name = "Elements")]
		private readonly KeyedCollection<string, Base> m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.FontMap m_fontmap;

		#endregion
	}
}