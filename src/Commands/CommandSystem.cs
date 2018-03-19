using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;

namespace xnaMugen.Commands
{
	internal class CommandSystem : SubSystem
	{
		public CommandSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_commandcache = new Dictionary<string, ReadOnlyList<Command>>(StringComparer.OrdinalIgnoreCase);
			m_numberregex = new Regex("(\\d+)", RegexOptions.IgnoreCase);
			m_internalcommands = GetCommands("xnaMugen.data.Internal.cmd");

			//Test();
		}

		private void Test()
		{
			var command = BuildCommand("Attack", "~10$B, $F, x", 15, 1);

			var input = new InputBuffer();
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left, Facing.Right);
			input.Add(PlayerButton.Left | PlayerButton.Right, Facing.Right);
			input.Add(PlayerButton.Right, Facing.Right);
			input.Add(PlayerButton.Right | PlayerButton.X, Facing.Right);

			var check = CommandChecker.Check(command, input);
		}

		public ICommandManager CreateManager(PlayerMode mode, string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

            return mode == PlayerMode.Human ?
                                     (ICommandManager)new CommandManager(this, filepath, GetCommands(filepath)) :
                                     new AiCommandManager(this, filepath, GetCommands(filepath));
		}

		private ReadOnlyList<Command> GetCommands(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			if (m_commandcache.ContainsKey(filepath)) return m_commandcache[filepath];

			var commands = new List<Command>();

			var textfile = GetSubSystem<FileSystem>().OpenTextFile(filepath);
			foreach (var textsection in textfile)
			{
				if (string.Equals("Command", textsection.Title, StringComparison.OrdinalIgnoreCase))
				{
					var name = textsection.GetAttribute<string>("name");
					var text = textsection.GetAttribute<string>("command");
					var time = textsection.GetAttribute("time", 15);
					var buffertime = textsection.GetAttribute("Buffer.time", 1);

					commands.Add(BuildCommand(name, text, time, buffertime));
				}
			}

			if (m_internalcommands != null)
			{
				commands.AddRange(m_internalcommands);
			}

			var list = new ReadOnlyList<Command>(commands);
			m_commandcache.Add(filepath, list);

			return list;
		}

		private Command BuildCommand(string name, string text, int time, int buffertime)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (text == null) throw new ArgumentNullException(nameof(text));
			if (time < 0) throw new ArgumentOutOfRangeException(nameof(time));
			if (buffertime < 0) throw new ArgumentOutOfRangeException(nameof(buffertime));

			var command = new Command(name, text, time, buffertime, ParseCommandText(text));
			if (command.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.CommandSystem, "Invalid command - '{0}'", name);
			}

			return command;
		}

		private ReadOnlyList<CommandElement> ParseCommandText(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			var elements = new List<CommandElement>();

			CommandElement lastelement = null;
			foreach (var token in text.Split(','))
			{
				if (string.IsNullOrEmpty(token)) continue;

				var helddown = false;
				var nothingelse = false;
				int? triggertime = null;
				var dir = CommandDirection.None;
				var buttons = CommandButton.None;

				if (token.IndexOf('~') != -1)
				{
					var time = 0;
					var match = m_numberregex.Match(token);
					if (match.Success) int.TryParse(match.Groups[1].Value, out time);

					triggertime = time;
				}

				if (token.IndexOf('/') != -1) helddown = true;

				if (token.IndexOf('>') != -1) nothingelse = true;

				if (token.IndexOf("$B") != -1) dir = CommandDirection.B4Way;
				else if (token.IndexOf("$U") != -1) dir = CommandDirection.U4Way;
				else if (token.IndexOf("$F") != -1) dir = CommandDirection.F4Way;
				else if (token.IndexOf("$D") != -1) dir = CommandDirection.D4Way;
				else if (token.IndexOf("DB") != -1) dir = CommandDirection.DB;
				else if (token.IndexOf("DF") != -1) dir = CommandDirection.DF;
				else if (token.IndexOf("UF") != -1) dir = CommandDirection.UF;
				else if (token.IndexOf("UF") != -1) dir = CommandDirection.UF;
				else if (token.IndexOf("D+B") != -1) dir = CommandDirection.DB;
				else if (token.IndexOf("D+F") != -1) dir = CommandDirection.DF;
				else if (token.IndexOf("U+B") != -1) dir = CommandDirection.UB;
				else if (token.IndexOf("U+F") != -1) dir = CommandDirection.UF;
				else if (token.IndexOf("D") != -1) dir = CommandDirection.D;
				else if (token.IndexOf("F") != -1) dir = CommandDirection.F;
				else if (token.IndexOf("U") != -1) dir = CommandDirection.U;
				else if (token.IndexOf("B") != -1) dir = CommandDirection.B;

				if (token.IndexOf("a") != -1) buttons |= CommandButton.A;
				if (token.IndexOf("b") != -1) buttons |= CommandButton.B;
				if (token.IndexOf("c") != -1) buttons |= CommandButton.C;
				if (token.IndexOf("x") != -1) buttons |= CommandButton.X;
				if (token.IndexOf("y") != -1) buttons |= CommandButton.Y;
				if (token.IndexOf("z") != -1) buttons |= CommandButton.Z;
				if (token.IndexOf("s") != -1) buttons |= CommandButton.Taunt;

				var element = new CommandElement(dir, buttons, triggertime, helddown, nothingelse);

				if (lastelement != null && SuccessiveDirectionCheck(lastelement, element))
				{
					var newelement1 = new CommandElement(element.Direction, CommandButton.None, 0, false, true);
					var newelement2 = new CommandElement(element.Direction, CommandButton.None, null, false, true);

					elements.Add(newelement1);
					elements.Add(newelement2);
				}
				else
				{
					elements.Add(element);
				}

				lastelement = element;
			}

			return new ReadOnlyList<CommandElement>(elements);
		}

		private bool SuccessiveDirectionCheck(CommandElement last, CommandElement now)
		{
			if (last != now) return false;
			if (last.Direction == CommandDirection.None || last.Direction == CommandDirection.B4Way || last.Direction == CommandDirection.D4Way || last.Direction == CommandDirection.F4Way || last.Direction == CommandDirection.U4Way) return false;
			if (last.Buttons != CommandButton.None || last.HeldDown || last.NothingElse || last.TriggerOnRelease != null) return false;

			return true;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<string, ReadOnlyList<Command>> m_commandcache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Regex m_numberregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<Command> m_internalcommands;

		#endregion
	}
}