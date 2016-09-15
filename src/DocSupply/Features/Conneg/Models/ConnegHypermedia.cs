using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocSupply.Framework.Hypermedia;

namespace DocSupply.Features.Conneg.Models
{

    public class ConnegHypermedia : HydraClass
    {
        public ConnegHypermedia(Conneg conneg) : base()
        {
            Title = conneg.Title;
            Id = conneg.Id;
        }

        public void AddPut()
        {
            //this to be streamlined at some point
            var operartion = new HydraOperation{Method = HttpMethod.PUT};

            operartion.SupportingPropertyList.Add(new SupportingProperty("Title"));

            OperationList.Add(operartion);
        }
    }
}
