using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Backgrounds
{
    class Animated : Base
    {
        public Animated(TextSection textsection, Drawing.SpriteManager spritemanager, Animations.AnimationManager animationmanager)
            : base(textsection, spritemanager, animationmanager)
        {
            m_animationnumber = textsection.GetAttribute<Int32>("actionno", Int32.MinValue);
        }

        public override void Reset()
        {
            base.Reset();

            AnimationManager.SetLocalAnimation(AnimationNumber, 0);

            SpriteManager.LoadSprites(AnimationManager.CurrentAnimation);
        }

        public Int32 AnimationNumber
        {
            get { return m_animationnumber; }
        }

		public override Point TilingSpacing
		{
			get
			{
				Point spacing = base.TilingSpacing;

				if (spacing.Y == 0) spacing.Y = spacing.X;

				return spacing;
			}
		}

		protected override SpriteId DrawSpriteId
		{
			get 
			{
				if (AnimationManager.CurrentElement == null) return SpriteId.Invalid;

				return AnimationManager.CurrentElement.SpriteId; 
			}
		}

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Int32 m_animationnumber;

        #endregion
    }
}