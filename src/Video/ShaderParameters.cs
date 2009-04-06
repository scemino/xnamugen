using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.Video
{
	class ShaderParameters
	{
		public ShaderParameters()
		{
			Reset();
		}

		public void Reset()
		{
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
		}

		public Boolean AfterImageEnable
		{
			get { return m_afterimageuse; }

			set { m_afterimageuse = value; }
		}

		public Boolean AfterImageInvert
		{
			get { return m_afterimageinvert; }

			set { m_afterimageinvert = value; }
		}

		public Single AfterImageColor
		{
			get { return m_afterimagecolor; }

			set { m_afterimagecolor = value; }
		}

		public Vector3 AfterImagePreAdd
		{
			get { return m_afterimagepreadd; }

			set { m_afterimagepreadd = value; }
		}

		public Vector3 AfterImageConstrast
		{
			get { return m_afterimagecontrast; }

			set { m_afterimagecontrast = value; }
		}

		public Vector3 AfterImagePostAdd
		{
			get { return m_afterimagepostadd; }

			set { m_afterimagepostadd = value; }
		}

		public Vector3 AfterImagePaletteAdd
		{
			get { return m_afterimagepaladd; }

			set { m_afterimagepaladd = value; }
		}

		public Vector3 AfterImagePaletteMultiply
		{
			get { return m_afterimagepalmul; }

			set { m_afterimagepalmul = value; }
		}

		public Int32 AfterImageNumber
		{
			get { return m_afterimagenumber; }

			set { m_afterimagenumber = value; }
		}

		public Boolean PaletteFxEnable
		{
			get { return m_usepalfx; }

			set { m_usepalfx = value; }
		}

		public Int32 PaletteFxTime
		{
			get { return m_palfxtime; }

			set { m_palfxtime = value; }
		}

		public Boolean PaletteFxInvert
		{
			get { return m_palfxinvert; }

			set { m_palfxinvert = value; }
		}

		public Vector3 PaletteFxAdd
		{
			get { return m_palfxadd; }

			set { m_palfxadd = value; }
		}

		public Vector3 PaletteFxMultiply
		{
			get { return m_palfxmul; }

			set { m_palfxmul = value; }
		}

		public Vector4 PaletteFxSinAdd
		{
			get { return m_palfxsinadd; }

			set { m_palfxsinadd = value; }
		}

		public Single PaletteFxColor
		{
			get { return m_palfxcolor; }

			set { m_palfxcolor = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_afterimageuse;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_afterimageinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_afterimagecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_afterimagepreadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_afterimagecontrast;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_afterimagepostadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_afterimagepaladd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_afterimagepalmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_afterimagenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_usepalfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_palfxtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_palfxadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_palfxmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector4 m_palfxsinadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_palfxinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_palfxcolor;

		#endregion
	}
}