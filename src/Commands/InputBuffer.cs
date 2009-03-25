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
			public InputElement[] Items
			{
				get
				{
					return m_data.GetCurrentReversedBuffer();
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly CircularBuffer<InputElement> m_data;
		}

		public InputBuffer()
		{
			m_buffer = new CircularBuffer<InputElement>(120);
		}

		public void Reset()
		{
			m_buffer.Clear();
		}

		public void Add(ButtonArray array, Facing facing)
		{
			m_buffer.Add(new InputElement(array, facing));
		}

		public InputElement Get(Int32 index)
		{
			return m_buffer.ReverseGet(index);
		}

		public ButtonState GetState(Int32 index, CommandElement element)
		{
			if (index < 0 || index >= Size) throw new ArgumentNullException("index");

#if true
			InputElement current = Get(index);
			InputElement previous = (index != Size - 1) ? Get(index + 1) : new InputElement();

			Boolean current_state = GetCommandButtonState(current, element.Buttons) && GetCommandDirectionState(current, element.Direction);
			Boolean previous_state = GetCommandButtonState(previous, element.Buttons) && GetCommandDirectionState(previous, element.Direction);

			if (current_state == false && previous_state == false) return ButtonState.Up;
			else if (current_state == false && previous_state == true) return ButtonState.Released;
			else if (current_state == true && previous_state == false) return ButtonState.Pressed;
			return ButtonState.Down;

#else
			if (element.Direction == CommandDirection.None)
			{
				return GetButtonState(index, element.Buttons);
			}
			else
			{
				return GetButtonState(index, element.Direction);
			}
#endif
		}

		public Boolean AreIdentical(Int32 start, Int32 end)
		{
			if (start < 0 || start >= Size) throw new ArgumentOutOfRangeException("start");
			if (end < 0 || end >= Size) throw new ArgumentOutOfRangeException("end");

			InputElement match = Get(start);
			for (Int32 i = start + 1; i < end; ++i) if (match != Get(i)) return false;

			return true;
		}

		ButtonState GetButtonState(Int32 index, CommandButton button)
		{
			if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException("index");
			if (button == CommandButton.None) throw new ArgumentException("button");

			InputElement current = Get(index);
			InputElement previous = (index != Size - 1) ? Get(index + 1) : new InputElement();

			Boolean current_state = GetCommandButtonState(current, button);
			Boolean previous_state = GetCommandButtonState(previous, button);

			if (current_state == false && previous_state == false) return ButtonState.Up;
			else if (current_state == false && previous_state == true) return ButtonState.Released;
			else if (current_state == true && previous_state == false) return ButtonState.Pressed;
			return ButtonState.Down;
		}

		ButtonState GetButtonState(Int32 index, CommandDirection direction)
		{
			if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException("index");
			if (direction == CommandDirection.None) throw new ArgumentException("direction");

			InputElement current = Get(index);
			InputElement previous = (index != Size - 1) ? Get(index + 1) : new InputElement();

			Boolean current_state = GetCommandDirectionState(current, direction);
			Boolean previous_state = GetCommandDirectionState(previous, direction);

			if (current_state == false && previous_state == false) return ButtonState.Up;
			else if (current_state == false && previous_state == true) return ButtonState.Released;
			else if (current_state == true && previous_state == false) return ButtonState.Pressed;
			return ButtonState.Down;
		}

		Boolean GetCommandButtonState(InputElement element, CommandButton button)
		{
			if (button == CommandButton.None) return true;

			ButtonArray array = element.Buttons;
			Boolean state = true;

			if ((button & CommandButton.A) != 0) state &= array[PlayerButton.A];
			if ((button & CommandButton.B) != 0) state &= array[PlayerButton.B];
			if ((button & CommandButton.C) != 0) state &= array[PlayerButton.C];
			if ((button & CommandButton.X) != 0) state &= array[PlayerButton.X];
			if ((button & CommandButton.Y) != 0) state &= array[PlayerButton.Y];
			if ((button & CommandButton.Z) != 0) state &= array[PlayerButton.Z];
			if ((button & CommandButton.Taunt) != 0) state &= array[PlayerButton.Taunt];

			return state;
		}

		Boolean GetCommandDirectionState(InputElement element, CommandDirection direction)
		{
			Boolean? up, down, left, right;

			switch (direction)
			{
				case CommandDirection.B:
					up = false; down = false; left = true; right = false;
					break;

				case CommandDirection.DB:
					up = false; down = true; left = true; right = false;
					break;

				case CommandDirection.D:
					up = false; down = true; left = false; right = false;
					break;

				case CommandDirection.DF:
					up = false; down = true; left = false; right = true;
					break;

				case CommandDirection.F:
					up = false; down = false; left = false; right = true;
					break;

				case CommandDirection.UF:
					up = true; down = false; left = false; right = true;
					break;

				case CommandDirection.U:
					up = true; down = false; left = false; right = false;
					break;

				case CommandDirection.UB:
					up = true; down = false; left = true; right = false;
					break;

				case CommandDirection.B4Way:
					up = null; down = null; left = true; right = null;
					break;

				case CommandDirection.U4Way:
					up = true; down = null; left = null; right = null;
					break;

				case CommandDirection.F4Way:
					up = null; down = null; left = null; right = true;
					break;

				case CommandDirection.D4Way:
					up = null; down = true; left = null; right = null;
					break;

				case CommandDirection.None:
				default:
					return true;
			}

			if (element.Facing == Facing.Left) Misc.Swap(ref left, ref right);

			if (up != null && up != element.Buttons[PlayerButton.Up]) return false;
			if (down != null && down != element.Buttons[PlayerButton.Down]) return false;
			if (left != null && left != element.Buttons[PlayerButton.Left]) return false;
			if (right != null && right != element.Buttons[PlayerButton.Right]) return false;
			return true;
		}

		public Int32 Size
		{
			get { return m_buffer.Size; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CircularBuffer<InputElement> m_buffer;

		#endregion
	}
}
