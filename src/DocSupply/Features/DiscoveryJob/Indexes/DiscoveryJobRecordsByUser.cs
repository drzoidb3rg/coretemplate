using System.Linq;
using Raven.Client.Indexes;

namespace DocSupply.Features.DiscoveryJob
{

    public class DiscoveryJobRecordsByUser : AbstractIndexCreationTask<DiscoveryJobRecord>
    {
        public DiscoveryJobRecordsByUser()
        {
            Map = docs => docs.Select(x => new { x.UserId });
        }
    }
}
