using DocSupply.RisParser;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.Tests.Features.RisFile
{
    [TestFixture]
    public class RisFileParserTests
    {

        [TestCase]
        public void CanParseStandardLine()
        {
            var lineCode = "T1";
            var lineTagSuffix = "  - ";
            var lineTag = string.Concat(lineCode, lineTagSuffix);
            var lineData = "This is a test standard line.";
            var line = string.Concat(lineTag, lineData);

            var parsedLine = RisFileParser.ParseLine(line);

            Assert.IsNotNull(parsedLine);
            Assert.AreEqual(lineCode, parsedLine.Code);
            Assert.AreEqual(lineData, parsedLine.Data);
        }

        [Test]
        public void CanParseDataOnlyLine()
        {
            var line = "This is a test data continuation line.";
            var parsedLine = RisFileParser.ParseLine(line);

            Assert.IsNotNull(parsedLine);
            Assert.IsNull(parsedLine.Code);
            Assert.AreEqual(line, parsedLine.Data);
        }

        [Test]
        public void CanParseSeparatorLine()
        {
            var line = string.Empty;
            var parsedLine = RisFileParser.ParseLine(line);

            Assert.IsNotNull(parsedLine);
            Assert.IsNull(parsedLine.Code);
            Assert.AreEqual(null, parsedLine.Data);
        }
    }
}
