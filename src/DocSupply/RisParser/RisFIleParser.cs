using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocSupply.RisParser
{
    public class RisFileParser
    {
        private static string _standardLineReStr = @"^(?<tag>(?<code>[A-Z][A-Z0-9])  - (?<data>[^\r\n]*))";
        private static string _dataOnlyLineReStr = @"^(?![A-Z][A-Z0-9]  - )(?<data>[^\r\n]+)";
        private static string _separatorLineReStr = @"^$";

        private static readonly Regex StandardLineRe;
        private static readonly Regex DataOnlyLineRe;
        private static readonly Regex SeparatorLineRe;

        static RisFileParser()
        {
            StandardLineRe = new Regex(_standardLineReStr.ToPlatformSpecificLineEndings());
            DataOnlyLineRe = new Regex(_dataOnlyLineReStr.ToPlatformSpecificLineEndings());
            SeparatorLineRe = new Regex(_separatorLineReStr.ToPlatformSpecificLineEndings());
        }

        public static RisFile ParseRisFile(IFormFile file)
        {
            var parsedFile = new RisFile();
            var parsedLines = new List<RisLine>();

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                            parsedLines.Add(ParseLine(line));
                    }
                }
                parsedFile.Docs = AssignLinesToDocs(parsedLines);
                parsedFile.Valid = true;
            }
            catch (Exception ex)
            {
                parsedFile.Valid = false;
            }
            return parsedFile;
        }

        public static RisLine ParseLine(string line)
        {
            var risLine = new RisLine(LineType.None);

            var match = StandardLineRe.Match(line);
            if (match.Success)
            {
                risLine.LineType = LineType.Standard;
                risLine.Code = match.Groups["code"].Value;
                risLine.Data = match.Groups["data"].Value;
            }
            else
            {
                match = DataOnlyLineRe.Match(line);
                if (match.Success)
                {
                    risLine.LineType = LineType.DataContinuation;
                    risLine.Data = match.Groups["data"].Value;
                }
                else
                {
                    match = SeparatorLineRe.Match(line);
                    if (match.Success)
                    {
                        risLine.LineType = LineType.Separator;
                    }
                    else
                    {
                        throw new Exception("Unexpected case parsing file!");
                    }
                }
            }
            return risLine;
        }

        private static List<RisFileDoc> AssignLinesToDocs(IEnumerable<RisLine> parsedLines)
        {
            var docs = new List<RisFileDoc>();
            RisFileDoc currentDoc = null;
            RisLine curContinuationRootLine = null;
            var curContinuationLines = new List<RisLine>();

            var withinDoc = false;
            RisLine prevLine = null;
            foreach (var risLine in parsedLines)
            {
                if (withinDoc)
                {
                    if (risLine.LineType == LineType.Standard && curContinuationLines.Any())
                    {
                        curContinuationRootLine.ContinuationLines = curContinuationLines.ToArray();
                        curContinuationRootLine = null;
                        curContinuationLines.Clear();
                    }

                    if (risLine.LineType == LineType.Standard && risLine.Code == "ER")
                    {
                        currentDoc.Lines.Add(risLine);
                        docs.Add(currentDoc);
                        currentDoc = null;
                        withinDoc = false;
                    }
                    else if (risLine.LineType != LineType.None)
                    {
                        switch (risLine.LineType)
                        {
                            case LineType.Standard:
                                currentDoc.Lines.Add(risLine);
                                curContinuationRootLine = risLine;
                                break;

                            case LineType.DataContinuation:
                                curContinuationLines.Add(risLine);
                                break;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected line");
                    }
                }
                else
                {
                    if (risLine.LineType == LineType.Standard && risLine.Code == "TY")
                    {
                        withinDoc = true;
                        currentDoc = new RisFileDoc();
                        currentDoc.Lines.Add(risLine);
                    }
                    else if (risLine.LineType == LineType.Separator)
                    {
                        // do nothing
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected line");
                    }
                }
                prevLine = risLine; 
            }
            return docs;
        }
    }
}
