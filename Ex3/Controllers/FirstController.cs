using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ex3.Models;

namespace Ex3.Controllers
{
    public class FirstController : Controller
    {
        // GET: First
        public ActionResult Index(string IP, string myPort)
        {
            ViewBag.ip = "127.0.0.1";
            ViewBag.port = "5402";
            CommandChannel.Instance.ServerIP = "127.0.0.1";
            CommandChannel.Instance.CommandPort = 5402;
            CommandChannel.Instance.Start();

            return View();
        }
    }
}