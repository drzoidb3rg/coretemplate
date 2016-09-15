using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;


namespace DocSupply.Tests
{
    public static class Extensions
    {
        public static T FromJson<T>(this string value)
        {
            return  JsonConvert.DeserializeObject<T>(value);
        }

        public static string JsonPrettify(this string value)
        {
            using (var stringReader = new StringReader(value))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

    }
}
