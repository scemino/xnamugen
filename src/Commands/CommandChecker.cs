using System;

namespace xnaMugen.Commands
{
	internal static class CommandChecker
	{
		public static bool Check(Command command, InputBuffer input)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (input == null) throw new ArgumentNullException(nameof(input));

			var element_index = command.Elements.Count - 1;

			for (var input_index = 0; input_index != input.Size; ++input_index)
			{
				var match_index = ScanForMatch(command, element_index, input, input_index);
				if (match_index == int.MinValue) return false;

				if (element_index > 0)
				{
					if (match_index > command.Time) return false;

					--element_index;
					input_index = match_index;
				}
				else if (element_index == 0)
				{
					return match_index <= command.Time;
				}
				else
				{
					return false;
				}
			}

			return false;
		}

		private static int ScanForMatch(Command command, int element_index, InputBuffer input, int input_index)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (input == null) throw new ArgumentNullException(nameof(input));

			var element = command.Elements[element_index];

			var scanlength = Math.Min(input.Size, command.Time);

			for (var i = input_index; i < scanlength; ++i)
			{
				//Only check for the last element at the top of the input buffer
				if (element_index == command.Elements.Count - 1)
				{
					if (element.TriggerOnRelease == null)
					{
						if (i != input_index) return int.MinValue;
					}
					else
					{
						if (i - 1 != input_index && i != input_index) return int.MinValue;
					}
				}

				if (ElementMatch(element, input, i))
				{
					//If no match, confirm CommandElement.NothingElse before going back a tick in the input.
					if (element_index < command.Elements.Count - 1)
					{
						var nextelement = command.Elements[element_index + 1];
						if (nextelement.NothingElse && input.AreIdentical(input_index, i) == false) continue;
					}

					return i;
				}
			}

			return int.MinValue;
		}

		private static bool ElementMatch(CommandElement element, InputBuffer input, int input_index)
		{
			var state = input.GetState(input_index, element);

			if (element.HeldDown)
			{
				var result = state == ButtonState.Down || state == ButtonState.Pressed;
				return result;
			}

			if (element.TriggerOnRelease != null)
			{
				if (input_index >= input.Size)
				{
					return false;
				}

				if (input_index == 0 || input.GetState(input_index - 1, element) != ButtonState.Released)
				{
					return false;
				}

				var holdcount = 1;
				for (var i = input_index + 1; i < input.Size; ++i, ++holdcount) if (input.GetState(i, element) != ButtonState.Down) break;

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