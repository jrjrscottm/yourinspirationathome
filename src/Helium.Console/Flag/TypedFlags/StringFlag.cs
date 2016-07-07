using System;

namespace Helium.Console.Flag.TypedFlags
{
    public class StringFlag : Flag<string>
    {
        public override void Set(string value)
        {
            Value = value;
        }

        internal override void Apply(FlagSet set)
        {
            if (!string.IsNullOrEmpty(EnvironmentVariable))
            {
                foreach (var variable in EnvironmentVariable.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var envVar = variable.Trim();
                    var envVal = Environment.GetEnvironmentVariable(envVar);
                    if (string.IsNullOrEmpty(envVal)) continue;

                    Value = envVal;
                    break;
                }
            }

            if (string.IsNullOrEmpty(Value))
            {
                Value = DefaultValue;
            }

            set.AddFlag(this);
        }
    }

    public class DecimalFlag : Flag<decimal>
    {
        public override void Set(string value)
        {
            Value = decimal.Parse(value);
        }

        internal override void Apply(FlagSet set)
        {
            if (!string.IsNullOrEmpty(EnvironmentVariable))
            {
                foreach (var variable in EnvironmentVariable.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var envVar = variable.Trim();
                    var envVal = Environment.GetEnvironmentVariable(envVar);
                    if (string.IsNullOrEmpty(envVal)) continue;

                    Value = decimal.Parse(envVal);
                    break;
                }
            }

            if (Value == default(decimal) && DefaultValue != default(decimal))
            {
                Value = DefaultValue;
            }

            set.AddFlag(this);
        }
    }
}