using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSupply.RisParser
{
    [Serializable]
    public class RisLine
    {
        public RisLine(LineType lineType)
        {
            LineType = lineType;
        }
        public LineType LineType { get; set; }
        public string Code { get; set; }
        public string Data { get; set; }
        public RisLine[] ContinuationLines { get; internal set; }

    }
}
