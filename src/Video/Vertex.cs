using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Video
{
	[DebuggerDisplay("{Position} {TextureCoordinate} {Tint}")]
	struct Vertex
	{
		static Vertex()
		{
			s_elements = new VertexElement[3];
			s_elements[0] = new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0);
			s_elements[1] = new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0);
			s_elements[2] = new VertexElement(0, 16, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0);
		}

		public Vertex(Vector2 position, Vector2 tecoord)
		{
			m_position = position;
			m_texcoord = tecoord;
			m_color = Color.White;
		}

		public Vertex(Vector2 position, Vector2 tecoord, Color tint)
		{
			m_position = position;
			m_texcoord = tecoord;
			m_color = tint;
		}

		public Vector2 Position 
		{ 
			get { return m_position; } 

			set { m_position = value; } 
		}

		public Vector2 TextureCoordinate 
		{ 
			get { return m_texcoord; } 

			set { m_texcoord = value; } 
		}

		public Color Tint
		{ 
			get { return m_color; } 
			
			set { m_color = value; } 
		}

		static public VertexElement[] VertexElements { get { return s_elements; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_texcoord;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Color m_color;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static VertexElement[] s_elements;
	}
}