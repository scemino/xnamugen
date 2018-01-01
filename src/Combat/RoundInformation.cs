using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	internal class RoundInformation : EngineObject
	{
		public RoundInformation(FightEngine engine, IO.TextFile file)
			: base(engine)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			var roundsection = file.GetSection("Round");
			var powerbarsection = file.GetSection("Powerbar");

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

			m_roundsforwin = roundsection.GetAttribute<int>("match.wins");
			m_maxdrawgames = roundsection.GetAttribute<int>("match.maxdrawgames");
			m_introdelay = roundsection.GetAttribute<int>("start.waittime");
			m_defaultlocation = (Vector2)roundsection.GetAttribute<Point>("pos");
			m_rounddisplaytime = roundsection.GetAttribute<int>("round.time");
			m_controltime = roundsection.GetAttribute<int>("ctrl.time");
			m_koslowtime = roundsection.GetAttribute<int>("slow.time");
			m_overwaittime = roundsection.GetAttribute<int>("over.waittime");
			m_overhittime = roundsection.GetAttribute<int>("over.hittime");
			m_overwintime = roundsection.GetAttribute<int>("over.wintime");
			m_overtime = roundsection.GetAttribute<int>("over.time");
			m_wintime = roundsection.GetAttribute<int>("win.time");

			m_roundnumbers = BuildRoundNumbersSoundMap();
		}

		private ReadOnlyDictionary<int, Elements.Base> BuildRoundNumbersSoundMap()
		{
            var elements = Engine.Elements;

			var roundnumbers = new Dictionary<int, Elements.Base>();
            roundnumbers[1] = elements.GetElement("round1");
            roundnumbers[2] = elements.GetElement("round2");
            roundnumbers[3] = elements.GetElement("round3");
            roundnumbers[4] = elements.GetElement("round4");
            roundnumbers[5] = elements.GetElement("round5");
            roundnumbers[6] = elements.GetElement("round6");
            roundnumbers[7] = elements.GetElement("round7");
            roundnumbers[8] = elements.GetElement("round8");
            roundnumbers[9] = elements.GetElement("round9");

			return new ReadOnlyDictionary<int, Elements.Base>(roundnumbers);
		}

		public Elements.Base GetRoundElement(int roundnumber)
		{
			return m_roundnumbers[roundnumber];
		}

		public void PlaySoundElement(string elementname)
		{
			if (elementname == null) throw new ArgumentNullException(nameof(elementname));

			Engine.Elements.PlaySound(elementname);
		}

		public int NumberOfRounds => m_roundsforwin;

		public int MaximumDrawGames => m_maxdrawgames;

		public int IntroDelay => m_introdelay;

		public int RoundDisplayTime => m_rounddisplaytime;

		public int ControlTime => m_controltime;

		public int KOSlowTime => m_koslowtime;

		public int OverWaitTime => m_overwaittime;

		public int OverHitTime => m_overhittime;

		public int OverWinTime => m_overwintime;

		public int OverTime => m_overtime;

		public int WinTime => m_wintime;

		public Vector2 DefaultElementLocation => m_defaultlocation;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_roundsforwin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_maxdrawgames;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_introdelay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_defaultlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_rounddisplaytime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_controltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_koslowtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_overwaittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_overhittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_overwintime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_overtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_wintime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyDictionary<int, Elements.Base> m_roundnumbers;

		#endregion
	}
}