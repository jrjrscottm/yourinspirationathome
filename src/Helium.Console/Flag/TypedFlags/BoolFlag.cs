using System;
using Helium.Console.Application;

namespace Helium.Console.Flag.TypedFlags
{
    public class BoolFlag : Flag<bool>
    {
        public override void Set(string value)
        {
            Value = bool.Parse(value);
        }

        internal override void Apply(FlagSet set)
        {
            if (!string.IsNullOrEmpty(EnvironmentVariable))
            {
                foreach (var variable in EnvironmentVariable.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var envVar = variable.Trim();
                    var envVal = Environment.GetEnvironmentVariable(envVar);
                    if (string.IsNullOrEmpty(envVal)) continue;

                    bool envValBool;

                    if (bool.TryParse(envVal, out envValBool))
                    {
                        Value = envValBool;
                        break;
                    }
                }
            }

            if (!set.HasFlag(this))
            {
                set.AddFlag(this);
            }
        }
    }
}