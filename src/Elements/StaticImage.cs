using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	class StaticImage : Base
	{
		public StaticImage(Collection collection, String name, DataMap datamap, Drawing.SpriteManager sprites, Animations.AnimationManager animations, Audio.SoundManager sounds)
			: base(collection, name, datamap, sprites, animations, sounds)
		{
		}

		public override void Draw(Vector2 location)
		{
			SpriteManager.Draw(DataMap.SpriteId, location, DataMap.Offset, DataMap.Scale, DataMap.Flip);
		}

		public override Boolean FinishedDrawing(Int32 tickcount)
		{
			return DataMap.DisplayTime == tickcount;
		}
	}
}