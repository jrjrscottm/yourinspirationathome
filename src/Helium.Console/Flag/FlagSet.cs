using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helium.Console.Flag.Exception;
using Helium.Console.Flag.TypedFlags;

namespace Helium.Console.Flag
{
    internal class FlagSet
    {
        public FlagSet(string name)
        {
            Name = name;
        }

        internal string Name { get; set; }
        internal bool Parsed { get; set; }
        internal Dictionary<string, Flag> Actual { get; set; } = new Dictionary<string, Flag>();
        internal Dictionary<string, Flag> Formal { get; set; } = new Dictionary<string, Flag>();
        internal List<string> Args { get; set; }
        internal StreamWriter Output { get; set; }

        internal void AddFlag<T>(Flag<T> flag)
        {
            foreach (var name in flag.Name.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries))
            {
                Formal.Add(name.Trim(), flag);
            }
        }

        internal void Set(string name, string value)
        {
            if (!Formal.ContainsKey(name))
            {
                throw new MissingFlagException(name);
            }

            var flag = Formal[name];
            flag.Set(value);
            Actual[name] = flag;
        }

        internal void Set<T>(string name, T value)
        {
            if (!Formal.ContainsKey(name))
            {
                throw new MissingFlagException(name);
            }
            var flag = Formal[name];

            ((IFlag<T>) flag).Value = value;

            Actual[name] = flag;
        }

        public void Parse(params string[] args)
        {
            Parsed = true;
            Args = args.ToList();
            var cont = true;
            while (cont)
            {
                try
                {
                    cont = ParseOne(args.ToList());
                }
                catch (System.Exception)
                {
                    switch (ErrorHandling)
                    {
                        case ErrorHandlingType.ContinueOnError:
                            return;
                        case ErrorHandlingType.ExceptionOnError:
                            throw;
                        case ErrorHandlingType.ExitOnError:
                            Environment.Exit(2);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public ErrorHandlingType ErrorHandling { get; set; }

        private bool ParseOne(List<string> args)
        {
            if (args.Count == 0) return false;

            var s = args[0];
            if (s.Length == 0 || s[0] != '-' || s.Length == 1)
            {
                return ParseOne(args.Skip(1).ToList());
            }

            var numMinuses = 1;

            if (s[1] == '-')
            {
                numMinuses++;
                if (s.Length == 2)
                {
                    return ParseOne(args.Skip(1).ToList());
                }
            }

            var name = s.Substring(numMinuses);
            if (name.Length == 0 || name[0] == '-' || name[0] == '=')
            {
                throw new BadSyntaxException(s);
            }

            args = args.Skip(1).ToList();
            var hasValue = false;

            var value = "";

            for (var i = 1; i < name.Length; i++)
            {
                if (name[i] == '=')
                {
                    value = name.Substring(i + 1);
                    hasValue = true;
                    name = name.Substring(0, i);
                    break;
                }
            }

            Flag flag;
            if (Formal.TryGetValue(name, out flag))
            {
                if (flag is BoolFlag)
                {
                    if (hasValue)
                    {
                        try
                        {
                            flag.Set(value);
                        }
                        catch (InvalidCastException)
                        {
                            throw new BadSyntaxException($"Invalid boolean value {value} for -{s}");
                        }
                    }
                    else
                    {
                        flag.Set("true");
                    }
                }
                else
                {
                    if (!hasValue && Args.Count > 0)
                    {
                        hasValue = true;
                        value = args[0];
                        args = args.Skip(1).ToList();
                    }
                    if (!hasValue)
                    {
                        throw new BadSyntaxException($"Flag needs an argument: -{s}");
                    }
                    flag.Set(value);
                }

                Actual[name] = flag;
            }
            
            return ParseOne(args);
        }
    }

    public class BadSyntaxException : System.Exception
    {
        public BadSyntaxException(string s) : base($"Bad flag syntax: {s}")
        {
        }
    }

    public enum ErrorHandlingType
    {
        ContinueOnError,
        ExitOnError,
        ExceptionOnError
    }
}