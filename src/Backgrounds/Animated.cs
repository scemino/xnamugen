using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.Backgrounds
{
	internal class Animated : Base
	{
		public Animated(TextSection textsection, Drawing.SpriteManager spritemanager, Animations.AnimationManager animationmanager)
			: base(textsection)
		{
			if (spritemanager == null) throw new ArgumentNullException(nameof(spritemanager));
			if (animationmanager == null) throw new ArgumentNullException(nameof(animationmanager));

			m_spritemanager = spritemanager;
			m_animationmanager = animationmanager;
			m_animationnumber = textsection.GetAttribute("actionno", int.MinValue);
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
			var sprite = SpriteManager.GetSprite(AnimationManager.CurrentElement.SpriteId);
			if (sprite == null) return;

			Point tilestart;
			Point tileend;
			GetTileLength(sprite.Size, out tilestart, out tileend);

			var drawstate = SpriteManager.DrawState;
			drawstate.Reset();
			drawstate.Blending = Transparency;
			drawstate.Set(sprite);
			drawstate.AddData(CurrentLocation, null);

			if (palettefx != null) palettefx.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		public Drawing.SpriteManager SpriteManager => m_spritemanager;

		public Animations.AnimationManager AnimationManager => m_animationmanager;

		public int AnimationNumber => m_animationnumber;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_animationnumber;

		#endregion
	}
}