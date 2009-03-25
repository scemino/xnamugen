using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.StateMachine;

namespace xnaMugen.Combat
{
	struct AfterImageSubdata
	{
		public AfterImageSubdata(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			m_location = entity.CurrentLocation;
			m_element = entity.AnimationManager.CurrentElement;
			m_flip = entity.GetDrawFlip();
            m_scale = entity.CurrentScale;
			m_blending = entity.Transparency == new Blending() ? m_element.Blending : entity.Transparency;
			m_facing = entity.CurrentFacing;
			m_drawangle = (entity.AngleDraw == true) ? Misc.FaceScalar(entity.CurrentFacing, -entity.DrawingAngle) : 0;

            if (entity is Character) m_scale *= (entity as Character).DrawScale;
		}

		public Animations.AnimationElement Element
		{
			get { return m_element; }
		}

		public Vector2 Location
		{
			get { return m_location; }
		}

		public SpriteEffects Flip
		{
			get { return m_flip; }
		}

        public Vector2 Scale
        {
            get { return m_scale; }
        }

		public Blending Transparency
		{
			get { return m_blending; }
		}

		public Facing Facing
		{
			get { return m_facing; }
		}

		public Single DrawAngle
		{
			get { return m_drawangle; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationElement m_element;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteEffects m_flip;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Facing m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_drawangle;

		#endregion
	}

	class AfterImage
	{
		public AfterImage(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			m_entity = entity;
			m_imagedata = new LinkedList<AfterImageSubdata>();
			m_countdown = new Dictionary<Int32, Int32>();

			Reset();
		}

		public void ModifyDisplayTime(Int32 time)
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

			Int32 image_index = 0;
			Boolean isactive = false;
			foreach (AfterImageSubdata data in m_imagedata)
			{
				if (TimeCheck_Update(image_index) == true) isactive = true;
				++image_index;
			}

			if (isactive == false && m_firstcreated == true)
			{
				Reset();
			}
		}

		Boolean TimeCheck_Update(Int32 image_index)
		{
			if (DisplayTime == -1) return true;

			if (m_countdown.ContainsKey(image_index) == true)
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

		Boolean TimeCheck(Int32 image_index)
		{
			if (DisplayTime == -1) return true;

			if (m_countdown.ContainsKey(image_index) == true)
			{
				if (m_countdown[image_index] <= 0) return false;
			}

			return true;
		}

		Boolean FrameGapCheck(Int32 index)
		{
			if (index <= 0) return false;

			return index % m_framegap == 0;
		}

		public void Draw()
		{
			if (IsActive == false) return;

			Int32 index = m_imagedata.Count;
			foreach (AfterImageSubdata data in m_imagedata)
			{
				--index;

				if (FrameGapCheck(index) == false) continue;
				if (TimeCheck(index) == false) continue;

				Drawing.Sprite sprite = m_entity.SpriteManager.GetSprite(data.Element.SpriteId);
				if (sprite == null) continue;

				Vector2 drawlocation = data.Location + new Vector2(Mugen.ScreenSize.X / 2, m_entity.Engine.Stage.ZOffset);
				Vector2 drawoffset = Misc.GetOffset(Vector2.Zero, data.Facing, data.Element.Offset);

				Video.DrawState drawstate = m_entity.SpriteManager.SetupDrawing(data.Element.SpriteId, drawlocation, drawoffset, data.Scale, data.Flip);

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

		public Boolean IsActive
		{
			get { return m_isactive; }

			set { m_isactive = value; }
		}

		public Int32 DisplayTime
		{
			get { return m_displaytime; }

			set { m_displaytime = value; }
		}

		public Int32 NumberOfFrames
		{
			get { return m_numberofframes; }

			set { m_numberofframes = value; }
		}

		public Single BaseColor
		{
			get { return m_basecolor; }

			set { m_basecolor = Misc.Clamp(value, 0.0f, 1.0f); }
		}

		public Boolean InvertColor
		{
			get { return m_invertcolor; }

			set { m_invertcolor = value; }
		}

		public Vector3 ColorPreAdd
		{
			get { return m_preadd; }

			set { m_preadd = value; }
		}

		public Vector3 ColorContrast
		{
			get { return m_constrast; }

			set { m_constrast = value; }
		}

		public Vector3 ColorPostAdd
		{
			get { return m_postadd; }

			set { m_postadd = value; }
		}

		public Vector3 ColorPaletteAdd
		{
			get { return m_palettecoloradd; }

			set { m_palettecoloradd = value; }
		}

		public Vector3 ColorPaletteMultiply
		{
			get { return m_palettecolormul; }

			set { m_palettecolormul = value; }
		}

		public Int32 TimeGap
		{
			get { return m_timegap; }

			set { m_timegap = value; }
		}

		public Int32 FrameGap
		{
			get { return m_framegap; }

			set { m_framegap = value; }
		}

		public Blending? Transparency
		{
			get { return m_transparency; }

			set { m_transparency = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Entity m_entity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly LinkedList<AfterImageSubdata> m_imagedata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<Int32, Int32> m_countdown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_displaytime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_numberofframes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_basecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_invertcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_preadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_constrast;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_postadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_palettecoloradd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_palettecolormul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_timegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_framegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Blending? m_transparency;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_timegapcounter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_firstcreated;

		#endregion
	}
}