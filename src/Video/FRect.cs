namespace xnaMugen.Video
{
	internal struct FRect
	{
		public float X;
		public float Y;
		public float Width;
		public float Height;

		public float Left => X;
		public float Right => X + Width;
		public float Top => Y;
		public float Bottom => Y + Height;
	}
}