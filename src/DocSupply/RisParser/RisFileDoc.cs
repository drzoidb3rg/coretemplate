using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.RisParser
{
    public class RisFileDoc
    {
        public RisFileDoc()
        {
            Lines = new List<RisLine>();
        }
        public List<RisLine> Lines { get; private set; }
    }
}
