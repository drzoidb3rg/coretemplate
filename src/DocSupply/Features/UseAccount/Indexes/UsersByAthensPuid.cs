using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace DocSupply.Features.UserAccount
{

    public class UsersByAthensPuid : AbstractIndexCreationTask<User>
    {
        public UsersByAthensPuid()
        {
            Map = docs => docs.Select(x => new { x.AthensPuid });
        }
    }
}
