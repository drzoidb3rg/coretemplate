using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.RisParser
{
    public static class RisFileExtensions
    {
        public static string ToPlatformSpecificLineEndings(this string str)
        {
            var normalised = str.Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);

            return normalised;
        }
    }
}
