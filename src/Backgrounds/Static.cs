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
	class Static : Base
	{
		public Static(TextSection textsection, Drawing.SpriteManager spritemanager, Animations.AnimationManager animationmanager)
			: base(textsection, spritemanager, animationmanager)
		{
			m_spriteid = textsection.GetAttribute<SpriteId>("spriteno", SpriteId.Invalid);
		}

		protected override SpriteId DrawSpriteId
		{
			get { return m_spriteid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteId m_spriteid;

		#endregion
	}
}