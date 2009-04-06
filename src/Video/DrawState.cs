using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.Video
{
	class DrawState
	{
		public DrawState(VideoSystem videosystem)
		{
			if (videosystem == null) throw new ArgumentNullException("videosystem");

			m_videosystem = videosystem;
			m_mode = DrawMode.Normal;
            m_texture = null;
            m_scissorrect = Rectangle.Empty;
			m_blending = new Blending();
			m_drawdata = new List<DrawData>(10);
			m_scale = Vector2.One;
			m_axis = Vector2.Zero;
			m_flip = SpriteEffects.None;
			m_rotation = 0;
			m_offset = Vector2.Zero;
			m_parameters = new ShaderParameters();
		}

		public void Use()
		{
			m_videosystem.Draw(this);
		}

		public void Reset()
		{
			m_mode = DrawMode.Normal;
			m_texture = null;
			m_scissorrect = Rectangle.Empty;
			m_blending = new Blending();
			m_drawdata.Clear();
			m_scale = Vector2.One;
			m_axis = Vector2.Zero;
			m_flip = SpriteEffects.None;
			m_rotation = 0;
			m_offset = Vector2.Zero;
			m_parameters.Reset();
		}

		public void AddData(Vector2 location, Rectangle? rect)
		{
			m_drawdata.Add(new DrawData(location, rect));
		}

		public void AddData(Vector2 location, Rectangle? rect, Color tint)
		{
			m_drawdata.Add(new DrawData(location, rect, tint));
		}

		public List<DrawData>.Enumerator GetEnumerator()
		{
			return m_drawdata.GetEnumerator();
		}

		public ShaderParameters ShaderParameters
		{
			get { return m_parameters; }
		}

		public DrawMode Mode
		{
			get { return m_mode; }

			set { m_mode = value; }
		}

		public Texture2D Texture
		{
            get { return m_texture; }

            set { m_texture = value; }
		}

		public Rectangle ScissorRectangle
		{
			get { return m_scissorrect; }

			set { m_scissorrect = value; }
		}

		public Blending Blending
		{
			get { return m_blending; }

			set { m_blending = value; }
		}

		public Vector2 Scale
		{
			get { return m_scale; }

			set { m_scale = value; }
		}

		public Vector2 Axis
		{
			get { return m_axis; }

			set { m_axis = value; }
		}

		public Vector2 Offset
		{
			get { return m_offset; }

			set { m_offset = value; }
		}

		public SpriteEffects Flip
		{
			get { return m_flip; }

			set { m_flip = value; }
		}

		public Single Rotation
		{
			get { return m_rotation; }

			set { m_rotation = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly VideoSystem m_videosystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ShaderParameters m_parameters;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		DrawMode m_mode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Texture2D m_texture;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Rectangle m_scissorrect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		List<DrawData> m_drawdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_rotation;

		#endregion
	}
}