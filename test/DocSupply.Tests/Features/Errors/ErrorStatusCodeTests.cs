using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client;

namespace DocSupply.Tests.Features.Errors
{
    [TestFixture]
    public class ErrorStatusCodeTests
    {
        [TestCase]
        public void Returns404ForJsonRequest()
        {
            using (var _ = Tester.AsUnknown()
              .AsJson()
              .Get("/fakeUrl")
              .IsNotFound()) { }
        }

        [TestCase]
        public void Returns404ForHtmlRequest()
        {
            using (var _ = Tester.AsUnknown()
              .AsHtml()
              .Get("/fakeUrl")
              .IsNotFound()
              .IsHtmlContent()
              .ResponseContains("<h1>Page not found</h1>")) { }
        }

        [TestCase]
        public void Returns500ForJsonRequest()
        {
            using (var _ = Tester.AsUserOne()
             .AsJson()
             .WithStoreAction(store => store.Dispose()) //force an internal error
             .Get("/discoveryJob/1")
             .IsInternalServerError()) { }
        }

        [TestCase]
        public void Returns500ForHtmlRequest()
        {
            using (var _ = Tester.AsUserOne()
                .AsHtml()
                .WithStoreAction(store => store.Dispose()) //force an internal error
                .Get("/discoveryJob/1")
                .IsInternalServerError()
                .IsHtmlContent()
                .ResponseContains("<h1>Something's gone wrong</h1>")) {}
        }
    }
}
