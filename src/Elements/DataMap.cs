using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Elements
{
	[DebuggerDisplay("Type = {" + nameof(Type) + "}")]
	internal class DataMap
	{
		public DataMap(IO.TextSection section, string prefix)
		{
			if (section == null) throw new ArgumentNullException(nameof(section));
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));

			m_prefix = prefix;

			m_animationnumber = section.GetAttribute(prefix + ".anim", -1);
			m_spriteid = section.GetAttribute(prefix + ".spr", SpriteId.Invalid);
			m_fontdata = section.GetAttribute(prefix + ".font", new Drawing.PrintData());
			m_text = section.GetAttribute<string>(prefix + ".text", null);
			m_soundid = section.GetAttribute(prefix + ".snd", SoundId.Invalid);
			m_soundtime = section.GetAttribute(prefix + ".sndtime", 0);
			m_offset = (Vector2)section.GetAttribute(prefix + ".offset", new Point(0, 0));
			m_displaytime = section.GetAttribute(prefix + ".displaytime", 0);

			var hflip = section.GetAttribute(prefix + ".facing", 0);
			m_flip |= hflip >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			var vflip = section.GetAttribute(prefix + ".vfacing", 0);
			m_flip |= vflip >= 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

			m_layernumber = section.GetAttribute(prefix + ".layerno", 0);
			m_scale = section.GetAttribute(prefix + ".scale", Vector2.One);

			if (AnimationNumber > -1)
			{
				m_type = ElementType.Animation;
			}
			else if (SpriteId != SpriteId.Invalid)
			{
				m_type = ElementType.Static;
			}
			else if (FontData.IsValid)
			{
				m_type = ElementType.Text;
			}
			else
			{
				m_type = ElementType.None;
			}
		}

		public ElementType Type => m_type;

		public string Prefix => m_prefix;

		public int AnimationNumber => m_animationnumber;

		public SpriteId SpriteId => m_spriteid;

		public Drawing.PrintData FontData => m_fontdata;

		public string Text => m_text;

		public SoundId SoundId => m_soundid;

		public int SoundTime => m_soundtime;

		public Vector2 Offset => m_offset;

		public int DisplayTime => m_displaytime;

		public SpriteEffects Flip => m_flip;

		public int LayerNumber => m_layernumber;

		public Vector2 Scale => m_scale;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_prefix;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ElementType m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteId m_spriteid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.PrintData m_fontdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_soundtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_displaytime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_layernumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_scale;

		#endregion
	}
}