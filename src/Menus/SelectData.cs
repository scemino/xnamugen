using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
	class SelectData
	{
		public SelectData(SelectScreen selectscreen, Input.ButtonMap buttonmap, TextSection textsection, String prefix, Boolean moveoverempty)
		{
			if (selectscreen == null) throw new ArgumentNullException("selectscreen");
			if (buttonmap == null) throw new ArgumentNullException("buttonmap");
			if (textsection == null) throw new ArgumentNullException("textsection");
			if (prefix == null) throw new ArgumentNullException("prefix");

			m_selectscreen = selectscreen;
			m_buttonmap = buttonmap;
			m_moveoverempty = moveoverempty;

			m_elements = new Elements.Collection(SelectScreen.SpriteManager, SelectScreen.AnimationManager, SelectScreen.SoundManager, SelectScreen.MenuSystem.FontMap);
			m_elements.Build("cursor.active", textsection, prefix + ".cursor.active");
			m_elements.Build("cursor.done", textsection, prefix + ".cursor.done");
			m_elements.Build("cursor.move", textsection, prefix + ".cursor.move");
			m_elements.Build("random.move", textsection, prefix + ".random.move");
			m_elements.Build("player.face", textsection, prefix + ".face");
			m_elements.Build("player.name", textsection, prefix + ".name");

			m_startcell = textsection.GetAttribute<Point>(prefix + ".cursor.startcell");

			// X & Y seem to be reversed for this
			m_startcell = new Point(m_startcell.Y, m_startcell.X);

			Reset();
		}

		public void Reset()
		{
			m_currentcell = m_startcell;
			m_selected = false;
			m_paletteindex = 0;

			m_elements.Reset();
		}

		public void Update()
		{
			m_elements.Update();
		}

		public Boolean MoveCharacterCursor(CursorDirection direction, Point gridsize, Boolean wrapping)
		{
			Point newlocation = GetNewLocation(CurrentCell, direction, gridsize, wrapping);

			Boolean hasmoved = CurrentCell != newlocation;

			CurrentCell = newlocation;

			return hasmoved;
		}

		public void PlayCursorMoveSound()
		{
			Elements.Base element = m_elements.GetElement("cursor.move");
			if (element != null) element.PlaySound();
		}

		public void PlaySelectSound()
		{
			Elements.Base element = m_elements.GetElement("cursor.done");
			if (element != null) element.PlaySound();
		}

		public void DrawCursorActive(Vector2 location)
		{
			Elements.Base element = m_elements.GetElement("cursor.active");
			if (element != null) element.Draw(location);
		}

		public void DrawCursorDone(Vector2 location)
		{
			Elements.Base element = m_elements.GetElement("cursor.done");
			if (element != null) element.Draw(location);
		}

		public void DrawProfile(PlayerProfile profile)
		{
			if (profile == null) throw new ArgumentNullException("profile");

			Elements.Base face = m_elements.GetElement("player.face");
			if (face != null)
			{
				profile.SpriteManager.Draw(SpriteId.LargePortrait, face.DataMap.Offset, Vector2.Zero, face.DataMap.Scale, face.DataMap.Flip);
			}

			Elements.Text name = m_elements.GetElement("player.name") as Elements.Text;
			if (name != null)
			{
				SelectScreen.Print(name.DataMap.FontData, name.DataMap.Offset, profile.DisplayName, null);
			}
		}

		Point GetNewLocation(Point location, CursorDirection direction, Point gridsize, Boolean wrapping)
		{
			if (direction == CursorDirection.Down)
			{
				Point newlocation = location + new Point(0, 1);
				if (newlocation.Y > gridsize.Y - 1)
				{
					if (wrapping == false) return location;
					newlocation.Y = 0;
				}

				for (Int32 i = newlocation.Y; i < gridsize.Y; ++i)
				{
					newlocation.Y = i;
					PlayerSelect selection = SelectScreen.GetSelection(newlocation, true);
					if (selection != null || m_moveoverempty == true) return newlocation;
				}
			}

			if (direction == CursorDirection.Up)
			{
				Point newlocation = location + new Point(0, -1);
				if (newlocation.Y < 0)
				{
					if (wrapping == false) return location;
					newlocation.Y = gridsize.Y - 1;
				}

				for (Int32 i = newlocation.Y; i >= 0; --i)
				{
					newlocation.Y = i;
					PlayerSelect selection = SelectScreen.GetSelection(newlocation, true);
					if (selection != null || m_moveoverempty == true) return newlocation;
				}
			}

			if (direction == CursorDirection.Left)
			{
				Point newlocation = location + new Point(-1, 0);
				if (newlocation.X < 0)
				{
					if (wrapping == false) return location;
					newlocation.X = gridsize.X - 1;
				}

				for (Int32 i = newlocation.X; i >= 0; --i)
				{
					newlocation.X = i;
					PlayerSelect selection = SelectScreen.GetSelection(newlocation, true);
					if (selection != null || m_moveoverempty == true) return newlocation;
				}
			}

			if (direction == CursorDirection.Right)
			{
				Point newlocation = location + new Point(1, 0);
				if (newlocation.X > gridsize.X - 1)
				{
					if (wrapping == false) return location;
					newlocation.X = 0;
				}

				for (Int32 i = newlocation.X; i < gridsize.X; ++i)
				{
					newlocation.X = i;
					PlayerSelect selection = SelectScreen.GetSelection(newlocation, true);
					if (selection != null || m_moveoverempty == true) return newlocation;
				}
			}

			return location;
		}

		public SelectScreen SelectScreen
		{
			get { return m_selectscreen; }
		}

		public Point StartCell
		{
			get { return m_startcell; }
		}

		public Point CurrentCell
		{
			get { return m_currentcell; }

			set { m_currentcell = value; }
		}

		public Boolean IsSelected
		{
			get { return m_selected; }

			set { m_selected = value; }
		}

		public Int32 PaletteIndex
		{
			get { return m_paletteindex; }

			set { m_paletteindex = value; }
		}

		public Input.ButtonMap ButtonMap
		{
			get { return m_buttonmap; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SelectScreen m_selectscreen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Elements.Collection m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_startcell;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_moveoverempty;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Point m_currentcell;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_selected;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_paletteindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Input.ButtonMap m_buttonmap;

		#endregion
	}
}