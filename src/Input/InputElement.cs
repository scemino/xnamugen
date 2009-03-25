using System;
using System.Diagnostics;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("{Facing} {Buttons}")]
	struct InputElement: IEquatable<InputElement>
	{
		public InputElement(ButtonArray array, Facing facing)
		{
			m_buttons = array;
			m_facing = facing;
		}

		public Boolean Equals(InputElement other)
		{
			return this == other;
		}

		public override Boolean Equals(Object obj)
		{
			if (obj == null || obj is InputElement == false) return false;

			return this == (InputElement)obj;
		}

		public static Boolean operator ==(InputElement lhs, InputElement rhs)
		{
			return lhs.Buttons == rhs.Buttons && lhs.Facing == rhs.Facing;
		}

		public static Boolean operator !=(InputElement lhs, InputElement rhs)
		{
			return !(lhs == rhs);
		}

		public override Int32 GetHashCode()
		{
			return Buttons.GetHashCode() ^ Facing.GetHashCode();
		}

		public ButtonArray Buttons
		{
			get { return m_buttons; }
		}

		public Facing Facing
		{
			get { return m_facing; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ButtonArray m_buttons;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Facing m_facing;

		#endregion
	}
}