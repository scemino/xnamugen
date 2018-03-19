using System;
using System.Diagnostics;

namespace xnaMugen
{
	internal class PlayerCreation
	{
        public PlayerCreation(PlayerProfile profile, int paletteindex, PlayerMode mode)
		{
			if (profile == null) throw new ArgumentNullException(nameof(profile));
			if (paletteindex < 0 || paletteindex > 11) throw new ArgumentOutOfRangeException(nameof(paletteindex));

            Mode = mode;
			m_profile = profile;
			m_paletteindex = paletteindex;
		}

        public PlayerMode Mode { get; }

		public PlayerProfile Profile => m_profile;

		public int PaletteIndex => m_paletteindex;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_paletteindex;

		#endregion
	}
}