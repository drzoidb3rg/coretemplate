using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Models;
using DocSupply.RisParser;

namespace DocSupply.Features.DiscoveryJob
{
    public class DiscoveryJobRecord : RavenRecord
    {

        public string FileName { get; set; }
        public RisFile Risfile { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

        public DiscoveryJobRecord()
        {
        }

        public DiscoveryJobRecord(string filename, RisFile risfile)
        {
            FileName = filename;
            Risfile = risfile;
        }
    }
}
