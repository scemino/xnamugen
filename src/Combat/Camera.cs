using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	class Camera : EngineObject
	{
		delegate Boolean CharacterFilter(Character previous, Character current);

		static Camera()
		{
			s_leftmost = (lhs, rhs) => rhs.CameraFollowX == true && (lhs == null || rhs.GetLeftEdgePosition(true) < lhs.GetLeftEdgePosition(true));
			s_rightmost = (lhs, rhs) => rhs.CameraFollowX == true && (lhs == null || rhs.GetRightEdgePosition(true) > lhs.GetRightEdgePosition(true));
			s_highest = (lhs, rhs) => rhs.CameraFollowY == true && (lhs == null || rhs.CurrentLocation.Y < lhs.CurrentLocation.Y);
			s_lowest = (lhs, rhs) => rhs.CameraFollowY == true && (lhs == null || rhs.CurrentLocation.Y > lhs.CurrentLocation.Y);
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

		void Move(Point delta)
		{
			Int32 cap = 5;

			delta.X = Misc.Clamp(delta.X, -cap, cap);
			delta.Y = Misc.Clamp(delta.Y, -cap, cap);

			m_location += delta;

			m_location = m_bounds.Bound(m_location);
		}

		Point GetCameraMovement()
		{
			Int32 left = GetLeftmostCharacterAdjustment();
			Int32 right = GetRightmostCharacterAdjustment();
			Int32 up = GetHighestCharacterAdjustment();

			Point delta = new Point(left + right, up);
			return delta;
		}

		public void Reset()
		{
			m_location = new Point(0, 0);
			m_bounds = Engine.Stage.CameraBounds;
		}

		Int32 GetHighestCharacterAdjustment()
		{
			Character character = GetCharacter(s_highest);
			if (character == null) return 0;

			Single height = (character.CurrentLocation.Y + Engine.Stage.FloorTension) * Engine.Stage.VerticalFollow;

			return (Int32)Math.Min(0, height) - Location.Y;
		}

		Int32 GetLeftmostCharacterAdjustment()
		{
			Character character = GetCharacter(s_leftmost);
			if (character == null) return 0;

			Int32 xpos = character.GetLeftEdgePosition(true) - Location.X;
			Int32 leftshift = xpos + ((Mugen.ScreenSize.X / 2) - Engine.Stage.Tension);

			return (leftshift < 0) ? leftshift : 0;
		}

		Int32 GetRightmostCharacterAdjustment()
		{
			Character character = GetCharacter(s_rightmost);
			if (character == null) return 0;

			Int32 xpos = character.GetRightEdgePosition(true) - Location.X;
			Int32 rightshift = xpos - ((Mugen.ScreenSize.X / 2) - Engine.Stage.Tension);

			return (rightshift > 0) ? rightshift : 0;
		}

		Character GetCharacter(CharacterFilter filter)
		{
			if (filter == null) throw new ArgumentNullException("filter");

			Character found = null;
			foreach (Entity entity in Engine.Entities)
			{
				Character character = entity as Character;
				if (character == null || HelperCheck(character) == true) continue;

				if (filter(found, character) == true) found = character;
			}

			return found;
		}

		Boolean HelperCheck(Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			Helper helper = character as Helper;
			if (helper == null) return false;

			return helper.Data.Type == HelperType.Normal;
		}

		public Rectangle ScreenBounds
		{
			get { return new Rectangle(Location.X - (Mugen.ScreenSize.X / 2), Location.Y, Mugen.ScreenSize.X, Mugen.ScreenSize.Y); }
		}

		public Point Location
		{
			get { return m_location; }

			set { m_location = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static CharacterFilter s_leftmost;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static CharacterFilter s_rightmost;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static CharacterFilter s_highest;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static CharacterFilter s_lowest;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Point m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		BoundsRect m_bounds;

		#endregion
	}
}