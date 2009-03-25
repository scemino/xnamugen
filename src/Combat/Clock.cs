using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
    class Clock : EngineObject
    {
        public Clock(FightEngine engine)
            : base(engine)
        {
			IO.TextFile textfile = Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            IO.TextSection timesection = textfile.GetSection("Time");

            m_position = (Vector2)timesection.GetAttribute<Point>("pos", new Point(0, 0));
            m_bgelement = Engine.Elements.Build("time bg", timesection, "bg");
            m_counterelement = Engine.Elements.Build("time counter", timesection, "counter");

			m_time = -1;
        }

		String BuildTimeString()
		{
			return (m_time >= 0) ? m_time.ToString() : "o";
		}

		public void Tick()
		{
			if (m_time > 0) --m_time;
		}

        public void Draw()
        {
            m_bgelement.Draw(m_position);

			if (m_counterelement.DataMap.Type == ElementType.Text)
			{
				m_counterelement.Collection.Fonts.Print(m_counterelement.DataMap.FontData, m_position, BuildTimeString(), null);
			}
        }

		public Int32 Time
		{
			get { return m_time; }

			set { m_time = value; }
		}

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_position;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_bgelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_counterelement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

        #endregion
    }
}