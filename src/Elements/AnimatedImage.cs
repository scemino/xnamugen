using Microsoft.Xna.Framework;

namespace xnaMugen.Elements
{
	internal class AnimatedImage : Base
	{
		public AnimatedImage(Collection collection, string name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations/*, Audio.SoundManager sounds*/)
			: base(collection, name, datamap, sprites, animations/*, sounds*/)
		{
			Reset();
		}

		public override void Draw(Vector2 location)
		{
			if (AnimationManager.HasAnimation(DataMap.AnimationNumber))
			{
				var element = AnimationManager.CurrentElement;
				if (element == null) return;

				var sprite = SpriteManager.GetSprite(element.SpriteId);
				if (sprite == null) return;

				var drawstate = SpriteManager.SetupDrawing(element.SpriteId, location, DataMap.Offset + element.Offset, DataMap.Scale, DataMap.Flip);
				drawstate.Blending = element.Blending;
				drawstate.Use();
			}
		}

		public override void Update()
		{
			base.Update();

			if (AnimationManager.HasAnimation(DataMap.AnimationNumber)) AnimationManager.Update();
		}

		public override void Reset()
		{
			base.Reset();

			if (AnimationManager.HasAnimation(DataMap.AnimationNumber)) AnimationManager.SetLocalAnimation(DataMap.AnimationNumber, 0);
		}

		public override bool FinishedDrawing(int tickcount)
		{
			if (AnimationManager.HasAnimation(DataMap.AnimationNumber) == false) return true;

			if (DataMap.DisplayTime == 0) return AnimationManager.IsAnimationFinished;

			return AnimationManager.TimeInAnimation == tickcount;
		}
	}
}