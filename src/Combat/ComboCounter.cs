using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
    internal class ComboCounter
    {
        private enum State { None, NotShown, MovingIn, MovingOut, Shown }

        public ComboCounter(Team team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));

            m_team = team;

            var prefix = Misc.GetPrefix(m_team.Side);

            var textfile = m_team.Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            var combosection = textfile.GetSection("Combo");

            m_displaylocation = (Vector2)combosection.GetAttribute("pos", new Point(0, 0));

            var startx = (float)combosection.GetAttribute("start.x", (int)m_displaylocation.X);
            var starty = (float)combosection.GetAttribute("start.y", (int)m_displaylocation.Y);
            m_startlocation = new Vector2(startx, starty);

            m_counterelement = m_team.Engine.Elements.Build(prefix + ".combo counter", combosection, "counter");
            m_displayelement = m_team.Engine.Elements.Build(prefix + ".combo text", combosection, "text");

            m_displaytime = combosection.GetAttribute("displaytime", 90);

            m_velocity = new Vector2(5, 5);
            m_state = State.NotShown;
            m_currentlocation = m_startlocation;
            m_hitcount = 0;
            m_displaytimecount = 0;
            m_countertext = string.Empty;
            m_displaytext = string.Empty;
            m_hitbonus = 0;
        }

        public void Reset()
        {
            m_state = State.NotShown;
            m_currentlocation = m_startlocation;
            m_hitcount = 0;
            m_displaytimecount = 0;
            m_countertext = string.Empty;
            m_displaytext = string.Empty;
            m_hitbonus = 0;
            m_displayelement.Reset();
            m_displayelement.Reset();
        }

        public void Update()
        {
            SetHitCount(GetNewHitCount());

            switch (m_state)
            {
                case State.NotShown:
                    m_hitbonus = 0;
                    m_hitcount = 0;
                    m_displaytimecount = 0;
                    break;

                case State.MovingIn:
                    m_displaytimecount = 0;
                    m_currentlocation = MoveVector(m_currentlocation, m_displaylocation, m_velocity);
                    if (m_currentlocation == m_displaylocation) m_state = State.Shown;
                    break;

                case State.Shown:
                    m_displaytimecount = Math.Max(0, m_displaytimecount - 1);
                    if (m_displaytimecount == 0) m_state = State.MovingOut;
                    break;

                case State.MovingOut:
                    m_displaytimecount = 0;
                    m_currentlocation = MoveVector(m_currentlocation, m_startlocation, m_velocity);
                    if (m_currentlocation == m_startlocation) m_state = State.NotShown;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("m_state");
            }
        }

        public void Draw()
        {
            if (m_state == State.NotShown) return;

            var location = GetDrawLocation();

            var offset = DrawElement(m_counterelement, location, m_countertext);

            location.X += offset;

            DrawElement(m_displayelement, location, m_displaytext);
        }

        private int DrawElement(Elements.Base element, Vector2 location, string overridetext)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (overridetext == null) throw new ArgumentNullException(nameof(overridetext));

            if (element is Elements.Text)
            {
                var fontdata = ModifyPrintData(element.DataMap.FontData);
                var font = element.Collection.Fonts.GetFont(fontdata.Index);
                if (font == null) return 0;

                font.Print(location + element.DataMap.Offset, fontdata.ColorIndex, fontdata.Justification, overridetext, null);
                return font.GetTextLength(overridetext);
            }

            return 0;
        }

        public void AddHits(int hits)
        {
            if (hits < 0) throw new ArgumentOutOfRangeException(nameof(hits));

            m_hitbonus += hits;
        }

        private Drawing.PrintData ModifyPrintData(Drawing.PrintData data)
        {
            if (data.IsValid == false) return new Drawing.PrintData();

            PrintJustification justification;
            switch (m_team.Side)
            {
                case TeamSide.Left:
                    justification = PrintJustification.Left;
                    break;

                case TeamSide.Right:
                    justification = PrintJustification.Right;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("m_team.Side");
            }

            return new Drawing.PrintData(data.Index, data.ColorIndex, justification);
        }

        private Vector2 GetDrawLocation()
        {
            switch (m_team.Side)
            {
                case TeamSide.Left:
                    return m_currentlocation;

                case TeamSide.Right:
                    return new Vector2(Mugen.ScreenSize.X - m_currentlocation.X, m_currentlocation.Y);

                default:
                    throw new ArgumentOutOfRangeException("m_team.Side");
            }
        }

        private void SetHitCount(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            if (count < 2) return;

            if (count == m_hitcount)
            {
                m_displaytimecount = m_displaytime;
            }
            else if (count > m_hitcount)
            {
                m_hitcount = count;
                m_countertext = m_team.Engine.GetSubSystem<StringFormatter>().BuildString("%i", m_hitcount);
                m_displaytext = m_team.Engine.GetSubSystem<StringFormatter>().BuildString(m_displayelement.DataMap.Text, m_hitcount);

                if (m_state == State.MovingOut || m_state == State.NotShown) m_state = State.MovingIn;
            }
        }

        private static Vector2 MoveVector(Vector2 location, Vector2 target, Vector2 velocity)
        {
            if (location.X > target.X) location.X = Math.Max(target.X, location.X - velocity.X);
            if (location.X < target.X) location.X = Math.Min(target.X, location.X + velocity.X);

            if (location.Y > target.Y) location.Y = Math.Max(target.Y, location.Y - velocity.Y);
            if (location.Y < target.Y) location.Y = Math.Min(target.Y, location.Y + velocity.Y);

            return location;
        }

        private int GetNewHitCount()
        {
            var otherteam = m_team.OtherTeam;
            var hitcount = 0;

            if (otherteam.MainPlayer != null && otherteam.MainPlayer.MoveType == MoveType.BeingHit) hitcount += otherteam.MainPlayer.DefensiveInfo.HitCount;
            if (otherteam.TeamMate != null && otherteam.TeamMate.MoveType == MoveType.BeingHit) hitcount += otherteam.TeamMate.DefensiveInfo.HitCount;

            return hitcount + m_hitbonus;
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Team m_team;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_displaylocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_startlocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int m_displaytime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private State m_state;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 m_currentlocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_velocity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_hitcount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_displaytimecount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string m_countertext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string m_displaytext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Elements.Base m_displayelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Elements.Base m_counterelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_hitbonus;

        #endregion
    }
}