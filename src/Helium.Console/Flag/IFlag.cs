namespace Helium.Console.Flag
{
    public interface IFlag
    {
        string Value { get; set; }
        void Set(string value);
    }

    public interface IFlag<T>
    {
        T Value { get; set; }
        
    }
}