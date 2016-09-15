using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSupply.RisParser
{
    public class RisFile
    {
        public RisFile()
        {
            Valid = false;
            Docs = new List<RisFileDoc>();
        }
        public List<RisFileDoc> Docs { get; set; }
        public bool Valid { get; set; }


    }
}
