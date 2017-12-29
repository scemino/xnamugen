using System;
using System.Diagnostics;
using xnaMugen.IO;
using xnaMugen.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
	internal class VersusData
	{
		public VersusData(string prefix, TextSection textsection)
		{
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));

			m_profile = null;
			m_portraitlocation = textsection.GetAttribute<Point>(prefix + "pos");
			m_portraitscale = textsection.GetAttribute<Vector2>(prefix + "scale");
			m_portraitflip = textsection.GetAttribute<int>(prefix + "facing") < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			m_namelocation = textsection.GetAttribute<Point>(prefix + "name.pos");
			m_printdata = textsection.GetAttribute<PrintData>(prefix + "name.font");
		}

		public PlayerProfile Profile
		{
			get => m_profile;

			set { m_profile = value; }
		}

		public Point PortraitLocation => m_portraitlocation;

		public Vector2 PortraitScale => m_portraitscale;

		public SpriteEffects PortraitFlip => m_portraitflip;

		public Point NameLocation => m_namelocation;

		public PrintData NameFont => m_printdata;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_portraitlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_portraitscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteEffects m_portraitflip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_namelocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_printdata;

		#endregion
	}
}