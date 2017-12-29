using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class PaletteFx
	{
		public PaletteFx()
		{
			Reset();
		}

		public void Reset()
		{
			m_totaltime = 0;
			m_time = 0;
			m_add = Vector3.Zero;
			m_mul = Vector3.Zero;
			m_sinadd = new Vector4(0, 0, 0, 1);
			m_invert = false;
			m_basecolor = 1;
			m_isactive = false;
		}

		public void Update()
		{
			if (IsActive && (TotalTime == -1 || TotalTime > 0 && Time < TotalTime))
			{
				++m_time;
			}
			else
			{
				Reset();
			}
		}

		public void SetShader(Video.ShaderParameters shader)
		{
			if (shader == null) throw new ArgumentNullException(nameof(shader));

			if (IsActive)
			{
				shader.PaletteFxEnable = true;
				shader.PaletteFxAdd = Add;
				shader.PaletteFxColor = BaseColor;
				shader.PaletteFxInvert = Invert;
				shader.PaletteFxMultiply = Mul;
				shader.PaletteFxSinAdd = SinAdd;
				shader.PaletteFxTime = Time;
			}
			else
			{
				shader.PaletteFxEnable = false;
			}
		}

		public void Set(int totaltime, Vector3 add, Vector3 mul, Vector4 sinadd, bool invert, float basecolor)
		{
			m_totaltime = totaltime;
			m_time = 0;
			m_add = Clamp(add);
			m_mul = Clamp(mul);
			m_sinadd = new Vector4(Clamp(new Vector3(sinadd.X, sinadd.Y, sinadd.Z)), sinadd.W);
			m_invert = invert;
			m_basecolor = Misc.Clamp(basecolor, 0.0f, 1.0f);

			m_isactive = true;
		}

		private static Vector3 Clamp(Vector3 input)
		{
			return input / 255.0f;
		}

		public bool IsActive => m_isactive;

		public int TotalTime => m_totaltime;

		public int Time => m_time;

		public Vector3 Add => m_add;

		public Vector3 Mul => m_mul;

		public Vector4 SinAdd => m_sinadd;

		public bool Invert => m_invert;

		public float BaseColor => m_basecolor;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_totaltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_add;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_mul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_basecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector4 m_sinadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_invert;

		#endregion
	}
}