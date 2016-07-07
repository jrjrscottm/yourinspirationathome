using System.Collections.Generic;
using Helium.Console.Flag;

namespace Helium.Console.Commands
{
    public class CommandContext
    {
        internal CommandContext(Application.Application app, FlagSet flagSet, CommandContext parentContext = null)
        {
            Application = app;
            FlagSet = flagSet;
            ParentContext = parentContext;
        }
        public Application.Application Application { get; }
        public ConsoleCommand ConsoleCommand { get; internal set; }

        public CommandContext ParentContext { get; set; }

        public List<string> Args()
        {
            return FlagSet.Args;
        }
        internal FlagSet FlagSet { get; set; }

        public T Get<T>(string name)
        {
            var context = this;
            while (context != null)
            {
                Flag.Flag flag;
                if (context.FlagSet.Formal.TryGetValue(name, out flag))
                {
                    return ((Flag<T>) flag).Value;
                }

                context = context.ParentContext;
            }

            return default(T);
        }

        public string Get(string name)
        {
            var context = this;
            while (context != null)
            {
                Flag.Flag flag;
                if (context.FlagSet.Formal.TryGetValue(name, out flag))
                {
                    return flag.Value;
                }

                context = context.ParentContext;
            }
            return null;
        }
    }
}