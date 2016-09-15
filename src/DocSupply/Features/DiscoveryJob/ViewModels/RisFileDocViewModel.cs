using DocSupply.RisParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.Features.DiscoveryJob.ViewModels
{
    public class RisFileDocViewModel
    {
        public RisFileDocViewModel(RisFileDoc risFileDoc)
        {
            PopulateFields(risFileDoc.Lines);
        }

        public string Type { get; private set; }
        public string Title { get; private set; }

        public void PopulateFields(IEnumerable<RisLine> lines)
        {
            Type = lines.FirstOrDefault(x => x.Code.Equals(RisCode.Type)).Data;
            Title = lines.FirstOrDefault(x => x.Code.Equals(RisCode.Title)).Data;
        }
    }
}
