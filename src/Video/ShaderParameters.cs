using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Video
{
	internal class ShaderParameters
	{
		public ShaderParameters()
		{
			Reset();
		}

		public void Reset()
		{
			m_fontcolorindex = 0;
			m_fonttotalcolors = 0;

			m_afterimageuse = false;
			m_afterimageinvert = false;
			m_afterimagecolor = 0;
			m_afterimagepreadd = Vector3.Zero;
			m_afterimagecontrast = Vector3.Zero;
			m_afterimagepostadd = Vector3.Zero;
			m_afterimagepaladd = Vector3.Zero;
			m_afterimagepalmul = Vector3.Zero;
			m_afterimagenumber = 0;

			m_usepalfx = false;
			m_palfxadd = Vector3.Zero;
			m_palfxcolor = 0;
			m_palfxinvert = false;
			m_palfxmul = Vector3.Zero;
			m_palfxsinadd = Vector4.Zero;
			m_palfxtime = 0;

			m_shadowcolor = Vector4.Zero;
		}

		public int FontColorIndex
		{
			get => m_fontcolorindex;

			set { m_fontcolorindex = value; }
		}

		public int FontTotalColors
		{
			get => m_fonttotalcolors;

			set { m_fonttotalcolors = value; }
		}

		public bool AfterImageEnable
		{
			get => m_afterimageuse;

			set { m_afterimageuse = value; }
		}

		public bool AfterImageInvert
		{
			get => m_afterimageinvert;

			set { m_afterimageinvert = value; }
		}

		public float AfterImageColor
		{
			get => m_afterimagecolor;

			set { m_afterimagecolor = value; }
		}

		public Vector3 AfterImagePreAdd
		{
			get => m_afterimagepreadd;

			set { m_afterimagepreadd = value; }
		}

		public Vector3 AfterImageConstrast
		{
			get => m_afterimagecontrast;

			set { m_afterimagecontrast = value; }
		}

		public Vector3 AfterImagePostAdd
		{
			get => m_afterimagepostadd;

			set { m_afterimagepostadd = value; }
		}

		public Vector3 AfterImagePaletteAdd
		{
			get => m_afterimagepaladd;

			set { m_afterimagepaladd = value; }
		}

		public Vector3 AfterImagePaletteMultiply
		{
			get => m_afterimagepalmul;

			set { m_afterimagepalmul = value; }
		}

		public int AfterImageNumber
		{
			get => m_afterimagenumber;

			set { m_afterimagenumber = value; }
		}

		public bool PaletteFxEnable
		{
			get => m_usepalfx;

			set { m_usepalfx = value; }
		}

		public int PaletteFxTime
		{
			get => m_palfxtime;

			set { m_palfxtime = value; }
		}

		public bool PaletteFxInvert
		{
			get => m_palfxinvert;

			set { m_palfxinvert = value; }
		}

		public Vector3 PaletteFxAdd
		{
			get => m_palfxadd;

			set { m_palfxadd = value; }
		}

		public Vector3 PaletteFxMultiply
		{
			get => m_palfxmul;

			set { m_palfxmul = value; }
		}

		public Vector4 PaletteFxSinAdd
		{
			get => m_palfxsinadd;

			set { m_palfxsinadd = value; }
		}

		public float PaletteFxColor
		{
			get => m_palfxcolor;

			set { m_palfxcolor = value; }
		}

		public Vector4 ShadowColor
		{
			get => m_shadowcolor;

			set { m_shadowcolor = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_fontcolorindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_fonttotalcolors;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_afterimageuse;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_afterimageinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_afterimagecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_afterimagepreadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_afterimagecontrast;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_afterimagepostadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_afterimagepaladd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_afterimagepalmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_afterimagenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_usepalfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_palfxtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_palfxadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_palfxmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector4 m_palfxsinadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_palfxinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_palfxcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector4 m_shadowcolor;

		#endregion
	}
}