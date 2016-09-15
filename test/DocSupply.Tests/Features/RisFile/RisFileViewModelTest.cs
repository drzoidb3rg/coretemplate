using DocSupply.Features.DiscoveryJob.ViewModels;
using DocSupply.RisParser;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DocSupply.Tests.Features.RisFile
{
    [TestFixture]
    public class RisFileViewModelTest
    {
        [TestCase]
        public void CanCreateRisFileViewModel()
        {

            var docOne = new RisFileDoc();

            var lineOne = new RisLine(LineType.Standard)
            {
                Code = "TY",
                Data = "This is the type."
            };
            var lineTwo = new RisLine(LineType.Standard)
            {
                Code = "T1",
                Data = "This is the title"
            };
            var lineThree = new RisLine(LineType.Standard)
            {
                Code = "ER",
                Data = "This is the end"
            };
            docOne.Lines.Add(lineOne);
            docOne.Lines.Add(lineTwo);
            docOne.Lines.Add(lineThree);
            var vm = new RisFileDocViewModel(docOne);

            Assert.AreEqual("This is the title", vm.Title);
        }

    }
}
