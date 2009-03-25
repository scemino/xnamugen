using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
	class VersusData
	{
		public VersusData(String prefix, TextSection textsection)
		{
			if (prefix == null) throw new ArgumentNullException("prefix");
			if (textsection == null) throw new ArgumentNullException("textsection");

			m_profile = null;
			m_portraitlocation = textsection.GetAttribute<Point>(prefix + "pos");
			m_portraitscale = textsection.GetAttribute<Vector2>(prefix + "scale");
			m_portraitflip = (textsection.GetAttribute<Int32>(prefix + "facing") < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			m_namelocation = textsection.GetAttribute<Point>(prefix + "name.pos");
			m_printdata = textsection.GetAttribute<PrintData>(prefix + "name.font");
		}

		public PlayerProfile Profile
		{
			get { return m_profile; }

			set { m_profile = value; }
		}

		public Point PortraitLocation
		{
			get { return m_portraitlocation; }
		}

		public Vector2 PortraitScale
		{
			get { return m_portraitscale; }
		}

		public SpriteEffects PortraitFlip
		{
			get { return m_portraitflip; }
		}

		public Point NameLocation
		{
			get { return m_namelocation; }
		}

		public PrintData NameFont
		{
			get { return m_printdata; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		PlayerProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_portraitlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_portraitscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteEffects m_portraitflip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_namelocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PrintData m_printdata;

		#endregion
	}
}