using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
	class PaletteFx
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
			if (IsActive == true && (TotalTime == -1 || (TotalTime > 0 && Time < TotalTime)))
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
			if (shader == null) throw new ArgumentNullException("shader");

			if (IsActive == true)
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

		public void Set(Int32 totaltime, Vector3 add, Vector3 mul, Vector4 sinadd, Boolean invert, Single basecolor)
		{
			m_totaltime = totaltime;
			m_time = 1;
			m_add = Clamp(add);
			m_mul = Clamp(mul);
			m_sinadd = new Vector4(Clamp(new Vector3(sinadd.X, sinadd.Y, sinadd.Z)), sinadd.W);
			m_invert = invert;
			m_basecolor = Misc.Clamp(basecolor, 0.0f, 1.0f);

			m_isactive = true;
		}

		static Vector3 Clamp(Vector3 input)
		{
			return input / 255.0f;
		}

		public Boolean IsActive
		{
			get { return m_isactive; }
		}

		public Int32 TotalTime
		{
			get { return m_totaltime; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		public Vector3 Add
		{
			get { return m_add; }
		}

		public Vector3 Mul
		{
			get { return m_mul; }
		}

		public Vector4 SinAdd
		{
			get { return m_sinadd; }
		}

		public Boolean Invert
		{
			get { return m_invert; }
		}

		public Single BaseColor
		{
			get { return m_basecolor; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_totaltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_add;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_mul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_basecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector4 m_sinadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_invert;

		#endregion
	}
}