using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Elements;
using xnaMugen.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
    internal class SelectGrid
    {
        public Point Size { get; }

        public bool Wrapping { get; }

        public ListIterator<PlayerSelect> PlayerProfiles { get; }

        public bool MoveOverEmptyBoxes { get; }

        public Point GridPosition { get; }

        public int CellSpacing { get; private set; }

        public  Point CellSize { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool m_showemptyboxes;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<Point, PlayerSelect> m_selectmap;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<Point, PlayerSelect> m_selectmovemap;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_blinkval;

        private Collection m_elements;

        public SelectGrid(TextSection textsection, Collection elements, ListIterator<PlayerSelect> playerProfiles)
        {
            PlayerProfiles = playerProfiles;
            m_selectmap = new Dictionary<Point, PlayerSelect>();
            m_selectmovemap = new Dictionary<Point, PlayerSelect>();
            m_elements = elements;
            m_elements.Build(textsection, "cell.bg");
            m_elements.Build(textsection, "cell.random");

            Size = new Point(textsection.GetAttribute<int>("columns"), textsection.GetAttribute<int>("rows"));
            Wrapping = textsection.GetAttribute<bool>("wrapping");
            m_showemptyboxes = textsection.GetAttribute<bool>("showEmptyBoxes");
            MoveOverEmptyBoxes = textsection.GetAttribute<bool>("moveOverEmptyBoxes");
            GridPosition = textsection.GetAttribute<Point>("pos");
            CellSize = textsection.GetAttribute<Point>("cell.size");
            CellSpacing = textsection.GetAttribute<int>("cell.spacing");
        }

        public void Draw()
        {
            var cellbg = m_elements.GetElement("cell.bg") as StaticImage;
            if (cellbg == null) return;

            var sprite = cellbg.SpriteManager.GetSprite(cellbg.DataMap.SpriteId);
            if (sprite == null) return;

            var drawstate = cellbg.SpriteManager.DrawState;
            drawstate.Reset();
            drawstate.Set(sprite);

            for (var y = 0; y != Size.Y; ++y)
            {
                for (var x = 0; x != Size.X; ++x)
                {
                    var location = GridPosition;
                    location.X += (CellSize.X + CellSpacing) * x;
                    location.Y += (CellSize.Y + CellSpacing) * y;

                    var selection = GetSelection(new Point(x, y), false);
                    if (selection == null && m_showemptyboxes == false) continue;

                    drawstate.AddData((Vector2)location, null);
                }
            }

            drawstate.Use();
        }

        public PlayerSelect GetSelection(Point location, bool movement)
        {
            if (location.X < 0 || location.X >= Size.X || location.Y < 0 || location.Y >= Size.Y) return null;

            var map = movement ? m_selectmovemap : m_selectmap;

            if (map.TryGetValue(location, out PlayerSelect @select)) return @select;

            var index = 0;
            var profiles = PlayerProfiles;

            for (var y = 0; y != Size.Y; ++y)
            {
                for (var x = 0; x != Size.X; ++x)
                {
                    var selection = index < profiles.Count ? profiles[index] : null;
                    ++index;

                    if (location == new Point(x, y))
                    {
                        if (selection == null && movement && MoveOverEmptyBoxes == false) return null;

                        map[location] = selection;
                        return selection;
                    }

                    //if (selection == null && movement == true && m_moveoveremptyboxes == false) continue;
                    //if (location == new Point(x, y)) return selection;
                }
            }

            return null;
        }

        public void Reset()
        {
            m_blinkval = 0;
        }

        public void Update()
        {
            if (++m_blinkval > 6) m_blinkval = -6;
        }

        public void DrawCursorGrid(SelectData p1, SelectData p2)
        {
            for (var y = 0; y != Size.Y; ++y)
            {
                for (var x = 0; x != Size.X; ++x)
                {
                    var xy = new Point(x, y);

                    var location = (Vector2)GridPosition;
                    location.X += (CellSize.X + CellSpacing) * x;
                    location.Y += (CellSize.Y + CellSpacing) * y;

                    var selection = GetSelection(xy, false);
                    if (selection != null && selection.SelectionType == PlayerSelectType.Profile)
                        selection.Profile.SpriteManager.Draw(SpriteId.SmallPortrait, location, Vector2.Zero, Vector2.One, SpriteEffects.None);

                    if (selection != null && selection.SelectionType == PlayerSelectType.Random)
                    {
                        var randomimage = m_elements.GetElement("cell.random") as StaticImage;
                        if (randomimage != null) randomimage.Draw(location);
                    }

                    if (p1?.CurrentCell == xy && p2?.CurrentCell == xy)
                    {
                        if (m_blinkval > 0) p1.DrawCursorActive(location);
                        else p2.DrawCursorActive(location);
                    }
                    else if (p1?.CurrentCell == xy)
                    {
                        p1.DrawCursorActive(location);
                    }
                    else if (p2?.CurrentCell == xy)
                    {
                        p2.DrawCursorActive(location);
                    }
                }
            }
        }
    }
}