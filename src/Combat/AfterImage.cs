using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	internal struct AfterImageSubdata
	{
		public AfterImageSubdata(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			m_location = entity.CurrentLocation;
			m_element = entity.AnimationManager.CurrentElement;
			m_flip = entity.GetDrawFlip();
            m_scale = entity.CurrentScale;
			m_blending = entity.Transparency == new Blending() ? m_element.Blending : entity.Transparency;
			m_facing = entity.CurrentFacing;
			m_drawangle = entity.AngleDraw ? Misc.FaceScalar(entity.CurrentFacing, -entity.DrawingAngle) : 0;

            if (entity is Character) m_scale *= (entity as Character).DrawScale;
		}

		public Animations.AnimationElement Element => m_element;

		public Vector2 Location => m_location;

		public SpriteEffects Flip => m_flip;

		public Vector2 Scale => m_scale;

		public Blending Transparency => m_blending;

		public Facing Facing => m_facing;

		public float DrawAngle => m_drawangle;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationElement m_element;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteEffects m_flip;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Facing m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_drawangle;

		#endregion
	}

	internal class AfterImage
	{
		public AfterImage(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			m_entity = entity;
			m_imagedata = new LinkedList<AfterImageSubdata>();
			m_countdown = new Dictionary<int, int>();

			Reset();
		}

		public void ModifyDisplayTime(int time)
		{
			DisplayTime = time;
			m_countdown.Clear();
		}

		public void Reset()
		{
			m_imagedata.Clear();
			m_countdown.Clear();
			m_isactive = false;
			m_displaytime = 0;
			m_numberofframes = 0;
			m_basecolor = 0;
			m_invertcolor = false;
			m_preadd = Vector3.Zero;
			m_constrast = Vector3.Zero;
			m_postadd = Vector3.Zero;
			m_palettecoloradd = Vector3.Zero;
			m_palettecolormul = Vector3.Zero;
			m_timegap = 0;
			m_framegap = 0;
			m_transparency = null;
			m_timegapcounter = 0;
			m_firstcreated = false;
		}

		public void Update()
		{
			if (IsActive == false) return;

			++m_timegapcounter;
			if (m_timegapcounter >= m_timegap)
			{
				m_firstcreated = true;
				m_timegapcounter = 0;
				m_imagedata.AddLast(new AfterImageSubdata(m_entity));

				while (m_imagedata.Count > m_numberofframes) m_imagedata.RemoveFirst();
			}

			var imageIndex = 0;
			var isactive = false;
			foreach (var data in m_imagedata)
			{
				if (TimeCheck_Update(imageIndex)) isactive = true;
				++imageIndex;
			}

			if (isactive == false && m_firstcreated)
			{
				Reset();
			}
		}

		private bool TimeCheck_Update(int image_index)
		{
			if (DisplayTime == -1) return true;

			if (m_countdown.ContainsKey(image_index))
			{
				if (m_countdown[image_index] <= 0) return false;

				--m_countdown[image_index];
			}
			else
			{
				if (DisplayTime <= 0) return false;
				m_countdown.Add(image_index, DisplayTime);
			}

			return true;
		}

		private bool TimeCheck(int image_index)
		{
			if (DisplayTime == -1) return true;

			if (m_countdown.ContainsKey(image_index))
			{
				if (m_countdown[image_index] <= 0) return false;
			}

			return true;
		}

		private bool FrameGapCheck(int index)
		{
			if (index <= 0) return false;

			return index % m_framegap == 0;
		}

		public void Draw()
		{
			if (IsActive == false) return;

			var index = m_imagedata.Count;
			foreach (var data in m_imagedata)
			{
				--index;

				if (FrameGapCheck(index) == false) continue;
				if (TimeCheck(index) == false) continue;

				var sprite = m_entity.SpriteManager.GetSprite(data.Element.SpriteId);
				if (sprite == null) continue;

				var drawlocation = data.Location + new Vector2(Mugen.ScreenSize.X / 2, m_entity.Engine.Stage.ZOffset);
				var drawoffset = Misc.GetOffset(Vector2.Zero, data.Facing, data.Element.Offset);

				var drawstate = m_entity.SpriteManager.SetupDrawing(data.Element.SpriteId, drawlocation, drawoffset, data.Scale, data.Flip);

				drawstate.Rotation = data.DrawAngle;
				drawstate.Blending = Transparency ?? data.Transparency;

				drawstate.ShaderParameters.AfterImageEnable = true;
				drawstate.ShaderParameters.AfterImageColor = m_basecolor;
				drawstate.ShaderParameters.AfterImageConstrast = m_constrast;
				drawstate.ShaderParameters.AfterImageInvert = m_invertcolor;
				drawstate.ShaderParameters.AfterImageNumber = index;
				drawstate.ShaderParameters.AfterImagePaletteAdd = m_palettecoloradd;
				drawstate.ShaderParameters.AfterImagePaletteMultiply = m_palettecolormul;
				drawstate.ShaderParameters.AfterImagePostAdd = m_postadd;
				drawstate.ShaderParameters.AfterImagePreAdd = m_preadd;

				drawstate.Use();
			}
		}

		public bool IsActive
		{
			get => m_isactive;

			set { m_isactive = value; }
		}

		public int DisplayTime
		{
			get => m_displaytime;

			set { m_displaytime = value; }
		}

		public int NumberOfFrames
		{
			get => m_numberofframes;

			set { m_numberofframes = value; }
		}

		public float BaseColor
		{
			get => m_basecolor;

			set { m_basecolor = Misc.Clamp(value, 0.0f, 1.0f); }
		}

		public bool InvertColor
		{
			get => m_invertcolor;

			set { m_invertcolor = value; }
		}

		public Vector3 ColorPreAdd
		{
			get => m_preadd;

			set { m_preadd = value; }
		}

		public Vector3 ColorContrast
		{
			get => m_constrast;

			set { m_constrast = value; }
		}

		public Vector3 ColorPostAdd
		{
			get => m_postadd;

			set { m_postadd = value; }
		}

		public Vector3 ColorPaletteAdd
		{
			get => m_palettecoloradd;

			set { m_palettecoloradd = value; }
		}

		public Vector3 ColorPaletteMultiply
		{
			get => m_palettecolormul;

			set { m_palettecolormul = value; }
		}

		public int TimeGap
		{
			get => m_timegap;

			set { m_timegap = value; }
		}

		public int FrameGap
		{
			get => m_framegap;

			set { m_framegap = value; }
		}

		public Blending? Transparency
		{
			get => m_transparency;

			set { m_transparency = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Entity m_entity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly LinkedList<AfterImageSubdata> m_imagedata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<int, int> m_countdown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_displaytime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_numberofframes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_basecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_invertcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_preadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_constrast;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_postadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_palettecoloradd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_palettecolormul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_timegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_framegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Blending? m_transparency;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_timegapcounter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_firstcreated;

		#endregion
	}
}