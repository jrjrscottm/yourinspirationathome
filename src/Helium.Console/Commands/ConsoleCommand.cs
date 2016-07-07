using System;
using System.Collections.Generic;

namespace Helium.Console.Commands
{
    public class ConsoleCommand
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public IEnumerable<string> Aliases { get; set; } = new List<string>();
        public string Usage { get; set; }
        public Action<CommandContext> Action { get; set; }
        public string HelpName { get; set; }
        public string ArgsUsage { get; set; }

        public string Description { get; set; }

        public List<ConsoleCommand> Subcommands { get; set; } = new List<ConsoleCommand>();
        public ICollection<Flag.Flag> Flags { get; set; } = new List<Flag.Flag>();
    }
}