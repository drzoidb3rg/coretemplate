using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DocSupply.Models;
using System.Net.Http;


namespace DocSupply.Features.Session
{
    [Route("sessionerror")]
    public class SessionErrorController : Controller
    {
        // GET: api/values
        [HttpGet]
        public string Get()
        {
            var q = Request.QueryString;
            return "Error, could not log in";
        }
        

    }
}
