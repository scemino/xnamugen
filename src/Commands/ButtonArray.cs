using System;
using System.Diagnostics;
using System.Text;

namespace xnaMugen.Commands
{
	struct ButtonArray: IEquatable<ButtonArray>
	{
		static ButtonArray()
		{
			s_numberofbuttons = 12;
		}

		public static Int32 NumberOfButtons
		{
			get { return s_numberofbuttons; }
		}

		public Boolean Equals(ButtonArray other)
		{
			return this == other;
		}

		public override Boolean Equals(Object obj)
		{
			if (obj == null || obj is ButtonArray == false) return false;

			return this == (ButtonArray)obj;
		}

		public static Boolean operator ==(ButtonArray lhs, ButtonArray rhs)
		{
			for (Int32 i = 1; i != NumberOfButtons; ++i)
			{
				if (lhs[i] != rhs[i]) return false;
			}

			return true;
		}

		public static Boolean operator !=(ButtonArray lhs, ButtonArray rhs)
		{
			return !(lhs == rhs);
		}

		public override Int32 GetHashCode()
		{
			return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ Up.GetHashCode() ^ Down.GetHashCode() ^ Left.GetHashCode() ^ Right.GetHashCode() ^ Taunt.GetHashCode();
		}

		public override String ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(Left ? "B" : " ");
			builder.Append(Up ? "U" : " ");
			builder.Append(Right ? "F" : " ");
			builder.Append(Down ? "D" : "  ");
			builder.Append(A ? "A" : " ");
			builder.Append(B ? "B" : " ");
			builder.Append(C ? "C" : " ");
			builder.Append(X ? "X" : " ");
			builder.Append(Y ? "Y" : " ");
			builder.Append(Z ? "Z " : "  ");
			builder.Append(Taunt ? "Taunt " : "");
			builder.Append(Pause ? "Pause" : "");

			return builder.ToString();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Boolean this[PlayerButton button]
		{
			[DebuggerStepThrough]
			get
			{
				switch (button)
				{
					case PlayerButton.A:
						return A;

					case PlayerButton.B:
						return B;

					case PlayerButton.C:
						return C;

					case PlayerButton.X:
						return X;

					case PlayerButton.Y:
						return Y;

					case PlayerButton.Z:
						return Z;

					case PlayerButton.Up:
						return Up;

					case PlayerButton.Down:
						return Down;

					case PlayerButton.Left:
						return Left;

					case PlayerButton.Right:
						return Right;

					case PlayerButton.Taunt:
						return Taunt;

					case PlayerButton.Pause:
						return Pause;

					case PlayerButton.None:
					default:
						throw new ArgumentOutOfRangeException("Button is not a valid PlayerButton");
				}
			}

			[DebuggerStepThrough]
			set
			{
				switch (button)
				{
					case PlayerButton.A:
						A = value;
						break;

					case PlayerButton.B:
						B = value;
						break;

					case PlayerButton.C:
						C = value;
						break;

					case PlayerButton.X:
						X = value;
						break;

					case PlayerButton.Y:
						Y = value;
						break;

					case PlayerButton.Z:
						Z = value;
						break;

					case PlayerButton.Up:
						Up = value;
						break;

					case PlayerButton.Down:
						Down = value;
						break;

					case PlayerButton.Left:
						Left = value;
						break;

					case PlayerButton.Right:
						Right = value;
						break;

					case PlayerButton.Taunt:
						Taunt = value;
						break;

					case PlayerButton.Pause:
						Pause = value;
						break;

					case PlayerButton.None:
					default:
						throw new ArgumentOutOfRangeException("Button is not a valid PlayerButton");
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Boolean this[Int32 buttonindex]
		{
			get { return this[(PlayerButton)buttonindex]; }

			set { this[(PlayerButton)buttonindex] = value; }
		}

		public Boolean A
		{
			get { return m_A; }

			set { m_A = value; }
		}

		public Boolean B
		{
			get { return m_B; }

			set { m_B = value; }
		}

		public Boolean C
		{
			get { return m_C; }

			set { m_C = value; }
		}

		public Boolean X
		{
			get { return m_X; }

			set { m_X = value; }
		}

		public Boolean Y
		{
			get { return m_Y; }

			set { m_Y = value; }
		}

		public Boolean Z
		{
			get { return m_Z; }

			set { m_Z = value; }
		}

		public Boolean Up
		{
			get { return m_up; }

			set { m_up = value; }
		}

		public Boolean Down
		{
			get { return m_down; }

			set { m_down = value; }
		}

		public Boolean Left
		{
			get { return m_left; }

			set { m_left = value; }
		}

		public Boolean Right
		{
			get { return m_right; }

			set { m_right = value; }
		}

		public Boolean Pause
		{
			get { return m_pause; }

			set { m_pause = value; }
		}

		public Boolean Taunt
		{
			get { return m_taunt; }

			set { m_taunt = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static readonly Int32 s_numberofbuttons;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_A;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_B;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_C;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_X;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_Y;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_Z;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_up;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_down;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_left;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_right;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_taunt;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_pause;

		#endregion
	}
}