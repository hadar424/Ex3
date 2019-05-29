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
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult Save(string ip, int port, int time, int saveTime, string file)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            ViewBag.time = time;
            ViewBag.saveTime = saveTime;
            ViewBag.file = file;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Time = time;
            CommandChannel.Instance.Start();


            string data = CommandChannel.Instance.GetInfo();
            Debug.WriteLine("before getData");
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            Debug.WriteLine("after getData");

            ViewBag.lon = lon;
            ViewBag.lat = lat;

            // read from file
            Session["time"] = time;

            return View();

        }

        // GET: First
        public ActionResult Map(string ip, int port)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Start();
            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);

            ViewBag.lon = lon;
            ViewBag.lat = lat;

            return View();
        }

        public float getData(string line, int index)
        {
            string parseString = "";
            string[] values = line.Split(' ');
            parseString = values[index];
            return float.Parse(parseString);
        }

        public ActionResult display(string ip, int port, int time)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Time = time;
            CommandChannel.Instance.Start();


            string data = CommandChannel.Instance.GetInfo();
            Debug.WriteLine("before getData");
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            Debug.WriteLine("after getData");

            ViewBag.lon = lon;
            ViewBag.lat = lat;

            // read from file
            Session["time"] = time;

            return View();
        }

        [HttpPost]
        public string GetLonLat()
        {
            Debug.WriteLine("GetLonLat");
             string data = CommandChannel.Instance.GetInfo();
            

            return ToXml(data);
        }

        private string ToXml(string data)
        {
            Debug.WriteLine("ToXml");

            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            // parse data string
            Random rnd = new Random();
            float lon = getData(data, 0) + rnd.Next(50);
            float lat = getData(data, 1) + rnd.Next(50);

            writer.WriteStartDocument();
            writer.WriteStartElement("Sampling");
            writer.WriteElementString("Lon", lon.ToString());
            writer.WriteElementString("Lat", lat.ToString());
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