using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	class CommandChecker
	{
		public CommandChecker(CommandSystem system)
		{
			if (system == null) throw new ArgumentNullException("system");

			m_system = system;
		}

		public Boolean Check(Command command, InputBuffer input)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (input == null) throw new ArgumentNullException("input");

			Int32 startmatch = Int32.MinValue;
			Int32 element_index = command.Elements.Count - 1;

			for (Int32 input_index = 0; input_index != input.Size; ++input_index)
			{
				Int32 match_index = ScanForMatch(command, element_index, input, input_index);
				if (match_index == Int32.MinValue) return false;

				if (element_index == 0) startmatch = match_index;

				if (element_index > 0) --element_index;
				else
				{
					return startmatch != Int32.MinValue && startmatch <= command.Time;
				}

				input_index = match_index;
			}

			return false;
		}

		Int32 ScanForMatch(Command command, Int32 element_index, InputBuffer input, Int32 input_index)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (input == null) throw new ArgumentNullException("input");

			CommandElement element = command.Elements[element_index];

			for (Int32 i = input_index; i != input.Size; ++i)
			{
				//Only check for the last element at the top of the input buffer
				if (element_index == command.Elements.Count - 1)
				{
					if (element.TriggerOnRelease == null)
					{
						if (i != input_index) return Int32.MinValue;
					}
					else
					{
						if (i - 1 != input_index && i != input_index) return Int32.MinValue;
					}
				}

				if (ElementMatch(element, input, i) == true)
				{
					//If no match, confirm CommandElement.NothingElse before going back a tick in the input.
					if (element_index < command.Elements.Count - 1)
					{
						CommandElement nextelement = command.Elements[element_index + 1];
						if (nextelement.NothingElse == true)
						{
							Boolean scan = input.AreIdentical(input_index, i);
							if (scan == false) continue;
						}
					}

					return i;
				}
			}

			return Int32.MinValue;
		}

		Boolean ElementMatch(CommandElement element, InputBuffer input, Int32 input_index)
		{
			ButtonState state = input.GetState(input_index, element);

			if (element.HeldDown == true)
			{
				return state == ButtonState.Down || state == ButtonState.Pressed;
			}
			else if (element.TriggerOnRelease != null)
			{
				if (input_index >= input.Size) return false;
				if (input_index == 0 || input.GetState(input_index - 1, element) != ButtonState.Released) return false;

				Int32 holdcount = 1;
				for (Int32 i = input_index + 1; i < input.Size; ++i, ++holdcount)
				{
					if (input.GetState(i, element) != ButtonState.Down) break;
				}

				if (holdcount < element.TriggerOnRelease.Value) return false;
			}
			else
			{
				if (state != ButtonState.Pressed) return false;
			}

			return true;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CommandSystem m_system;

		#endregion
	}
}