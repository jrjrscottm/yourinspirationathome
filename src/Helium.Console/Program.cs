using System;

namespace Helium.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Helium.Helium.Create().Run(Read());
            }
        }

        private const string Prompt = "helium> ";

        public static string[] Read(string message = null)
        {
            System.Console.Write($"{Prompt}");
            var text = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(text))
            {
                return text.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            }
            return new string[0];
        }
    }
}
