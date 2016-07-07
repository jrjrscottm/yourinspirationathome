using System.Linq;
using Helium.Console.Commands;

namespace Helium.Console.Application
{
    public static class ApplicationExtensions
    {
        public static ConsoleCommand GetCommand(this Application application, string command)
        {
            return application.Commands.FirstOrDefault(c => CommandExtensions.HasName(c, command));
        }

        public static bool HasFlag(this Application application, Flag.Flag flag)
        {
            return application.Flags.Any(f => f == flag);
        }
    }
}