using xnaMugen.IO;
using xnaMugen.Elements;
using xnaMugen.Drawing;
using System;
using Microsoft.Xna.Framework;
using xnaMugen.Input;

namespace xnaMugen.Menus
{
    internal enum TeamSelectState
    {
        TeamMode,
        SelectSelf,
        SelectMate,
        Stage,
    }

    internal class TeamSelectData
    {
        private Collection m_elements;
        private Point m_position;
        private Point m_itemLocation;
        private Point m_itemSpacing;
        private PrintData m_itemFont;
        private PrintData m_itemActiveFont;
        private PrintData m_itemActive2Font;
        private SelectScreen m_selectscreen;
        private Point m_spacing;
        private int m_blinkval;
        private bool m_moveWrapping;
        private SpriteId m_cursorSpriteId;
        private Point m_cursorOffset;

        private int CurrentLocation { get; set; }

        public ButtonMap ButtonMap { get; }

        public TeamSelectState State { get; set; }

        public SelectData P1SelectData { get; }

        public SelectData P2SelectData { get; }

        public TeamMode TeamMode
        {
            get
            {
                switch (CurrentLocation)
                {
                    case 0: return TeamMode.Single;
                    case 1: return TeamMode.Simul;
                    case 2: return TeamMode.Turns;
                    default:
                        throw new InvalidOperationException("Invalid location");
                }
            }
        }

        public bool IsSelected => State == TeamSelectState.SelectSelf ? P1SelectData.IsSelected : P1SelectData.IsSelected && P2SelectData.IsSelected;

        public TeamSelectData(SelectScreen selectscreen, ButtonMap buttonmap, TextSection textsection, string prefix, bool moveoverempty)
        {
            if (selectscreen == null) throw new ArgumentNullException(nameof(selectscreen));
            if (buttonmap == null) throw new ArgumentNullException(nameof(buttonmap));
            if (textsection == null) throw new ArgumentNullException(nameof(textsection));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            P1SelectData = new SelectData(selectscreen, buttonmap, textsection, prefix, moveoverempty);
            P2SelectData = new SelectData(selectscreen, buttonmap, textsection, prefix, moveoverempty);
            m_selectscreen = selectscreen;
            ButtonMap = buttonmap;
            m_elements = new Collection(selectscreen.SpriteManager, selectscreen.AnimationManager, selectscreen.SoundManager, selectscreen.MenuSystem.FontMap);
            m_moveWrapping = textsection.GetAttribute("teammenu.move.wrapping", true);
            m_position = textsection.GetAttribute<Point>(prefix + ".teammenu.pos");
            m_itemLocation = textsection.GetAttribute<Point>(prefix + ".teammenu.item.offset");
            m_itemSpacing = textsection.GetAttribute<Point>(prefix + ".teammenu.item.spacing");
            m_itemFont = textsection.GetAttribute<PrintData>(prefix + ".teammenu.item.font");
            m_itemActiveFont = textsection.GetAttribute<PrintData>(prefix + ".teammenu.item.active.font");
            m_itemActive2Font = textsection.GetAttribute<PrintData>(prefix + ".teammenu.item.active2.font");
            m_spacing = textsection.GetAttribute<Point>(prefix + ".teammenu.value.spacing");
            m_cursorSpriteId = textsection.GetAttribute<SpriteId>(prefix + ".teammenu.item.cursor.anim");
            m_cursorOffset = textsection.GetAttribute<Point>(prefix + ".teammenu.item.cursor.offset");

            m_elements.Build("selfTitle", textsection, prefix + ".teammenu.selftitle");
            m_elements.Build("enemytitle", textsection, prefix + ".teammenu.enemytitle");
            m_elements.Build("move", textsection, prefix + ".teammenu.move");
            m_elements.Build("value", textsection, prefix + ".teammenu.value");
            m_elements.Build("done", textsection, prefix + ".teammenu.done");
            m_elements.Build("value.icon", textsection, prefix + ".teammenu.value.icon");
            m_elements.Build("empty.icon", textsection, prefix + ".teammenu.value.empty.icon");
        }

        public void Draw()
        {
            var activeFont = m_blinkval > 0 ? m_itemActiveFont : m_itemActive2Font;

            if (State != TeamSelectState.TeamMode) return;

            // Title
            m_elements.GetElement("selfTitle").Draw((Vector2)m_position);

            // Cursor
            m_selectscreen.SpriteManager.Draw(m_cursorSpriteId, (Vector2)m_position,
                                              (Vector2)m_cursorOffset + (Vector2)m_itemLocation + (Vector2)m_itemSpacing * CurrentLocation,
                                              Vector2.One, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            // Single
            m_selectscreen.Print(CurrentLocation == 0 ? activeFont : m_itemFont, (Vector2)m_position + (Vector2)m_itemLocation, "Single", null);

            // Simul
            m_selectscreen.Print(CurrentLocation == 1 ? activeFont : m_itemFont, (Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_itemSpacing, "Simul", null);
            m_elements.GetElement("value.icon").Draw((Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_itemSpacing);
            m_elements.GetElement("value.icon").Draw((Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_spacing + (Vector2)m_itemSpacing);

            // Turns
            m_selectscreen.Print(CurrentLocation == 2 ? activeFont : m_itemFont, (Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_itemSpacing + (Vector2)m_itemSpacing, "Turns", null);
            m_elements.GetElement("value.icon").Draw((Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_itemSpacing + (Vector2)m_itemSpacing);
            m_elements.GetElement("value.icon").Draw((Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_spacing + (Vector2)m_itemSpacing + (Vector2)m_itemSpacing);
            m_elements.GetElement("empty.icon").Draw((Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_spacing + (Vector2)m_itemSpacing + (Vector2)m_spacing + (Vector2)m_itemSpacing);
            m_elements.GetElement("empty.icon").Draw((Vector2)m_position + (Vector2)m_itemLocation + (Vector2)m_spacing + (Vector2)m_itemSpacing + (Vector2)m_spacing + (Vector2)m_spacing + (Vector2)m_itemSpacing);
        }

        public void Reset()
        {
            State = TeamSelectState.TeamMode;
            CurrentLocation = 1;
            P1SelectData.Reset();
            P2SelectData.Reset();
        }

        public void Update()
        {
            if (++m_blinkval > 6) m_blinkval = -6;
        }

        public bool MoveTeamModeCursor(CursorDirection direction)
        {
            var newlocation = GetNewLocation(CurrentLocation, direction);

            var hasmoved = CurrentLocation != newlocation;

            CurrentLocation = newlocation;

            return hasmoved;
        }

        private int GetNewLocation(int location, CursorDirection direction)
        {
            if (direction == CursorDirection.Down)
            {
                location++;
                if (location == 3)
                {
                    location = m_moveWrapping ? 0 : 2;
                }
            }

            if (direction == CursorDirection.Up)
            {
                if (location == 0)
                {
                    location = m_moveWrapping ? 2 : 0;
                }
                else
                {
                    location--;
                }
            }

            return location;
        }

        public void PlayCursorMoveSound()
        {
            var element = m_elements.GetElement("move");
            element?.PlaySound();
        }

        public void PlaySelectSound()
        {
            var element = m_elements.GetElement("value");
            element?.PlaySound();
        }

        public bool MoveCharacterCursor(CursorDirection direction, Point gridsize, bool wrapping)
        {
            if (State == TeamSelectState.SelectSelf)
            {
                return P1SelectData.MoveCharacterCursor(direction, gridsize, wrapping);
            }
            if (State == TeamSelectState.SelectMate)
            {
                return P2SelectData.MoveCharacterCursor(direction, gridsize, wrapping);
            }
            return false;
        }
    }
}