using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace xnaMugen.Video
{
	internal class DrawState
	{
		public DrawState(VideoSystem videosystem)
		{
			if (videosystem == null) throw new ArgumentNullException(nameof(videosystem));

			m_videosystem = videosystem;
			m_mode = DrawMode.Normal;
			m_pixels = null;
			m_palette = null;
			m_scissorrect = Rectangle.Empty;
			m_blending = new Blending();
			m_drawdata = new List<DrawData>();
			m_scale = Vector2.One;
			m_axis = Vector2.Zero;
			m_flip = SpriteEffects.None;
			m_rotation = 0;
			m_offset = Vector2.Zero;
			m_stretch = Vector2.One;
			m_parameters = new ShaderParameters();
		}

		public void Use()
		{
			m_videosystem.Draw(this);
		}

		public void Reset()
		{
			m_mode = DrawMode.Normal;
			m_pixels = null;
			m_palette = null;
			m_scissorrect = Rectangle.Empty;
			m_blending = new Blending();
			m_drawdata.Clear();
			m_scale = Vector2.One;
			m_axis = Vector2.Zero;
			m_flip = SpriteEffects.None;
			m_rotation = 0;
			m_offset = Vector2.Zero;
			m_stretch = Vector2.One;

			m_parameters.Reset();
		}

		public void Set(Drawing.Sprite sprite)
		{
			if (sprite != null)
			{
				Pixels = sprite.Pixels;
				Palette = sprite.Palette;
				Axis = (Vector2)sprite.Axis;
			}
			else
			{
				Mode = DrawMode.None;
				Pixels = null;
				Palette = null;
				Axis = Vector2.Zero;
			}
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

		public ShaderParameters ShaderParameters => m_parameters;

		public DrawMode Mode
		{
			get => m_mode;

			set { m_mode = value; }
		}

		public Texture2D Pixels
		{
			get => m_pixels;

			set { m_pixels = value; }
		}

		public Texture2D Palette
		{
			get => m_palette;

			set { m_palette = value; }
		}

		public Rectangle ScissorRectangle
		{
			get => m_scissorrect;

			set { m_scissorrect = value; }
		}

		public Blending Blending
		{
			get => m_blending;

			set { m_blending = value; }
		}

		public Vector2 Scale
		{
			get => m_scale;

			set { m_scale = value; }
		}

		public Vector2 Axis
		{
			get => m_axis;

			set { m_axis = value; }
		}

		public Vector2 Offset
		{
			get => m_offset;

			set { m_offset = value; }
		}

		public SpriteEffects Flip
		{
			get => m_flip;

			set { m_flip = value; }
		}

		public float Rotation
		{
			get => m_rotation;

			set { m_rotation = value; }
		}

		public Vector2 Stretch
		{
			get => m_stretch;

			set { m_stretch = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly VideoSystem m_videosystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ShaderParameters m_parameters;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DrawMode m_mode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Texture2D m_pixels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Texture2D m_palette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Rectangle m_scissorrect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<DrawData> m_drawdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_rotation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_stretch;

		#endregion
	}
}