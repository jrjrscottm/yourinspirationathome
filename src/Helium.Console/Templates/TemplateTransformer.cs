using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Helium.Console.Templates
{
    public class TemplateTransformer
    {
        public static string TransformTemplate<T>(T data, string templateName)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.PropertyType == typeof(string));
            string templateText;
            using (var templateStream =
                new StreamReader(typeof(T).GetTypeInfo().Assembly
                    .GetManifestResourceStream($"Helium.Console.Templates.{templateName}.tpl")))
            {
                templateText = templateStream.ReadToEnd();

                foreach (var property in properties)
                {
                    templateText = templateText.Replace($"{{{{{property.Name}}}}}", (string)property.GetValue(data));
                }
            }

            return templateText;
        }
    }
}
