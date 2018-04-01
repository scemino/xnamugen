using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Elements;

namespace xnaMugen.Video
{
	internal class DrawState
	{
		public DrawState(VideoSystem videosystem)
		{
			if (videosystem == null) throw new ArgumentNullException(nameof(videosystem));

			m_videosystem = videosystem;
			Mode = DrawMode.Normal;
			Pixels = null;
			Palette = null;
			ScissorRectangle = Rectangle.Empty;
			Blending = new Blending();
			m_drawdata = new List<DrawData>();
			Scale = Vector2.One;
			Axis = Vector2.Zero;
			Flip = SpriteEffects.None;
			Rotation = 0;
			Offset = Vector2.Zero;
			Stretch = Vector2.One;
			m_parameters = new ShaderParameters();
		}

		public void Use()
		{
			m_videosystem.Draw(this);
		}

		public void Reset()
		{
			Mode = DrawMode.Normal;
			Pixels = null;
			Palette = null;
			ScissorRectangle = Rectangle.Empty;
			Blending = new Blending();
			m_drawdata.Clear();
			Scale = Vector2.One;
			Axis = Vector2.Zero;
			Flip = SpriteEffects.None;
			Rotation = 0;
			Offset = Vector2.Zero;
			Stretch = Vector2.One;

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
		
		public static Rectangle CreateBarScissorRectangle(Base element, Vector2 location, float percentage, Point range)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));

			var sprite = element.SpriteManager.GetSprite(element.DataMap.SpriteId);
			if (sprite == null) return new Rectangle();

			var drawlocation = (Point) Video.Renderer.GetDrawLocation(sprite.Size, location, (Vector2) sprite.Axis,
				element.DataMap.Scale, element.DataMap.Flip);

			var rectangle = new Rectangle();
			rectangle.X = (int) element.DataMap.Offset.X + drawlocation.X + 1;
			rectangle.Y = (int) element.DataMap.Offset.Y + drawlocation.Y;
			rectangle.Height = sprite.Size.Y + 1;
			rectangle.Width = sprite.Size.X + 1;

			var position = (int) MathHelper.Lerp(range.X, range.Y, percentage);
			if (position > 0)
			{
				rectangle.Width = position + 2;
			}
			else if (position < 0)
			{
				rectangle.Width = -position + 2;

				rectangle.X += position - range.Y - 1;
			}
			else
			{
				rectangle.Width = 0;
			}

			return rectangle;
		}

		public ShaderParameters ShaderParameters => m_parameters;

		public DrawMode Mode { get; set; }

		public Texture2D Pixels { get; set; }

		public Texture2D Palette { get; set; }

		public Rectangle ScissorRectangle { get; set; }

		public Blending Blending { get; set; }

		public Vector2 Scale { get; set; }

		public Vector2 Axis { get; set; }

		public Vector2 Offset { get; set; }

		public SpriteEffects Flip { get; set; }

		public float Rotation { get; set; }

		public Vector2 Stretch { get; set; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly VideoSystem m_videosystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ShaderParameters m_parameters;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<DrawData> m_drawdata;

		#endregion
	}
}