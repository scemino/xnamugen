using System;
using System.Collections.Generic;
using xnaMugen.Collections;
using System.Linq;

namespace xnaMugen.Commands
{
    internal class AiCommandManager : ICommandManager
    {
        private CommandSystem m_commandsystem;
        private string m_filepath;
        private ReadOnlyList<Command> m_commands;
        private List<string> m_activecommands;
        private System.Random m_random;

        public AiCommandManager(CommandSystem commandsystem, string filepath, ReadOnlyList<Command> commands)
        {
            if (commandsystem == null) throw new ArgumentNullException(nameof(commandsystem));
            if (filepath == null) throw new ArgumentNullException(nameof(filepath));
            if (commands == null) throw new ArgumentNullException(nameof(commands));

            m_commandsystem = commandsystem;
            m_filepath = filepath;
            m_commands = commands;
            m_activecommands = new List<string>();
            m_random = new System.Random();
        }

        public ICommandManager Clone()
        {
            return new AiCommandManager(m_commandsystem, m_filepath, m_commands);
        }

        public bool IsActive(string commandname)
        {
            return m_activecommands.Contains(commandname);
        }

        public void Reset()
        {
            m_activecommands.Clear();
        }

        public void Update(PlayerButton input, Facing facing, bool paused)
        {
            Reset();
            var rnd = m_random.Next(0, 100);
            if (rnd > 85)
            {
                var validCommands = m_commands.Where(c => c.IsValid).ToList();
                var cmdIndex = m_random.Next(0, validCommands.Count);
                m_activecommands.Add(validCommands[cmdIndex].Name);
            }
        }
    }
}