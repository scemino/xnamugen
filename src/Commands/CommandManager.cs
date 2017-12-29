using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("{" + nameof(Filepath) + "}")]
	internal class CommandManager
	{
		public CommandManager(CommandSystem commandsystem, string filepath, ReadOnlyList<Command> commands)
		{
			if (commandsystem == null) throw new ArgumentNullException(nameof(commandsystem));
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));
			if (commands == null) throw new ArgumentNullException(nameof(commands));

			m_commandsystem = commandsystem;
			m_filepath = filepath;
			m_commands = commands;
			m_commandcount = new Dictionary<string, BufferCount>(StringComparer.Ordinal);
			m_inputbuffer = new InputBuffer();
			m_activecommands = new List<string>();

			foreach (var command in Commands)
			{
				if (m_commandcount.ContainsKey(command.Name)) continue;

				m_commandcount.Add(command.Name, new BufferCount());
			}
		}

		public CommandManager Clone()
		{
			return m_commandsystem.CreateManager(Filepath);
		}

		public void Reset()
		{
			m_activecommands.Clear();
			m_inputbuffer.Reset();

			foreach (var data in m_commandcount) data.Value.Reset();
		}

		public void Update(PlayerButton input, Facing facing, bool paused)
		{
			m_inputbuffer.Add(input, facing);

			if (paused == false)
			{
				foreach (var count in m_commandcount.Values) count.Tick();
			}

			foreach (var command in Commands)
			{
				if (command.IsValid == false) continue;

				if (CommandChecker.Check(command, m_inputbuffer))
				{
					var time = command.BufferTime;
					if (paused) ++time;

					m_commandcount[command.Name].Set(time);
				}
			}

			m_activecommands.Clear();
			foreach (var data in m_commandcount) if (data.Value.IsActive) m_activecommands.Add(data.Key);
		}

		public bool IsActive(string commandname)
		{
			if (commandname == null) throw new ArgumentNullException(nameof(commandname));

			return m_activecommands.Contains(commandname);
		}

		private ReadOnlyList<Command> Commands => m_commands;

		public string Filepath => m_filepath;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CommandSystem m_commandsystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<Command> m_commands;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<string, BufferCount> m_commandcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<string> m_activecommands;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly InputBuffer m_inputbuffer;

		#endregion
	}
}