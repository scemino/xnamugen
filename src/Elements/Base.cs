using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	class Base
	{
		public Base(Collection collection, String name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			if (name == null) throw new ArgumentNullException("name");
			if (datamap == null) throw new ArgumentNullException("datamap");
			if (sprites == null) throw new ArgumentNullException("sprites");
			if (animations == null) throw new ArgumentNullException("animations");
			if (sounds == null) throw new ArgumentNullException("sounds");

			m_collection = collection;
			m_name = name;
			m_data = datamap;
			m_spritemanager = sprites;
			m_animationmanager = animations.Clone();
			m_soundmanager = sounds;
		}

		public virtual void Draw(Vector2 location)
		{
		}

		public virtual Boolean FinishedDrawing(Int32 tickcount)
		{
			return true;
		}

		public virtual void Update()
		{
		}

		public virtual void Reset()
		{
		}

		public Audio.Channel PlaySound()
		{
			return m_soundmanager.Play(DataMap.SoundId);
		}

		public Collection Collection
		{
			get { return m_collection; }
		}

		public String Name
		{
			get { return m_name; }
		}

		public DataMap DataMap
		{
			get { return m_data; }
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

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Collection m_collection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly DataMap m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		#endregion
	}
}