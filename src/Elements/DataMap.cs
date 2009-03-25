using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Elements
{
	[DebuggerDisplay("Type = {Type}")]
	class DataMap
	{
		public DataMap(IO.TextSection section, String prefix)
		{
			if (section == null) throw new ArgumentNullException("section");
			if (prefix == null) throw new ArgumentNullException("prefix");

			m_prefix = prefix;

			m_animationnumber = section.GetAttribute<Int32>(prefix + ".anim", -1);
			m_spriteid = section.GetAttribute<SpriteId>(prefix + ".spr", SpriteId.Invalid);
			m_fontdata = section.GetAttribute<Drawing.PrintData>(prefix + ".font", new Drawing.PrintData());
			m_text = section.GetAttribute<String>(prefix + ".text", null);
			m_soundid = section.GetAttribute<SoundId>(prefix + ".snd", SoundId.Invalid);
			m_soundtime = section.GetAttribute<Int32>(prefix + ".sndtime", 0);
			m_offset = (Vector2)section.GetAttribute<Point>(prefix + ".offset", new Point(0, 0));
			m_displaytime = section.GetAttribute<Int32>(prefix + ".displaytime", 0);

			Int32 hflip = section.GetAttribute<Int32>(prefix + ".facing", 0);
			m_flip |= (hflip >= 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Int32 vflip = section.GetAttribute<Int32>(prefix + ".vfacing", 0);
			m_flip |= (vflip >= 0) ? SpriteEffects.None : SpriteEffects.FlipVertically;

			m_layernumber = section.GetAttribute<Int32>(prefix + ".layerno", 0);
			m_scale = section.GetAttribute<Vector2>(prefix + ".scale", Vector2.One);

			if (AnimationNumber > -1)
			{
				m_type = ElementType.Animation;
			}
			else if (SpriteId != SpriteId.Invalid)
			{
				m_type = ElementType.Static;
			}
			else if (FontData.IsValid == true)
			{
				m_type = ElementType.Text;
			}
			else
			{
				m_type = ElementType.None;
			}
		}

		public ElementType Type
		{
			get { return m_type; }
		}

		public String Prefix
		{
			get { return m_prefix; }
		}

		public Int32 AnimationNumber
		{
			get { return m_animationnumber; }
		}

		public SpriteId SpriteId
		{
			get { return m_spriteid; }
		}

		public Drawing.PrintData FontData
		{
			get { return m_fontdata; }
		}

		public String Text
		{
			get { return m_text; }
		}

		public SoundId SoundId
		{
			get { return m_soundid; }
		}

		public Int32 SoundTime
		{
			get { return m_soundtime; }
		}

		public Vector2 Offset
		{
			get { return m_offset; }
		}

		public Int32 DisplayTime
		{
			get { return m_displaytime; }
		}

		public SpriteEffects Flip
		{
			get { return m_flip; }
		}

		public Int32 LayerNumber
		{
			get { return m_layernumber; }
		}

		public Vector2 Scale
		{
			get { return m_scale; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_prefix;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ElementType m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteId m_spriteid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.PrintData m_fontdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundId m_soundid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_soundtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_displaytime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_layernumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_scale;

		#endregion
	}
}