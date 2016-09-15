using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Framework;
using Microsoft.AspNetCore.Mvc;
using Raven.Client;
using System.Security.Claims;

namespace DocSupply.Controllers
{
    public class DummyDocument
    {
        public string Id { get; set; }
        public DateTime CreatedDate {get;set;}

        public DummyDocument()
        {
            Id = "DummyDocument/";
        }
    }


   // [Route("Home")]
    public class HomeController : Controller
    {
        public IDocumentStore _store;

        public HomeController(IDocumentStore documentStore)
        {
            _store = documentStore;
        }


        public IActionResult Index()
        {
            var name = User.Identity.Name;

            return View();
        }

        public IActionResult About()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(new DummyDocument { CreatedDate = DateTime.Now });
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var list = session.Query<DummyDocument>().Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).ToList();

                ViewData["Message"] = string.Format("Last dummy is {0}, total is {1}", list.LastOrDefault().Id, list.Count);
            }


            return View();
        }

        public string TestMe()
        {

            return "monkey";
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
