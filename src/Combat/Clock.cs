using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class Clock : EngineObject
    {
        public Clock(FightEngine engine)
            : base(engine)
        {
			var textfile = Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            var timesection = textfile.GetSection("Time");

            m_position = (Vector2)timesection.GetAttribute("pos", new Point(0, 0));
            m_bgelement = Engine.Elements.Build("time bg", timesection, "bg");
            m_counterelement = Engine.Elements.Build("time counter", timesection, "counter");

			m_time = -1;
        }

	    private string BuildTimeString()
		{
			return m_time >= 0 ? m_time.ToString() : "o";
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

		public int Time
		{
			get => m_time;

			set { m_time = value; }
		}

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_position;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Elements.Base m_bgelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Elements.Base m_counterelement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

        #endregion
    }
}