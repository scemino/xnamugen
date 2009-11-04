using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	class CommandElement : IEquatable<CommandElement>
	{
		public CommandElement(CommandDirection direction, CommandButton button, Int32? triggertime, Boolean helddown, Boolean nothingelse)
		{
			Direction = direction;
			Buttons = button;
			TriggerOnRelease = triggertime;
			HeldDown = helddown;
			NothingElse = nothingelse;
			Hash = Direction.GetHashCode() ^ Buttons.GetHashCode() ^ TriggerOnRelease.GetValueOrDefault(0).GetHashCode() ^ HeldDown.GetHashCode() ^ NothingElse.GetHashCode();
			MatchHash1 = BuildHash1();
			MatchHash2 = BuildHash2();
		}

		PlayerButton BuildHash1()
		{
			PlayerButton hash = 0;

			switch (Direction)
			{
				case CommandDirection.B4Way:
				case CommandDirection.B:
					hash |= PlayerButton.Left;
					break;

				case CommandDirection.DB:
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Left;
					break;

				case CommandDirection.D4Way:
				case CommandDirection.D:
					hash |= PlayerButton.Down;
					break;

				case CommandDirection.DF:
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.F4Way:
				case CommandDirection.F:
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.UF:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.U4Way:
				case CommandDirection.U:
					hash |= PlayerButton.Up;
					break;

				case CommandDirection.UB:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Left;
					break;

				case CommandDirection.None:
				default:
					break;
			}

			if ((Buttons & CommandButton.A) == CommandButton.A) hash |= PlayerButton.A;
			if ((Buttons & CommandButton.B) == CommandButton.B) hash |= PlayerButton.B;
			if ((Buttons & CommandButton.C) == CommandButton.C) hash |= PlayerButton.C;
			if ((Buttons & CommandButton.X) == CommandButton.X) hash |= PlayerButton.X;
			if ((Buttons & CommandButton.Y) == CommandButton.Y) hash |= PlayerButton.Y;
			if ((Buttons & CommandButton.Z) == CommandButton.Z) hash |= PlayerButton.Z;
			if ((Buttons & CommandButton.Taunt) == CommandButton.Taunt) hash |= PlayerButton.Taunt;

			return hash;
		}

		PlayerButton BuildHash2()
		{
			PlayerButton hash = 0;

			switch (Direction)
			{
				case CommandDirection.B:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.DB:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.D:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Left;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.DF:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Left;
					break;

				case CommandDirection.F:
					hash |= PlayerButton.Up;
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Left;
					break;

				case CommandDirection.UF:
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Left;
					break;

				case CommandDirection.U:
					hash |= PlayerButton.Left;
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.UB:
					hash |= PlayerButton.Down;
					hash |= PlayerButton.Right;
					break;

				case CommandDirection.B4Way:
				case CommandDirection.U4Way:
				case CommandDirection.F4Way:
				case CommandDirection.D4Way:
				case CommandDirection.None:
				default:
					break;
			}

			return hash;
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
			Boolean ll = Object.ReferenceEquals(lhs, null);
			Boolean rr = Object.ReferenceEquals(rhs, null);

			if (ll && rr) return true;
			if (ll != rr) return false;

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
			return Hash;
		}

		public readonly CommandDirection Direction;

		public readonly CommandButton Buttons;

		public readonly Int32? TriggerOnRelease;

		public readonly Boolean HeldDown;

		public readonly Boolean NothingElse;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public readonly PlayerButton MatchHash1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public readonly PlayerButton MatchHash2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 Hash;
	}
}