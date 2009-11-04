using System;
using xnaMugen.Collections;
using System.Diagnostics;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("Size = {m_buffer.Size}")]
	[DebuggerTypeProxy(typeof(InputBuffer.DebuggerProxy))]
	class InputBuffer
	{
		class DebuggerProxy
		{
			public DebuggerProxy(InputBuffer data)
			{
				m_data = data.m_buffer;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public PlayerButton[] Items
			{
				get
				{
					return m_data.GetCurrentReversedBuffer();
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly CircularBuffer<PlayerButton> m_data;
		}

		public InputBuffer()
		{
			m_buffer = new CircularBuffer<PlayerButton>(120);
			m_size = m_buffer.Size;
		}

		public void Reset()
		{
			m_buffer.Clear();
			m_size = m_buffer.Size;
		}

		public void Add(PlayerButton input, Facing facing)
		{
			if (facing == Facing.Left)
			{
				if (((input & PlayerButton.Left) == PlayerButton.Left) != ((input & PlayerButton.Right) == PlayerButton.Right))
				{
					input ^= PlayerButton.Left;
					input ^= PlayerButton.Right;
				}
			}

			if (((input & PlayerButton.Up) == PlayerButton.Up) && ((input & PlayerButton.Down) == PlayerButton.Down))
			{
				input &= ~PlayerButton.Up;
				input &= ~PlayerButton.Down;
			}

			if (((input & PlayerButton.Left) == PlayerButton.Left) && ((input & PlayerButton.Right) == PlayerButton.Right))
			{
				input &= ~PlayerButton.Left;
				input &= ~PlayerButton.Right;
			}

			m_buffer.Add(input);
			m_size = m_buffer.Size;
		}

		public ButtonState GetState(Int32 index, CommandElement element)
		{
			if (index < 0 || index >= Size) throw new ArgumentNullException("index");
			if (element == null) throw new ArgumentNullException("element");

			PlayerButton current = m_buffer.ReverseGet(index);
			PlayerButton previous = (index != Size - 1) ? m_buffer.ReverseGet(index + 1) : 0;

			Boolean current_state = GetButtonState(current, element);
			Boolean previous_state = GetButtonState(previous, element);

			if (current_state)
			{
				return previous_state ? ButtonState.Down : ButtonState.Pressed;
			}
			else
			{
				return previous_state ? ButtonState.Released : ButtonState.Up;
			}
		}

		public Boolean AreIdentical(Int32 start, Int32 end)
		{
			if (start < 0 || start >= Size) throw new ArgumentOutOfRangeException("start");
			if (end < 0 || end >= Size) throw new ArgumentOutOfRangeException("end");

			PlayerButton match = m_buffer.ReverseGet(start);
			for (Int32 i = start + 1; i < end; ++i) if (match != m_buffer.ReverseGet(i)) return false;

			return true;
		}

		Boolean GetButtonState(PlayerButton inputelement, CommandElement commandelement)
		{
			return ((inputelement & commandelement.MatchHash1) == commandelement.MatchHash1) && ((inputelement & commandelement.MatchHash2) == 0);
		}

		public Int32 Size
		{
			get { return m_size; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CircularBuffer<PlayerButton> m_buffer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_size;

		#endregion
	}
}
