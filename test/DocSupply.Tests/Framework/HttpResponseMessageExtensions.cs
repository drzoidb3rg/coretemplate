using System.Net.Http;
using System.Threading.Tasks;

namespace DocSupply.Tests
{
    public static class HttpResponseMessageExtensions
    {

        public static string BodyAsString(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public static T FromJson<T>(this HttpResponseMessage response)
        {
            return response.BodyAsString().FromJson<T>();
        }
    }
}