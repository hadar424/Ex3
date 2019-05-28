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
        public ActionResult Index()
        {
            ViewBag.ip = "127.0.0.1";
            ViewBag.port = "5402";
            CommandChannel.Instance.ServerIP = "127.0.0.1";
            CommandChannel.Instance.CommandPort = 5402;
            Debug.WriteLine("start");
            CommandChannel.Instance.Start();
            string data = CommandChannel.Instance.GetInfo();
            Debug.WriteLine("data !!!!!!!!! " + data);
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            Debug.WriteLine("lon !!!!!!!!! " + lon.ToString());
            Debug.WriteLine("lat !!!!!!!!! " + lat.ToString());

            ViewBag.lon = lon;
            ViewBag.lat = lat;

            return View();
        }

        public float getData(string line, int index)
        {
            Debug.WriteLine("getData");
            string parseString = "";
            string[] values = line.Split(' ');
            parseString = values[index];
            Debug.WriteLine("parseString " + parseString);
            return float.Parse(parseString);
        }

        public ActionResult display(string ip, int port, int time)
        {

            ViewBag.ip = "127.0.0.1";
            ViewBag.port = "5402";
            CommandChannel.Instance.ServerIP = "127.0.0.1";
            CommandChannel.Instance.CommandPort = 5402;
            CommandChannel.Instance.Time = time;
            Debug.WriteLine("start");
            CommandChannel.Instance.Start();

            // read from file
            Session["time"] = time;

            return View();
        }

        public string GetInfo()
        {
            return CommandChannel.Instance.GetInfo();
        }
        private string ToXml(string data)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            // parse  data string 
            string lon = "";
            string lat = "";
            string rudder = "";
            string throttle = "";

            writer.WriteStartDocument();
            writer.WriteStartElement("Data");
            writer.WriteStartElement("Sampling");
            writer.WriteElementString("Lon", lon);
            writer.WriteElementString("Lat", lat);
            writer.WriteElementString("Throttle", throttle);
            writer.WriteElementString("Rudder", rudder);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
        public string Search(string name)
        {
            return "";
        }
    }
}