using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
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
            Debug.WriteLine("start");
            CommandChannel.Instance.Start();

            return View();
        }

        private string ToXml(Employee employee)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Employess");

            employee.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
    }
}