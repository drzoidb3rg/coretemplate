using System.Collections.Generic;
using DocSupply.Framework.Hypermedia;

namespace DocSupply.Features.DiscoveryJob
{
    public class DiscoveryJobListHypermedia : HydraClass
    {
        public List<DiscoveryJobHypermedia> Collection { get; set; }


        public DiscoveryJobListHypermedia()
        {
            Collection = new List<DiscoveryJobHypermedia>();
        }

        public DiscoveryJobListHypermedia(List<DiscoveryJobRecord> discoveryJobList) : this()
        {
            foreach (var job in discoveryJobList)
            {
                Collection.Add(new DiscoveryJobHypermedia(job));
            }
        }

        public void AddPost()
        {
            var operartion = new HydraOperation { Method = HttpMethod.POST };

            operartion.SupportingPropertyList.Add(new SupportingProperty {Property = "Risfile", Range = "File"});

            OperationList.Add(operartion);
        }
    }
}
