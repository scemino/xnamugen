using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class CharacterDimensions
	{
		public CharacterDimensions(PlayerConstants constants)
		{
			if (constants == null) throw new ArgumentNullException(nameof(constants));

			m_groundfrontwidth = constants.GroundFront;
			m_groundbackwidth = constants.GroundBack;
			m_airfrontwidth = constants.Airfront;
			m_airbackwidth = constants.Airback;
			m_height = constants.Height;

			m_frontwidthoverride = 0;
			m_backwidthoverride = 0;
			m_frontedgewidthoverride = 0;
			m_backedgewidthoverride = 0;
		}

		public CharacterDimensions(int groundfront, int groundback, int airfront, int airback, int height)
		{
			m_groundfrontwidth = groundfront;
			m_groundbackwidth = groundback;
			m_airfrontwidth = airfront;
			m_airbackwidth = airback;
			m_height = height;

			m_frontwidthoverride = 0;
			m_backwidthoverride = 0;
			m_frontedgewidthoverride = 0;
			m_backedgewidthoverride = 0;
		}

		public void SetOverride(int frontwidth, int backwidth)
		{
			m_frontwidthoverride = frontwidth;
			m_backwidthoverride = backwidth;
		}

		public void SetEdgeOverride(int frontwidth, int backwidth)
		{
			m_frontedgewidthoverride = frontwidth;
			m_backedgewidthoverride = backwidth;
		}

		public void ClearOverride()
		{
			m_frontwidthoverride = 0;
			m_backwidthoverride = 0;
			m_frontedgewidthoverride = 0;
			m_backedgewidthoverride = 0;
		}

		public int GetFrontWidth(StateType statetype)
		{
			var width = m_frontwidthoverride;

			switch (statetype)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(statetype), "Statetype is not valid");

				case StateType.Prone:
				case StateType.Standing:
				case StateType.Crouching:
					width += m_groundfrontwidth;
					break;

				case StateType.Airborne:
					width += m_airfrontwidth;
					break;
			}

			return width;
		}

		public int GetBackWidth(StateType statetype)
		{
			var width = m_backwidthoverride;

			switch (statetype)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(statetype), "Statetype is not valid");

				case StateType.Prone:
				case StateType.Standing:
				case StateType.Crouching:
					width += m_groundbackwidth;
					break;

				case StateType.Airborne:
					width += m_airbackwidth;
					break;
			}

			return width;
		}

		public int Height => m_height;

		public int FrontEdgeWidth => m_frontedgewidthoverride;

		public int BackEdgeWidth => m_backedgewidthoverride;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_groundfrontwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_groundbackwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airfrontwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_airbackwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_height;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_frontwidthoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_backwidthoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_frontedgewidthoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_backedgewidthoverride;

		#endregion
	}
}