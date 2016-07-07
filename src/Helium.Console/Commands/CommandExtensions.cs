using System.Linq;

namespace Helium.Console.Commands
{
    public static class CommandExtensions
    {
        public static bool HasName(this ConsoleCommand consoleCommand, string name)
        {
            return consoleCommand.Name == name 
                   || consoleCommand.ShortName == name 
                   || consoleCommand.Aliases.Contains(name);
        }
    }
}