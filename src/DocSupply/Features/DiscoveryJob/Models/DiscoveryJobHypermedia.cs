using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Framework.Hypermedia;
using DocSupply.RisParser;

namespace DocSupply.Features.DiscoveryJob
{
    public class DiscoveryJobHypermedia : HydraClass
    {
        public string FileName { get; set; }
        public RisFile Risfile { get; set; }
        public string UserName { get; set; }

        public DiscoveryJobHypermedia(DiscoveryJobRecord discoveryJob)
        {
            FileName = discoveryJob.FileName;
            Risfile = discoveryJob.Risfile;
            UserName = discoveryJob.UserName;
            Id = "discoveryjob/" + discoveryJob.GetIntegerId();
        }
    }
}
