using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	struct CommandElement: IEquatable<CommandElement>
	{
		public CommandElement(CommandDirection direction, CommandButton button, Int32? triggertime, Boolean helddown, Boolean nothingelse)
		{
			m_direction = direction;
			m_button = button;
			m_triggeronrelease = triggertime;
			m_helddown = helddown;
			m_nothingelse = nothingelse;
		}

		static public Boolean Equals(CommandElement lhs, CommandElement rhs)
		{
			return lhs == rhs;
		}

		public Boolean Equals(CommandElement other)
		{
			return this == other;
		}

		public override Boolean Equals(Object obj)
		{
			if (obj == null) return false;
			if (obj is CommandElement == false) return false;

			return this == (CommandElement)obj;
		}

		public static Boolean operator ==(CommandElement lhs, CommandElement rhs)
		{
			if (lhs == null && rhs == null) return true;
			if ((lhs == null) != (rhs == null)) return false;

			if (lhs.Direction != rhs.Direction) return false;
			if (lhs.Buttons != rhs.Buttons) return false;
			if (lhs.TriggerOnRelease != rhs.TriggerOnRelease) return false;
			if (lhs.HeldDown != rhs.HeldDown) return false;
			if (lhs.NothingElse != rhs.NothingElse) return false;

			return true;
		}

		public static Boolean operator !=(CommandElement lhs, CommandElement rhs)
		{
			return !(lhs == rhs);
		}

		public override Int32 GetHashCode()
		{
			return Direction.GetHashCode() ^ Buttons.GetHashCode() ^ TriggerOnRelease.GetValueOrDefault(0).GetHashCode() ^ HeldDown.GetHashCode() ^ NothingElse.GetHashCode();
		}

		public CommandDirection Direction
		{
			get { return m_direction; }
		}

		public CommandButton Buttons
		{
			get { return m_button; }
		}

		public Int32? TriggerOnRelease
		{
			get { return m_triggeronrelease; }
		}

		public Boolean HeldDown
		{
			get { return m_helddown; }
		}

		public Boolean NothingElse
		{
			get { return m_nothingelse; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CommandDirection m_direction;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CommandButton m_button;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32? m_triggeronrelease;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_helddown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_nothingelse;

		#endregion
	}
}