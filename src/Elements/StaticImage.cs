using Microsoft.Xna.Framework;

namespace xnaMugen.Elements
{
	internal class StaticImage : Base
	{
		public StaticImage(Collection collection, string name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds)
			: base(collection, name, datamap, sprites, animations, sounds)
		{
		}

		public override void Draw(Vector2 location)
		{
			SpriteManager.Draw(DataMap.SpriteId, location, DataMap.Offset, DataMap.Scale, DataMap.Flip);
		}

		public override bool FinishedDrawing(int tickcount)
		{
			return DataMap.DisplayTime == tickcount;
		}
	}
}