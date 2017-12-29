using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Video
{
	[DebuggerDisplay("{Position} {TextureCoordinate} {Tint}")]
	internal struct Vertex
	{
		public Vector4 Position;

		public Vector2 TextureCoordinate;

		public Color Tint;

		public Vertex(Vector4 position,	Vector2	tecoord)
		{
			Position = position;
			TextureCoordinate =	tecoord;
			Tint = Color.White;
		}

		public Vertex(Vector4 position,	Vector2	tecoord, Color tint)
		{
			Position = position;
			TextureCoordinate =	tecoord;
			Tint = tint;
		}

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
		(
			new	VertexElement(0, VertexElementFormat.Vector4, /*VertexElementMethod.Default, */VertexElementUsage.Position,	0),
			new	VertexElement(16, VertexElementFormat.Vector2, /*VertexElementMethod.Default, */VertexElementUsage.TextureCoordinate, 0),
			new	VertexElement(24, VertexElementFormat.Color, /*VertexElementMethod.Default,	*/VertexElementUsage.Color,	0)
		);
	}
}