using Microsoft.Xna.Framework;

namespace xnaMugen.Elements
{
	internal class Text : Base
	{
		public Text(Collection collection, string name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds)
			: base(collection, name, datamap, sprites, animations, sounds)
		{
		}

		public override void Draw(Vector2 location)
		{
			Collection.Fonts.Print(DataMap.FontData, DataMap.Offset + location, DataMap.Text, null);
		}

		public override bool FinishedDrawing(int tickcount)
		{
			return DataMap.DisplayTime == tickcount;
		}
	}
}