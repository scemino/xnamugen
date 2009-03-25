using System;
using System.Diagnostics;

namespace xnaMugen
{
	class PlayerCreation
	{
		public PlayerCreation(PlayerProfile profile, Int32 paletteindex)
		{
			if (profile == null) throw new ArgumentNullException("profile");
			if (paletteindex < 0 || paletteindex > 11) throw new ArgumentOutOfRangeException("paletteindex");

			m_profile = profile;
			m_paletteindex = paletteindex;
		}

		public PlayerProfile Profile
		{
			get { return m_profile; }
		}

		public Int32 PaletteIndex
		{
			get { return m_paletteindex; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_paletteindex;

		#endregion
	}
}