using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	class RoundInformation : EngineObject
	{
		public RoundInformation(FightEngine engine, IO.TextFile file)
			: base(engine)
		{
			if (file == null) throw new ArgumentNullException("file");

			IO.TextSection roundsection = file.GetSection("Round");
			IO.TextSection powerbarsection = file.GetSection("Powerbar");

            var elements = Engine.Elements;

            elements.Build(roundsection, "round.default");
            elements.Build(roundsection, "round");
            elements.Build(roundsection, "round1");
            elements.Build(roundsection, "round2");
            elements.Build(roundsection, "round3");
            elements.Build(roundsection, "round4");
            elements.Build(roundsection, "round5");
            elements.Build(roundsection, "round6");
            elements.Build(roundsection, "round7");
            elements.Build(roundsection, "round8");
            elements.Build(roundsection, "round9");
            elements.Build(roundsection, "fight");
            elements.Build(roundsection, "KO");
            elements.Build(roundsection, "DKO");
            elements.Build(roundsection, "TO");
            elements.Build(roundsection, "win");
            elements.Build(roundsection, "win2");
            elements.Build(roundsection, "draw");

            elements.Build(powerbarsection, "level1");
            elements.Build(powerbarsection, "level2");
            elements.Build(powerbarsection, "level3");
            elements.Build(powerbarsection, "level4");
            elements.Build(powerbarsection, "level5");
            elements.Build(powerbarsection, "level6");
            elements.Build(powerbarsection, "level7");
            elements.Build(powerbarsection, "level8");
            elements.Build(powerbarsection, "level9");

			m_roundsforwin = roundsection.GetAttribute<Int32>("match.wins");
			m_maxdrawgames = roundsection.GetAttribute<Int32>("match.maxdrawgames");
			m_introdelay = roundsection.GetAttribute<Int32>("start.waittime");
			m_defaultlocation = (Vector2)roundsection.GetAttribute<Point>("pos");
			m_rounddisplaytime = roundsection.GetAttribute<Int32>("round.time");
			m_controltime = roundsection.GetAttribute<Int32>("ctrl.time");
			m_koslowtime = roundsection.GetAttribute<Int32>("slow.time");
			m_overwaittime = roundsection.GetAttribute<Int32>("over.waittime");
			m_overhittime = roundsection.GetAttribute<Int32>("over.hittime");
			m_overwintime = roundsection.GetAttribute<Int32>("over.wintime");
			m_overtime = roundsection.GetAttribute<Int32>("over.time");
			m_wintime = roundsection.GetAttribute<Int32>("win.time");

			m_roundnumbers = BuildRoundNumbersSoundMap();
		}

		ReadOnlyDictionary<Int32, Elements.Base> BuildRoundNumbersSoundMap()
		{
            var elements = Engine.Elements;

			Dictionary<Int32, Elements.Base> roundnumbers = new Dictionary<Int32, Elements.Base>();
            roundnumbers[1] = elements.GetElement("round1");
            roundnumbers[2] = elements.GetElement("round2");
            roundnumbers[3] = elements.GetElement("round3");
            roundnumbers[4] = elements.GetElement("round4");
            roundnumbers[5] = elements.GetElement("round5");
            roundnumbers[6] = elements.GetElement("round6");
            roundnumbers[7] = elements.GetElement("round7");
            roundnumbers[8] = elements.GetElement("round8");
            roundnumbers[9] = elements.GetElement("round9");

			return new ReadOnlyDictionary<Int32, Elements.Base>(roundnumbers);
		}

		public Elements.Base GetRoundElement(Int32 roundnumber)
		{
			return m_roundnumbers[roundnumber];
		}

		public void PlaySoundElement(String elementname)
		{
			if (elementname == null) throw new ArgumentNullException("elementname");

			Engine.Elements.PlaySound(elementname);
		}

		public Int32 NumberOfRounds
		{
			get { return m_roundsforwin; }
		}

		public Int32 MaximumDrawGames
		{
			get { return m_maxdrawgames; }
		}

		public Int32 IntroDelay
		{
			get { return m_introdelay; }
		}

		public Int32 RoundDisplayTime
		{
			get { return m_rounddisplaytime; }
		}

		public Int32 ControlTime
		{
			get { return m_controltime; }
		}

		public Int32 KOSlowTime
		{
			get { return m_koslowtime; }
		}

		public Int32 OverWaitTime
		{
			get { return m_overwaittime; }
		}

		public Int32 OverHitTime
		{
			get { return m_overhittime; }
		}

		public Int32 OverWinTime
		{
			get { return m_overwintime; }
		}

		public Int32 OverTime
		{
			get { return m_overtime; }
		}

		public Int32 WinTime
		{
			get { return m_wintime; }
		}

		public Vector2 DefaultElementLocation
		{
			get { return m_defaultlocation; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_roundsforwin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_maxdrawgames;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_introdelay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_defaultlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_rounddisplaytime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_controltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_koslowtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_overwaittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_overhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_overwintime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_overtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_wintime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyDictionary<Int32, Elements.Base> m_roundnumbers;

		#endregion
	}
}