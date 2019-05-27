using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ex3.Controllers
{
    public class FirstController : Controller
    {
        // GET: First
        public ActionResult Index(string ip, string myPort)
        {
            ViewBag.ip = ip;
            ViewBag.port = myPort;
            return View();
        }
    }
}