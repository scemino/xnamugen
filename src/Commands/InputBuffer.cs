using System;
using xnaMugen.Collections;
using System.Diagnostics;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("Size = {m_buffer.Size}")]
	[DebuggerTypeProxy(typeof(DebuggerProxy))]
	internal class InputBuffer
	{
		private class DebuggerProxy
		{
			public DebuggerProxy(InputBuffer data)
			{
				m_data = data.m_buffer;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public PlayerButton[] Items => m_data.GetCurrentReversedBuffer();

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly CircularBuffer<PlayerButton> m_data;
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
				if ((input & PlayerButton.Left) == PlayerButton.Left != ((input & PlayerButton.Right) == PlayerButton.Right))
				{
					input ^= PlayerButton.Left;
					input ^= PlayerButton.Right;
				}
			}

			if ((input & PlayerButton.Up) == PlayerButton.Up && (input & PlayerButton.Down) == PlayerButton.Down)
			{
				input &= ~PlayerButton.Up;
				input &= ~PlayerButton.Down;
			}

			if ((input & PlayerButton.Left) == PlayerButton.Left && (input & PlayerButton.Right) == PlayerButton.Right)
			{
				input &= ~PlayerButton.Left;
				input &= ~PlayerButton.Right;
			}

			m_buffer.Add(input);
			m_size = m_buffer.Size;
		}

		public ButtonState GetState(int index, CommandElement element)
		{
			if (index < 0 || index >= Size) throw new ArgumentNullException(nameof(index));
			if (element == null) throw new ArgumentNullException(nameof(element));

			var current = m_buffer.ReverseGet(index);
			var previous = index != Size - 1 ? m_buffer.ReverseGet(index + 1) : 0;

			var currentState = GetButtonState(current, element);
			var previousState = GetButtonState(previous, element);

			if (currentState)
			{
				return previousState ? ButtonState.Down : ButtonState.Pressed;
			}

			return previousState ? ButtonState.Released : ButtonState.Up;
		}

		public bool AreIdentical(int start, int end)
		{
			if (start < 0 || start >= Size) throw new ArgumentOutOfRangeException(nameof(start));
			if (end < 0 || end >= Size) throw new ArgumentOutOfRangeException(nameof(end));

			var match = m_buffer.ReverseGet(start);
			for (var i = start + 1; i < end; ++i) if (match != m_buffer.ReverseGet(i)) return false;

			return true;
		}

		private bool GetButtonState(PlayerButton inputelement, CommandElement commandelement)
		{
			return (inputelement & commandelement.MatchHash1) == commandelement.MatchHash1 && (inputelement & commandelement.MatchHash2) == 0;
		}

		public int Size => m_size;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CircularBuffer<PlayerButton> m_buffer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_size;

		#endregion
	}
}
