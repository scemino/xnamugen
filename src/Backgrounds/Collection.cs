using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Backgrounds
{
	/// <summary>
	/// A collection of backgrounds that are updated and drawn as one.
	/// </summary>
	[DebuggerDisplay("Count = {m_backgrounds.Count}")]
	class Collection
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="spritemanager">The xnaMugen.Sprites.SpriteManager used by all backgrounds in this collection.</param>
		/// <param name="animationmanager">The xnaMugen.Animations.AnimationManager used by all backgrounds in this collection.</param>
		public Collection(Drawing.SpriteManager spritemanager, Animations.AnimationManager animationmanager)
		{
			if (spritemanager == null) throw new ArgumentNullException("spritemanager");
			if (animationmanager == null) throw new ArgumentNullException("animationmanager");

			m_backgrounds = new List<Base>();
			m_spritemanager = spritemanager;
			m_animationmanager = animationmanager;
		}

		/// <summary>
		/// Determines whether a background can be created from the given xnaMugen.IO.TextSection.
		/// </summary>
		/// <param name="section">A xnaMugen.IO.TextSection.</param>
		/// <returns>true if a background can be created; false otherwise.</returns>
		public Boolean CanCreateBackground(TextSection section)
		{
			if (section == null) throw new ArgumentNullException("section");

			BackgroundType bgtype = section.GetAttribute<BackgroundType>("type");

			return bgtype != BackgroundType.None;
		}

		/// <summary>
		/// Creates a new background as per of the collection, initializes from the given xnaMugen.IO.TextSection.
		/// </summary>
		/// <param name="section">The text section used to create and initialize the created background.</param>
		/// <returns>The created background, if it could be created; null otherwise.</returns>
		public void CreateBackground(TextSection section)
		{
			if (section == null) throw new ArgumentNullException("section");

			BackgroundType bgtype = section.GetAttribute<BackgroundType>("type");
			Base background = null;

			switch (bgtype)
			{
				case BackgroundType.Static:
					background = new Static(section, m_spritemanager.Clone());
					break;

				case BackgroundType.Parallax:
					background = new Parallax(section, m_spritemanager.Clone());
					break;

				case BackgroundType.Animated:
					background = new Animated(section, m_spritemanager.Clone(), m_animationmanager.Clone());
					break;

				case BackgroundType.None:
				default:
					Log.Write(LogLevel.Error, LogSystem.BackgroundCollection, "Cannot create background with TextSection: {0}", section);
					return;
			}

			background.Reset();
			m_backgrounds.Add(background);
		}

		/// <summary>
		/// Resets all backgrounds that are a part of this collection.
		/// </summary>
		public void Reset()
		{
			foreach (Base background in this) background.Reset();
		}

		/// <summary>
		/// Updates all unpaused backgrounds that are a part of this collection.
		/// </summary>
		public void Update()
		{
			foreach (Base background in this)
			{
				if (background.IsPaused == false) background.Update();
			}
		}

		/// <summary>
		/// Draws all visible backgrounds that are a part of this collection.
		/// </summary>
		public void Draw()
		{
			foreach (Base background in this)
			{
				if (background.IsVisible == true) background.Draw(null);
			}
		}

		/// <summary>
		/// Draws all visible backgrounds in a given layer that are a part of this collection using the given palette information.
		/// </summary>
		/// <param name="layer">The layer of backgrounds to draw.</param>
		/// <param name="palettefx">The palette information used in drawing.</param>
		public void Draw(BackgroundLayer layer, Combat.PaletteFx palettefx)
		{
			foreach (Backgrounds.Base background in this)
			{
				if (background.Layer == layer && background.IsVisible == true) background.Draw(palettefx);
			}
		}

		public List<Base>.Enumerator GetEnumerator()
		{
			return m_backgrounds.GetEnumerator();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<Base> m_backgrounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		#endregion
	}
}