using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.RisParser
{
    public enum LineType
    {
        None = 0,
        Standard,
        DataContinuation,
        Separator
    }
}
