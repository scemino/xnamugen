using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class Camera : EngineObject
	{
		private delegate bool CharacterFilter(Character previous, Character current);

		static Camera()
		{
			s_leftmost = (lhs, rhs) => rhs.CameraFollowX && (lhs == null || rhs.GetLeftEdgePosition(true) < lhs.GetLeftEdgePosition(true));
			s_rightmost = (lhs, rhs) => rhs.CameraFollowX && (lhs == null || rhs.GetRightEdgePosition(true) > lhs.GetRightEdgePosition(true));
			s_highest = (lhs, rhs) => rhs.CameraFollowY && (lhs == null || rhs.CurrentLocation.Y < lhs.CurrentLocation.Y);
			s_lowest = (lhs, rhs) => rhs.CameraFollowY && (lhs == null || rhs.CurrentLocation.Y > lhs.CurrentLocation.Y);
		}

		public Camera(FightEngine fightengine)
			: base(fightengine)
		{
			m_bounds = new BoundsRect(0, 0, 0, 0);
			m_location = new Point(0, 0);
		}

		public void Update()
		{
			Move(GetCameraMovement());
			Move(GetCameraMovement());
		}

		private void Move(Point delta)
		{
			var cap = 5;

			delta.X = Misc.Clamp(delta.X, -cap, cap);
			delta.Y = Misc.Clamp(delta.Y, -cap, cap);

			m_location += delta;

			m_location = m_bounds.Bound(m_location);
		}

		private Point GetCameraMovement()
		{
			var left = GetLeftmostCharacterAdjustment();
			var right = GetRightmostCharacterAdjustment();
			var up = GetHighestCharacterAdjustment();

			var delta = new Point(left + right, up);
			return delta;
		}

		public void Reset()
		{
			m_location = new Point(0, 0);
			m_bounds = Engine.Stage.CameraBounds;
		}

		private int GetHighestCharacterAdjustment()
		{
			var character = GetCharacter(s_highest);
			if (character == null) return 0;

			var height = (character.CurrentLocation.Y + Engine.Stage.FloorTension) * Engine.Stage.VerticalFollow;

			return (int)Math.Min(0, height) - Location.Y;
		}

		private int GetLeftmostCharacterAdjustment()
		{
			var character = GetCharacter(s_leftmost);
			if (character == null) return 0;

			var xpos = character.GetLeftEdgePosition(true) - Location.X;
			var leftshift = xpos + (Mugen.ScreenSize.X / 2 - Engine.Stage.Tension);

			return leftshift < 0 ? leftshift : 0;
		}

		private int GetRightmostCharacterAdjustment()
		{
			var character = GetCharacter(s_rightmost);
			if (character == null) return 0;

			var xpos = character.GetRightEdgePosition(true) - Location.X;
			var rightshift = xpos - (Mugen.ScreenSize.X / 2 - Engine.Stage.Tension);

			return rightshift > 0 ? rightshift : 0;
		}

		private Character GetCharacter(CharacterFilter filter)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));

			Character found = null;
			foreach (var entity in Engine.Entities)
			{
				var character = entity as Character;
				if (character == null || HelperCheck(character)) continue;

				if (filter(found, character)) found = character;
			}

			return found;
		}

		private bool HelperCheck(Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			var helper = character as Helper;
			if (helper == null) return false;

			return helper.Data.Type == HelperType.Normal;
		}

		public Rectangle ScreenBounds => new Rectangle(Location.X - Mugen.ScreenSize.X / 2, Location.Y, Mugen.ScreenSize.X, Mugen.ScreenSize.Y);

		public Point Location
		{
			get => m_location;

			set { m_location = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly CharacterFilter s_leftmost;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly CharacterFilter s_rightmost;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly CharacterFilter s_highest;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly CharacterFilter s_lowest;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Point m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BoundsRect m_bounds;

		#endregion
	}
}