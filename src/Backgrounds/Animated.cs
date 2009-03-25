using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Backgrounds
{
	class Animated : Base
	{
		public Animated(TextSection textsection, Drawing.SpriteManager spritemanager, Animations.AnimationManager animationmanager)
			: base(textsection)
		{
			if (spritemanager == null) throw new ArgumentNullException("spritemanager");
			if (animationmanager == null) throw new ArgumentNullException("animationmanager");

			m_spritemanager = spritemanager;
			m_animationmanager = animationmanager;
			m_animationnumber = textsection.GetAttribute<Int32>("actionno", Int32.MinValue);
		}

		public override void Reset()
		{
			base.Reset();

			AnimationManager.SetLocalAnimation(AnimationNumber, 0);

			SpriteManager.LoadSprites(AnimationManager.CurrentAnimation);
		}

		public override void Update()
		{
			AnimationManager.Update();
		}

		public override void Draw(Combat.PaletteFx palettefx)
		{
			Drawing.Sprite sprite = SpriteManager.GetSprite(AnimationManager.CurrentElement.SpriteId);
			if (sprite == null) return;

			Point tilestart;
			Point tileend;
			GetTileLength(sprite.Size, out tilestart, out tileend);

			Video.DrawState drawstate = SpriteManager.DrawState;
			drawstate.Reset();
			drawstate.Blending = Transparency;
			drawstate.Set(sprite);
			drawstate.AddData(CurrentLocation, null);

			if (palettefx != null) palettefx.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		public Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public Animations.AnimationManager AnimationManager
		{
			get { return m_animationmanager; }
		}

		public Int32 AnimationNumber
		{
			get { return m_animationnumber; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_animationnumber;

		#endregion
	}
}