using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{code}")]
        public IActionResult Index(string code)
        {
            return View("Error/" + code);
        }
    }
}