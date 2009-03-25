using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using xnaMugen.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace xnaMugen.Commands
{
	[DebuggerDisplay("{Filepath}")]
	class CommandManager
	{
		public CommandManager(CommandSystem commandsystem, String filepath, ReadOnlyList<Command> commands)
		{
			if (commandsystem == null) throw new ArgumentNullException("commandsystem");
			if (filepath == null) throw new ArgumentNullException("filepath");
			if (commands == null) throw new ArgumentNullException("commands");

			m_commandsystem = commandsystem;
			m_filepath = filepath;
			m_commands = commands;
			m_activecommands = new Dictionary<String, BufferCount>(StringComparer.Ordinal);
			m_inputbuffer = new InputBuffer();

			foreach (Command command in Commands)
			{
				if (m_activecommands.ContainsKey(command.Name) == true) continue;

				m_activecommands.Add(command.Name, new BufferCount());
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
		}

		public void Update(ButtonArray input, Facing facing, Boolean paused)
		{
			m_inputbuffer.Add(input, facing);

			if (paused == false)
			{
				foreach (BufferCount count in m_activecommands.Values) count.Tick();
			}

			foreach (Command command in Commands)
			{
				if (command.IsValid == false) continue;

				if (m_commandsystem.CheckCommand(command, m_inputbuffer) == true)
				{
					Int32 time = command.BufferTime;
					if (paused == true) ++time;

					m_activecommands[command.Name].Set(time);
				}
			}
		}

		public Boolean IsActive(String commandname)
		{
			if (commandname == null) throw new ArgumentNullException("commandname");

			if (m_activecommands.ContainsKey(commandname) == false) return false;

			Boolean active = m_activecommands[commandname].IsActive;
			return active;
		}

		ReadOnlyList<Command> Commands
		{
			get { return m_commands; }
		}

		public String Filepath
		{
			get { return m_filepath; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CommandSystem m_commandsystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<Command> m_commands;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<String, BufferCount> m_activecommands;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly InputBuffer m_inputbuffer;

		#endregion
	}
}