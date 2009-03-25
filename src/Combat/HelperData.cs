using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Combat
{
	class HelperData
	{
		public String Name { get; set; }
		public Int32 HelperId { get; set; }
		public HelperType Type { get; set; }
		public Boolean KeyControl { get; set; }
		public Int32 FacingFlag { get; set; }
		public PositionType PositionType { get; set; }
		public Vector2 CreationOffset { get; set; }
		public Int32 InitialStateNumber { get; set; }
		public Vector2 Scale { get; set; }
		public Int32 GroundFront { get; set; }
		public Int32 GroundBack { get; set; }
		public Int32 AirFront { get; set; }
		public Int32 AirBack { get; set; }
		public Int32 Height { get; set; }
		public Boolean OwnPaletteFx { get; set; }
		public Int32 SuperPauseTime { get; set; }
		public Int32 PauseTime { get; set; }
		public Boolean ProjectileScaling { get; set; }
		public Vector2 HeadPosition { get; set; }
		public Vector2 MidPosition { get; set; }
		public Int32 ShadowOffset { get; set; }
	}
}