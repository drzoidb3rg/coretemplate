using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Models;

namespace DocSupply.Features.Session
{
    public class SessionRecord : RavenRecord
    {
        public string Token { get; set; }

        public AthensUser AthensUser { get; set; }
    }
}
