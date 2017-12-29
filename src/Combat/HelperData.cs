using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class HelperData
	{
		public string Name { get; set; }
		public int HelperId { get; set; }
		public HelperType Type { get; set; }
		public bool KeyControl { get; set; }
		public int FacingFlag { get; set; }
		public PositionType PositionType { get; set; }
		public Vector2 CreationOffset { get; set; }
		public int InitialStateNumber { get; set; }
		public Vector2 Scale { get; set; }
		public int GroundFront { get; set; }
		public int GroundBack { get; set; }
		public int AirFront { get; set; }
		public int AirBack { get; set; }
		public int Height { get; set; }
		public bool OwnPaletteFx { get; set; }
		public int SuperPauseTime { get; set; }
		public int PauseTime { get; set; }
		public bool ProjectileScaling { get; set; }
		public Vector2 HeadPosition { get; set; }
		public Vector2 MidPosition { get; set; }
		public int ShadowOffset { get; set; }
	}
}