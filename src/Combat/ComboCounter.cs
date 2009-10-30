using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
    class ComboCounter
    {
        enum State { None, NotShown, MovingIn, MovingOut, Shown }

        public ComboCounter(Team team)
        {
            if (team == null) throw new ArgumentNullException("team");

            m_team = team;

            String prefix = Misc.GetPrefix(m_team.Side);

            IO.TextFile textfile = m_team.Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            IO.TextSection combosection = textfile.GetSection("Combo");

            m_displaylocation = (Vector2)combosection.GetAttribute<Point>("pos", new Point(0, 0));

            Single startx = (Single)combosection.GetAttribute<Int32>("start.x", (Int32)m_displaylocation.X);
            Single starty = (Single)combosection.GetAttribute<Int32>("start.y", (Int32)m_displaylocation.Y);
            m_startlocation = new Vector2(startx, starty);

            m_counterelement = m_team.Engine.Elements.Build(prefix + ".combo counter", combosection, "counter");
            m_displayelement = m_team.Engine.Elements.Build(prefix + ".combo text", combosection, "text");

            m_displaytime = combosection.GetAttribute<Int32>("displaytime", 90);

            m_velocity = new Vector2(5, 5);
            m_state = State.NotShown;
            m_currentlocation = m_startlocation;
            m_hitcount = 0;
            m_displaytimecount = 0;
            m_countertext = String.Empty;
            m_displaytext = String.Empty;
            m_hitbonus = 0;
        }

        public void Reset()
        {
            m_state = State.NotShown;
            m_currentlocation = m_startlocation;
            m_hitcount = 0;
            m_displaytimecount = 0;
            m_countertext = String.Empty;
            m_displaytext = String.Empty;
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

            Vector2 location = GetDrawLocation();

            Int32 offset = DrawElement(m_counterelement, location, m_countertext);

            location.X += offset;

            DrawElement(m_displayelement, location, m_displaytext);
        }

        Int32 DrawElement(Elements.Base element, Vector2 location, String overridetext)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (overridetext == null) throw new ArgumentNullException("overridetext");

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

        public void AddHits(Int32 hits)
        {
            if (hits < 0) throw new ArgumentOutOfRangeException("hits");

            m_hitbonus += hits;
        }

        Drawing.PrintData ModifyPrintData(Drawing.PrintData data)
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

        Vector2 GetDrawLocation()
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

        void SetHitCount(Int32 count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count");

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

        static Vector2 MoveVector(Vector2 location, Vector2 target, Vector2 velocity)
        {
            if (location.X > target.X) location.X = Math.Max(target.X, location.X - velocity.X);
            if (location.X < target.X) location.X = Math.Min(target.X, location.X + velocity.X);

            if (location.Y > target.Y) location.Y = Math.Max(target.Y, location.Y - velocity.Y);
            if (location.Y < target.Y) location.Y = Math.Min(target.Y, location.Y + velocity.Y);

            return location;
        }

        Int32 GetNewHitCount()
        {
            Team otherteam = m_team.OtherTeam;
            Int32 hitcount = 0;

            if (otherteam.MainPlayer != null && otherteam.MainPlayer.MoveType == MoveType.BeingHit) hitcount += otherteam.MainPlayer.DefensiveInfo.HitCount;
            if (otherteam.TeamMate != null && otherteam.TeamMate.MoveType == MoveType.BeingHit) hitcount += otherteam.TeamMate.DefensiveInfo.HitCount;

            return hitcount + m_hitbonus;
        }

        State CounterState
        {
            get { return m_state; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Team m_team;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_displaylocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_startlocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Int32 m_displaytime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        State m_state;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Vector2 m_currentlocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_velocity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_hitcount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_displaytimecount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String m_countertext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String m_displaytext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_displayelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_counterelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_hitbonus;

        #endregion
    }
}