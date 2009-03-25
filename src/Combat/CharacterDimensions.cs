using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Combat
{

	class CharacterDimensions
	{
		public CharacterDimensions(PlayerConstants constants)
		{
			if (constants == null) throw new ArgumentNullException("constants");

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

		public CharacterDimensions(Int32 groundfront, Int32 groundback, Int32 airfront, Int32 airback, Int32 height)
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

		public void SetOverride(Int32 frontwidth, Int32 backwidth)
		{
			m_frontwidthoverride = frontwidth;
			m_backwidthoverride = backwidth;
		}

		public void SetEdgeOverride(Int32 frontwidth, Int32 backwidth)
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

		public Int32 GetFrontWidth(StateType statetype)
		{
			Int32 width = m_frontwidthoverride;

			switch (statetype)
			{
				case StateType.Unchanged:
				default:
					throw new ArgumentOutOfRangeException("statetype", "Statetype is not valid");

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

		public Int32 GetBackWidth(StateType statetype)
		{
			Int32 width = m_backwidthoverride;

			switch (statetype)
			{
				case StateType.Unchanged:
				default:
					throw new ArgumentOutOfRangeException("statetype", "Statetype is not valid");

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

		public Int32 Height
		{
			get { return m_height; }
		}

		public Int32 FrontEdgeWidth
		{
			get { return m_frontedgewidthoverride; }
		}

		public Int32 BackEdgeWidth
		{
			get { return m_backedgewidthoverride; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_groundfrontwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_groundbackwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airfrontwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_airbackwidth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_height;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_frontwidthoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_backwidthoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_frontedgewidthoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_backedgewidthoverride;

		#endregion
	}
}