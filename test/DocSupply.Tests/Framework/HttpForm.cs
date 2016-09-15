using System.Collections.Generic;
using System.Net.Http;

namespace DocSupply.Tests
{
    public class HttpForm
    {
        private List<KeyValuePair<string, string>> data { get; set; }

        public HttpForm()
        {
            data = new List<KeyValuePair<string, string>>();
        }

        public HttpForm Add(string key, string value)
        {
            data.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public FormUrlEncodedContent AsFormContent()
        {
            return new FormUrlEncodedContent(data);
        }
    }
}