using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	class Collection
	{
		public Collection(Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds, Drawing.FontMap fontmap)
		{
			if (sprites == null) throw new ArgumentNullException("sprites");
			if (animations == null) throw new ArgumentNullException("animations");
			if (sounds == null) throw new ArgumentNullException("sounds");
			if (fontmap == null) throw new ArgumentNullException("fontmap");

			m_elements = new KeyedCollection<String, Elements.Base>(x => x.Name, StringComparer.OrdinalIgnoreCase);
			m_spritemanager = sprites;
			m_animationmanager = animations;
			m_soundmanager = sounds;
			m_fontmap = fontmap;
		}

		public Base Build(IO.TextSection section, String prefix)
		{
			if (section == null) throw new ArgumentNullException("section");
			if (prefix == null) throw new ArgumentNullException("prefix");

			return Build(prefix, section, prefix);
		}

		public Base Build(String name, IO.TextSection section, String prefix)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (section == null) throw new ArgumentNullException("section");
			if (prefix == null) throw new ArgumentNullException("prefix");

			DataMap datamap = new DataMap(section, prefix);

			Base element = null;

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

				case ElementType.None:
				default:
					element = new Base(this, name, datamap, SpriteManager, AnimationManager, SoundManager);
					break;

			}

			m_elements.Add(element);
			return element;
		}

		public Base GetElement(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			if (m_elements.Contains(name) == false) return null;

			return m_elements[name];
		}

		public Audio.Channel PlaySound(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			Elements.Base element = GetElement(name);
			if (element != null) return element.PlaySound();

			return null;
		}

		public void Update()
		{
			foreach (Base element in m_elements)
			{
				element.Update();
			}
		}

		public void Reset()
		{
			foreach (Elements.Base element in m_elements) element.Reset();
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

		public Drawing.FontMap Fonts
		{
			get { return m_fontmap; }
		}

		#region Fields

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[DebuggerDisplay(null, Name = "Elements")]
		readonly KeyedCollection<String, Base> m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.FontMap m_fontmap;

		#endregion
	}
}