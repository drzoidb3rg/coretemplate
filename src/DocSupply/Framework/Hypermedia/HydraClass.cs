using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocSupply.Framework.Hypermedia
{

    public enum HttpMethod
    {
        GET,
        POST,
        DELETE,
        PUT
    }

    public class SupportingProperty
    {
        public string Property { get; set; }
        public string Range { get; set; }

        public SupportingProperty()
        {
            Range = "text";
            Property = "";
        }

        public SupportingProperty(string property) : this()
        {
            Property = property;
        }
    }

    public class HydraOperation
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpMethod Method { get; set; }

        [JsonProperty(PropertyName = "supportedProperty")]
        public List<SupportingProperty> SupportingPropertyList { get; set; }

        public HydraOperation()
        {
            Method = HttpMethod.GET;
            SupportingPropertyList = new List<SupportingProperty>();
        }
    }

   
    public class HydraContext
    {
        public string Vocab = "https://schema.org/";
        public string Hydra = "http://www.w3.org/ns/hydra/core#";
    }

    public class HydraRepresentation
    {
        [JsonProperty(Order = 1)]
        public HydraContext Context { get; set; }

        public HydraRepresentation()
        {
            Context = new HydraContext();
        }
    }


    public class HydraClass  //: HydraRepresentation
    {
        [JsonProperty(Order = 2)]
        public string Id { get; set; }

        [JsonProperty(Order = 3)]
        public string Type { get; set; }

        [JsonProperty(Order = 4)]
        public string Title { get; set; }

        [JsonProperty(Order = 5, PropertyName = "operation")]

        public List<HydraOperation> OperationList { get; set; }

        public HydraClass()
        {
            OperationList = new List<HydraOperation>();
            Type = GetType().Name.Replace("Hypermedia", "");
        }

        public bool CanPut()
        {
            return GetPut() != null;
        }

        public bool CanPost()
        {
            return GetPost() != null;
        }

        public bool CanDelete()
        {
            return GetDelete() != null;
        }

        public HydraOperation GetPut ()
        {
            return GetOperation(HttpMethod.PUT);
        }

        public HydraOperation GetPost()
        {
            return GetOperation(HttpMethod.POST);
        }

        public HydraOperation GetDelete()
        {
            return GetOperation(HttpMethod.DELETE);
        }

        public HydraOperation GetOperation(HttpMethod method)
        {
            return OperationList.FirstOrDefault(x => x.Method == method);
        }


    }
}
