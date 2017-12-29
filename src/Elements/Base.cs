using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Elements
{
	internal class Base
	{
		public Base(Collection collection, string name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations/*, Audio.SoundManager sounds*/)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (datamap == null) throw new ArgumentNullException(nameof(datamap));
			if (sprites == null) throw new ArgumentNullException(nameof(sprites));
			if (animations == null) throw new ArgumentNullException(nameof(animations));
			//if (sounds == null) throw new ArgumentNullException("sounds");

			m_collection = collection;
			m_name = name;
			m_data = datamap;
			m_spritemanager = sprites;
			m_animationmanager = animations.Clone();
			//m_soundmanager = sounds;
		}

		public virtual void Draw(Vector2 location)
		{
		}

		public virtual bool FinishedDrawing(int tickcount)
		{
			return true;
		}

		public virtual void Update()
		{
		}

		public virtual void Reset()
		{
		}

		//public Audio.Channel PlaySound()
		//{
		//	return m_soundmanager.Play(DataMap.SoundId);
		//}

		public Collection Collection => m_collection;

		public string Name => m_name;

		public DataMap DataMap => m_data;

		//public Audio.SoundManager SoundManager
		//{
		//	get { return m_soundmanager; }
		//}

		public Drawing.SpriteManager SpriteManager => m_spritemanager;

		public Animations.AnimationManager AnimationManager => m_animationmanager;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Collection m_collection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly DataMap m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		//readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		#endregion
	}
}