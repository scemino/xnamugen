using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.Video
{
	struct FRect
	{
		public Single X { get; set; }
		public Single Y { get; set; }
		public Single Width { get; set; }
		public Single Height { get; set; }

		public Single Left { get { return X; } }
		public Single Right { get { return X + Width; } }
		public Single Top { get { return Y; } }
		public Single Bottom { get { return Y + Height; } }
	}
}