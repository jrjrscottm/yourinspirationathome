using System;
using System.Collections.Generic;
using System.Linq;
using Helium.Console.Flag;

namespace Helium.Console.Application
{
    internal static class FlagExtensions
    {
        internal static void NormalizeFlags(this IEnumerable<Flag.Flag> flags, FlagSet flagSet)
        {
            foreach (var flag in flags)
            {
                var parts = flag.Name.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    if (!flagSet.Actual.ContainsKey(parts[0]))
                    {
                        flagSet.Set(parts[0], flag.Value);
                    }
                    continue;
                }

                Flag.Flag f = null;
                foreach (var part in parts)
                {
                    var name = part.Trim();
                    if (flagSet.Actual.ContainsKey(name))
                    {
                        if (f != null)
                        {
                            throw new BadSyntaxException($"Can't use two forms of the same flag: {name} {f.Name}");
                        }
                        f = flagSet.Formal[name];
                    }
                }

                if (f == null)
                {
                    continue;
                }

                foreach (var part in parts)
                {
                    var name = part.Trim();
                    if (!flagSet.Actual.ContainsKey(name))
                    {
                        flagSet.Set(name, f.Value);
                    }
                }
            }   
        }

        internal static bool HasFlag(this FlagSet flagSet, Flag.Flag flag)
        {
            return flagSet.Formal.Any(f => f.Value == flag);
        }
    }
}