using System;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using Shouldly.Configuration;


namespace DocSupply.Tests
{


    [TestFixture]
    public class ConnegTests
    {
        public ConnegTests()
        {
           WebTest.WithDiffTool();
        }

        [TestCase]
        public void ConnegTests_GetJson()
        {
            using (var _ = Tester.AsUserOne()
                .AsJson()
                .Get("/conneg/1")
                .IsOk()
                .IsJsonContent()) { }
        }

        [TestCase]
        public void ConnegTests_GetHtml()
        {
            using (var _ = Tester.AsUserOne()
                .AsHtml()
                .Get("/conneg/1")
                .IsOk()
                .IsHtmlContent())
            {
                _.ResponseString().ShouldMatchApproved(_.WithApprovalFolder);
            }
        }

        [TestCase]
        public void ConnegTests_PutAsJson()
        {
            using (var _ = Tester.AsUserOne()
                .AsJson()
                .Put("/conneg/1", new HttpForm().Add("Title", "[new title]").AsFormContent())
                .IsOk()
                .IsJsonContent())
            {
                _.PrettyJsonResponse().ShouldMatchApproved(_.WithApprovalFolder);
            }
        }
    }
}
