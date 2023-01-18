using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZymeToolbox.Core
{
    public static class ExtensionMethods
    {
        public static string ToString(this JsonElement element, bool indent)
        {
            return element.ValueKind == JsonValueKind.Undefined ? "" : JsonSerializer.Serialize(element, new JsonSerializerOptions { WriteIndented = indent });
        }

        public static void ThrowIfNull<T>(this T? value, string paramName)
        {
            if (value is null)
                throw new ArgumentNullException(paramName);
        }

        public static string StripHTMLTags(this string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}
