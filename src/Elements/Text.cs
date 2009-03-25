using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	class Text : Base
	{
		public Text(Collection collection, String name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds)
			: base(collection, name, datamap, sprites, animations, sounds)
		{
		}

		public override void Draw(Vector2 location)
		{
			Collection.Fonts.Print(DataMap.FontData, DataMap.Offset + location, DataMap.Text, null);
		}

		public override Boolean FinishedDrawing(Int32 tickcount)
		{
			return DataMap.DisplayTime == tickcount;
		}
	}
}