using DocSupply.Features.DiscoveryJob;
using NUnit.Framework;
using Shouldly;

namespace DocSupply.Tests.Features.DiscoveryJob
{
    [TestFixture]
    public class DiscoverJobTests
    {
        public DiscoverJobTests()
        {
            WebTest.WithDiffTool();

        }

        [TestCase]
        public void DiscoverJobTests_PostRisFilesJSON()
        {
            using (var store = WebTest.GetDocumentStore())
            {
                using (var _ = Tester.AsUserTwo()
                    .WithStore(store)
                    .WithIndex(new DiscoveryJobRecordsByUser())
                    .PostFile("/DiscoveryJobList", @"Features\DiscoveryJob\example_export_from_EPPI.ris", "file one")
                    .AsJson()
                    .Get("/DiscoveryJobList"))
                {
                    _.PrettyJsonResponse().ShouldMatchApproved(_.WithApprovalFolder);
                }
            }
        }

        [TestCase]
        public void DiscoverJobTests_PostRisFiles()
        {
            using (var store = WebTest.GetDocumentStore())
            {
                using (Tester.AsUserTwo()
                    .WithStore(store)
                    .PostFile("/DiscoveryJobList", @"Features\DiscoveryJob\example_export_from_EPPI.ris", "file one"))

                using (var _ = Tester.AsUserOne()
                   .WithStore(store)
                   .WithIndex(new DiscoveryJobRecordsByUser())
                   .PostFile("/DiscoveryJobList", @"Features\DiscoveryJob\example_export_from_EPPI.ris", "file one")
                   .PostFile("/DiscoveryJobList", @"Features\DiscoveryJob\example_export_from_EPPI.ris", "file two")
                   .Get("/DiscoveryJobList"))
                {
                    _.ResponseString().ShouldMatchApproved(_.WithApprovalFolder);
                };
            }
        }


        [TestCase]
        public void DiscoverJobTests_PostInvalidRisFile()
        {
            using (var store = WebTest.GetDocumentStore())
            {
                using (var _ = Tester.AsUserOne()
                   .WithStore(store)
                   .PostFile("/DiscoveryJobList", @"Features\DiscoveryJob\example_invalid_file_format.ris", "some file")
                   .IsRedirect())
                {
                    var doc = _.GetFromStore<DiscoveryJobRecord>("DiscoveryJobRecord/1");
                    Assert.AreEqual(null, doc);
                };
            }
        }

        [TestCase]
        public void DiscoverJobTests_PostRisFileWithParagraphs()
        {
            using (var store = WebTest.GetDocumentStore())
            {
                using (var _ = Tester.AsUserOne()
                   .WithStore(store)
                   .PostFile("/DiscoveryJoblist", @"Features\DiscoveryJob\ris_file_with_paragraphs.txt", "some file")
                   .IsRedirect())
                {
                    var doc = _.GetFromStore<DiscoveryJobRecord>("DiscoveryJobRecord/1");
                    Assert.AreEqual("some file", doc.FileName);
                    Assert.AreEqual(true, doc.Risfile.Valid);
                };
            }
        }

    }
}
