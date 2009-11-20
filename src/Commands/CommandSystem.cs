using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	class CommandSystem : SubSystem
	{
		public CommandSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_commandcache = new Dictionary<String, ReadOnlyList<Command>>(StringComparer.OrdinalIgnoreCase);
			m_numberregex = new Regex("(\\d+)", RegexOptions.IgnoreCase);
			m_internalcommands = GetCommands("xnaMugen.data.Internal.cmd");

			//Test();
		}

		void Test()
		{
			Command command = BuildCommand("Attack", "~10$B, $F, x", 15, 1);

			InputBuffer input = new InputBuffer();
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

			Boolean check = CommandChecker.Check(command, input);
		}

		public CommandManager CreateManager(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			return new CommandManager(this, filepath, GetCommands(filepath));
		}

		ReadOnlyList<Command> GetCommands(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			if (m_commandcache.ContainsKey(filepath) == true) return m_commandcache[filepath];

			List<Command> commands = new List<Command>();

			TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile(filepath);
			foreach (TextSection textsection in textfile)
			{
				if (String.Equals("Command", textsection.Title, StringComparison.OrdinalIgnoreCase) == true)
				{
					String name = textsection.GetAttribute<String>("name");
					String text = textsection.GetAttribute<String>("command");
					Int32 time = textsection.GetAttribute<Int32>("time", 15);
					Int32 buffertime = textsection.GetAttribute<Int32>("Buffer.time", 1);

					commands.Add(BuildCommand(name, text, time, buffertime));
				}
			}

			if (m_internalcommands != null)
			{
				commands.AddRange(m_internalcommands);
			}

			ReadOnlyList<Command> list = new ReadOnlyList<Command>(commands);
			m_commandcache.Add(filepath, list);

			return list;
		}

		Command BuildCommand(String name, String text, Int32 time, Int32 buffertime)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (text == null) throw new ArgumentNullException("text");
			if (time < 0) throw new ArgumentOutOfRangeException("time");
			if (buffertime < 0) throw new ArgumentOutOfRangeException("buffertime");

			Command command = new Command(name, text, time, buffertime, ParseCommandText(text));
			if (command.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.CommandSystem, "Invalid command - '{0}'", name);
			}

			return command;
		}

		ReadOnlyList<CommandElement> ParseCommandText(String text)
		{
			if (text == null) throw new ArgumentNullException("text");

			List<CommandElement> elements = new List<CommandElement>();

			CommandElement lastelement = null;
			foreach (String token in text.Split(','))
			{
				if (String.IsNullOrEmpty(token) == true) continue;

				Boolean helddown = false;
				Boolean nothingelse = false;
				Int32? triggertime = null;
				CommandDirection dir = CommandDirection.None;
				CommandButton buttons = CommandButton.None;

				if (token.IndexOf('~') != -1)
				{
					Int32 time = 0;
					Match match = m_numberregex.Match(token);
					if (match.Success == true) Int32.TryParse(match.Groups[1].Value, out time);

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

				CommandElement element = new CommandElement(dir, buttons, triggertime, helddown, nothingelse);

				if (lastelement != null && SuccessiveDirectionCheck(lastelement, element) == true)
				{
					CommandElement newelement1 = new CommandElement(element.Direction, CommandButton.None, 0, false, true);
					CommandElement newelement2 = new CommandElement(element.Direction, CommandButton.None, null, false, true);

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

		Boolean SuccessiveDirectionCheck(CommandElement last, CommandElement now)
		{
			if (last != now) return false;
			if (last.Direction == CommandDirection.None || last.Direction == CommandDirection.B4Way || last.Direction == CommandDirection.D4Way || last.Direction == CommandDirection.F4Way || last.Direction == CommandDirection.U4Way) return false;
			if (last.Buttons != CommandButton.None || last.HeldDown == true || last.NothingElse == true || last.TriggerOnRelease != null) return false;

			return true;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<String, ReadOnlyList<Command>> m_commandcache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Regex m_numberregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<Command> m_internalcommands;

		#endregion
	}
}