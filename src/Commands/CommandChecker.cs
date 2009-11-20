using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	static class CommandChecker
	{
		public static Boolean Check(Command command, InputBuffer input)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (input == null) throw new ArgumentNullException("input");

			Int32 element_index = command.Elements.Count - 1;

			for (Int32 input_index = 0; input_index != input.Size; ++input_index)
			{
				Int32 match_index = ScanForMatch(command, element_index, input, input_index);
				if (match_index == Int32.MinValue) return false;

				if (element_index > 0)
				{
					if (match_index > command.Time) return false;

					--element_index;
				}
				else if (element_index == 0)
				{
					return match_index <= command.Time;
				}
				else
				{
					return false;
				}

				input_index = match_index;
			}

			return false;
		}

		static Int32 ScanForMatch(Command command, Int32 element_index, InputBuffer input, Int32 input_index)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (input == null) throw new ArgumentNullException("input");

			CommandElement element = command.Elements[element_index];

			Int32 scanlength = Math.Min(input.Size, command.Time);

			for (Int32 i = input_index; i < scanlength; ++i)
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
						if (nextelement.NothingElse == true && input.AreIdentical(input_index, i) == false) continue;
					}

					return i;
				}
			}

			return Int32.MinValue;
		}

		static Boolean ElementMatch(CommandElement element, InputBuffer input, Int32 input_index)
		{
			ButtonState state = input.GetState(input_index, element);

			if (element.HeldDown == true)
			{
				Boolean result = (state == ButtonState.Down || state == ButtonState.Pressed);
				return result;
			}
			else if (element.TriggerOnRelease != null)
			{
				if (input_index >= input.Size)
				{
					return false;
				}

				if (input_index == 0 || input.GetState(input_index - 1, element) != ButtonState.Released)
				{
					return false;
				}

				Int32 holdcount = 1;
				for (Int32 i = input_index + 1; i < input.Size; ++i, ++holdcount) if (input.GetState(i, element) != ButtonState.Down) break;

				if (holdcount < element.TriggerOnRelease.Value)
				{
					return false;
				}
			}
			else
			{
				if (state != ButtonState.Pressed)
				{
					return false;
				}
			}

			return true;
		}
	}
}