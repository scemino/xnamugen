using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	class AnimatedImage : Base
	{
		public AnimatedImage(Collection collection, String name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds)
			: base(collection, name, datamap, sprites, animations, sounds)
		{
			Reset();
		}

		public override void Draw(Vector2 location)
		{
			if (AnimationManager.HasAnimation(DataMap.AnimationNumber) == true)
			{
				Animations.AnimationElement element = AnimationManager.CurrentElement;
				if (element == null) return;

				Drawing.Sprite sprite = SpriteManager.GetSprite(element.SpriteId);
				if (sprite == null) return;

				Video.DrawState drawstate = SpriteManager.SetupDrawing(element.SpriteId, location, DataMap.Offset + element.Offset, DataMap.Scale, DataMap.Flip);
				drawstate.Blending = element.Blending;
				drawstate.Use();
			}
		}

		public override void Update()
		{
			base.Update();

			if (AnimationManager.HasAnimation(DataMap.AnimationNumber) == true) AnimationManager.Update();
		}

		public override void Reset()
		{
			base.Reset();

			if (AnimationManager.HasAnimation(DataMap.AnimationNumber) == true) AnimationManager.SetLocalAnimation(DataMap.AnimationNumber, 0);
		}

		public override Boolean FinishedDrawing(Int32 tickcount)
		{
			if (AnimationManager.HasAnimation(DataMap.AnimationNumber) == false) return true;

			if (DataMap.DisplayTime == 0) return AnimationManager.IsAnimationFinished;

			return AnimationManager.TimeInAnimation == tickcount;
		}
	}
}