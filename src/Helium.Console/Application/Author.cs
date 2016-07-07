namespace Helium.Console.Application
{
    public class Author
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Email) 
                ? $"{Name}" 
                : $"{Name} <{Email}>";
        }
    }
}