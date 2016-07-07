using System;
using System.Linq;

namespace Helium.Console.Flag
{
    public abstract class Flag : IFlag
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Usage { get; set; }
        public string EnvironmentVariable { get; set; }
        public string DefaultValue { get; set; }

        internal abstract void Apply(FlagSet set);

        public static bool operator ==(Flag left, Flag right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }
            return left.Name == right.Name && left.Usage == right.Usage;
        }

        public static bool operator !=(Flag left, Flag right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            var flag = obj as Flag;
            if (flag == null) return false;

            return Name == flag.Name && Usage == flag.Usage;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Convert.ToInt32(string.Join("", (Name + Usage).Select(Convert.ToInt32)));
        }

        public string Value { get; set; }
        public abstract void Set(string value);

        //TODO: Hidden
    }

    public abstract class Flag<T> : Flag, IFlag<T>, IFlag
    {
        string IFlag.Value { get { return Value.ToString(); } set {Set(value);} }

        private T _value;
        private T _defaultValue;
        public new T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                base.Value = value?.ToString();
            }
        }

        public new T DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                base.DefaultValue = value?.ToString();
            }
        }
    }
}