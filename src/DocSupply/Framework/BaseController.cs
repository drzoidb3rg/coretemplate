using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client;

namespace DocSupply.Framework
{
    public class BaseController : Controller
    {
        public IDocumentStore Store { get; }

        public BaseController(IDocumentStore documentStore)
        {
            Store = documentStore;
        }
    }
}
