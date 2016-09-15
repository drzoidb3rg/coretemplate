using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;

namespace DocSupply
{
    public static class Extensions
    {
        public static bool EmptyString(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        public static string ToJson(this object value)
        {
            return  JsonConvert.SerializeObject(value);
        }

        public static T FromJson<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static StringContent ToHttpStringContent(this object value)
        {
           return new StringContent(value.ToJson(), Encoding.UTF8, "application/json");
        }
    }
}
